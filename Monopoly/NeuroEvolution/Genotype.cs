using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    public class VertexInfo
    {
        public enum EType
        {
            INPUT = 0,
            HIDDEN = 1,
            OUTPUT = 2,
        }

        public EType type;
        public int index = 0;

        public VertexInfo(EType t, int i)
        {
            type = t;
            index = i;
        }
    }

    public class EdgeInfo
    {
        //structual information
        public int source = 0;
        public int destination = 0;

        //network information
        public float weight = 0.0f;
        public bool enabled = false;
        public int innovation = 0;

        public EdgeInfo(int s, int d, float w, bool e)
        {
            source = s;
            destination = d;

            weight = w;
            enabled = e;
        }
    }

    public class Genotype
    {
        public List<VertexInfo> vertices;
        public List<EdgeInfo> edges;

        public int inputs = 0;
        public int externals = 0;

        public int bracket = 0;

        public float fitness = 0.0f;
        public float adjustedFitness = 0.0f;
            
        public Genotype()
        {
            vertices = new List<VertexInfo>();
            edges = new List<EdgeInfo>();
        }

        public void AddVertex(VertexInfo.EType type, int index)
        {
            VertexInfo v = new VertexInfo(type, index);
            vertices.Add(v);

            if (v.type != VertexInfo.EType.HIDDEN)
            {
                externals++;
            }

            if (v.type == VertexInfo.EType.INPUT)
            {
                inputs++;
            }
        }

        public void AddEdge(int source, int destination, float weight, bool enabled)
        {
            EdgeInfo e = new EdgeInfo(source, destination, weight, enabled);
            edges.Add(e);
        }

        public void AddEdge(int source, int destination, float weight, bool enabled, int innovation)
        {
            EdgeInfo e = new EdgeInfo(source, destination, weight, enabled);
            e.innovation = innovation;
            edges.Add(e);
        }

        public Genotype Clone()
        {
            Genotype copy = new Genotype();

            int vertexCount = vertices.Count;

            for (int i = 0; i < vertexCount; i++)
            {
                copy.AddVertex(vertices[i].type, vertices[i].index);
            }

            int edgeCount = edges.Count;

            for (int i = 0; i < edgeCount; i++)
            {
                copy.AddEdge(edges[i].source, edges[i].destination, edges[i].weight, edges[i].enabled, edges[i].innovation);
            }

            return copy;
        }

        public void SortTopology()
        {
            SortVertices();
            SortEdges();
        }

        public void SortVertices()
        {
            vertices.Sort(CompareVertexByOrder);
        }

        public void SortEdges()
        {
            edges.Sort(CompareEdgeByInnovation);
        }

        public int CompareVertexByOrder(VertexInfo a, VertexInfo b)
        {
            if (a.index > b.index)
            {
                return 1;
            }
            else if (a.index == b.index)
            {
                return 0;
            }

            return -1;
        }

        public int CompareEdgeByInnovation(EdgeInfo a, EdgeInfo b)
        {
            if (a.innovation > b.innovation)
            {
                return 1;
            }
            else if (a.innovation == b.innovation)
            {
                return 0;
            }

            return -1;
        }
    }
}
