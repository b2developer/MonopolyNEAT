using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    public class Vertex
    {
        public enum EType
        {
            INPUT = 0,
            HIDDEN = 1,
            OUTPUT = 2,
        }

        public EType type;
        public int index = 0;

        //structual information
        public List<Edge> incoming;

        //output extraction
        public float value = 0.0f;

        public Vertex(EType t, int i)
        {
            type = t;
            index = i;

            incoming = new List<Edge>();
        }
    }

    public class Edge
    {
        public enum EType
        {
            FORWARD,
            RECURRENT,
        }

        //structual information
        public EType type = EType.FORWARD;
        public int source = 0;
        public int destination = 0;

        //network information
        public float weight = 0.0f;
        public bool enabled = true;

        //propagation information
        public float signal = 0.0f;

        public Edge(int s, int d, float w, bool e)
        {
            source = s;
            destination = d;

            weight = w;
            enabled = e;
        }
    }

    public class Phenotype
    {
        public List<Vertex> vertices;
        public List<Edge> edges;

        public List<Vertex> vertices_inputs;
        public List<Vertex> vertices_outputs;

        public float score = 0;

        public Phenotype()
        {
            vertices = new List<Vertex>();
            edges = new List<Edge>();

            vertices_inputs = new List<Vertex>();
            vertices_outputs = new List<Vertex>();
        }

        public void InscribeGenotype(Genotype code)
        {
            vertices.Clear();
            edges.Clear();

            int vertexCount = code.vertices.Count;
            int edgeCount = code.edges.Count;

            for (int i = 0; i < vertexCount; i++)
            {
                //cast to int then to other enumerator type
                AddVertex((Vertex.EType)(int)code.vertices[i].type, code.vertices[i].index);
            }
            
            for (int i = 0; i < edgeCount; i++)
            {
                AddEdge(code.edges[i].source, code.edges[i].destination, code.edges[i].weight, code.edges[i].enabled);
            }
        }

        public void AddVertex(Vertex.EType type, int index)
        {
            Vertex v = new Vertex(type, index);
            vertices.Add(v);
        }

        public void AddEdge(int source, int destination, float weight, bool enabled)
        {
            Edge e = new Edge(source, destination, weight, enabled);
            edges.Add(e);

            vertices[e.destination].incoming.Add(e);
        }

        public void ProcessGraph()
        {
            int verticesCount = vertices.Count;

            //populate input and output sub-lists
            for (int i = 0; i < verticesCount; i++)
            {
                Vertex vertex = vertices[i];

                if (vertex.type == Vertex.EType.INPUT)
                {
                    vertices_inputs.Add(vertex);
                }
                else if (vertex.type == Vertex.EType.OUTPUT)
                {
                    vertices_outputs.Add(vertex);
                }
            }
        }

        public void ResetGraph()
        {
            int verticesCount = vertices.Count;

            for (int i = 0; i < verticesCount; i++)
            {
                Vertex vertex = vertices[i];
                vertex.value = 0.0f;
            }
        }

        public float[] Propagate(float[] X)
        {
            int repeats = 10;

            for (int e = 0; e < repeats; e++)
            {
                for (int i = 0; i < vertices_inputs.Count; i++)
                {
                    vertices_inputs[i].value = X[i];
                }

                for (int i = 0; i < vertices.Count; i++)
                {
                    if (vertices[i].type == Vertex.EType.OUTPUT)
                    {
                        continue;
                    }

                    int paths = vertices[i].incoming.Count;

                    for (int j = 0; j < paths; j++)
                    {
                        vertices[i].value += vertices[vertices[i].incoming[j].source].value * vertices[i].incoming[j].weight * (vertices[i].incoming[j].enabled ? 1.0f : 0.0f);
                    }

                    if (vertices[i].incoming.Count > 0)
                    {
                        vertices[i].value = Sigmoid(vertices[i].value);
                    }
                }

                float[] Y = new float[vertices_outputs.Count];

                for (int i = 0; i < vertices_outputs.Count; i++)
                {
                    int paths = vertices_outputs[i].incoming.Count;

                    for (int j = 0; j < paths; j++)
                    {
                        vertices_outputs[i].value += vertices[vertices_outputs[i].incoming[j].source].value * vertices_outputs[i].incoming[j].weight * (vertices_outputs[i].incoming[j].enabled ? 1.0f : 0.0f);
                    }

                    if (vertices_outputs[i].incoming.Count > 0)
                    {
                        vertices_outputs[i].value = Sigmoid(vertices_outputs[i].value);
                        Y[i] = vertices_outputs[i].value;
                    }
                }

                if (e == repeats - 1)
                {
                    return Y;
                }
            }

            return new float[0];
        }

        public float Sigmoid(float x)
        {
            return 1.0f / (1.0f + (float)Math.Pow(Math.E, -1.0f * x));
        }
    }
}
