using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    public class Marking
    {
        public int order = 0;
        public int source = 0;
        public int destination = 0;
    }

    public class Mutation
    {
        public static Mutation instance = null;

        public float MUTATE_LINK = 0.2f;
        public float MUTATE_NODE = 0.1f;
        public float MUTATE_ENABLE = 0.6f;
        public float MUTATE_DISABLE = 0.2f;
        public float MUTATE_WEIGHT = 2.0f;

        //chances
        public float PETRUB_CHANCE = 0.9f;
        public float SHIFT_STEP = 0.1f;

        public List<Marking> historical = new List<Marking>();

        public static void Initialise()
        {
            if (instance == null)
            {
                instance = new Mutation();
            }
        }

        public Mutation()
        {

        }

        public int RegisterMarking(EdgeInfo info)
        {
            int count = historical.Count;

            for (int i = 0; i < count; i++)
            {
                Marking marking = historical[i];

                if (marking.source == info.source && marking.destination == info.destination)
                {
                    return marking.order;
                }
            }

            Marking creation = new Marking();
            creation.order = historical.Count;
            creation.source = info.source;
            creation.destination = info.destination;

            historical.Add(creation);

            return historical.Count - 1;
        }

        public void MutateAll(Genotype genotype)
        {
            MUTATE_LINK = 0.2f;
            MUTATE_NODE = 0.1f;
            MUTATE_ENABLE = 0.6f;
            MUTATE_DISABLE = 0.2f;
            MUTATE_WEIGHT = 2.0f;

            float p = MUTATE_WEIGHT;

            while (p > 0)
            {
                float roll = (float)RNG.instance.gen.NextDouble();

                if (roll < p)
                {
                    MutateWeight(genotype);
                }

                p--;
            }

            p = MUTATE_LINK;

            while (p > 0)
            {
                float roll = (float)RNG.instance.gen.NextDouble();

                if (roll < p)
                {
                    MutateLink(genotype);
                }

                p--;
            }

            p = MUTATE_NODE;

            while (p > 0)
            {
                float roll = (float)RNG.instance.gen.NextDouble();

                if (roll < p)
                {
                    MutateNode(genotype);
                }

                p--;
            }

            p = MUTATE_DISABLE;

            while (p > 0)
            {
                float roll = (float)RNG.instance.gen.NextDouble();

                if (roll < p)
                {
                    MutateDisable(genotype);
                }

                p--;
            }

            p = MUTATE_ENABLE;

            while (p > 0)
            {
                float roll = (float)RNG.instance.gen.NextDouble();

                if (roll < p)
                {
                    MutateEnable(genotype);
                }

                p--;
            }

        }

        public void MutateLink(Genotype genotype)
        {
            int vertexCount = genotype.vertices.Count;
            int edgeCount = genotype.edges.Count;

            List<EdgeInfo> potential = new List<EdgeInfo>();
            
            //gather all possible potential edges
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    int source = genotype.vertices[i].index;
                    int destination = genotype.vertices[j].index;

                    VertexInfo.EType t1 = genotype.vertices[i].type;
                    VertexInfo.EType t2 = genotype.vertices[j].type;

                    if (t1 == VertexInfo.EType.OUTPUT || t2 == VertexInfo.EType.INPUT)
                    {
                        continue;
                    }

                    if (source == destination)
                    {
                        continue;
                    }

                    bool search = false;

                    //match edge
                    for (int k = 0; k < edgeCount; k++)
                    {
                        EdgeInfo edge = genotype.edges[k];

                        if (edge.source == source && edge.destination == destination)
                        {
                            search = true;
                            break;
                        }
                    }

                    if (!search)
                    {
                        float weight = (float)RNG.instance.gen.NextDouble() * 4.0f - 2.0f;
                        EdgeInfo creation = new EdgeInfo(source, destination, weight, true);

                        potential.Add(creation);
                    }
                }
            }

            if (potential.Count <= 0)
            {
                return;
            }

            int selection = RNG.instance.gen.Next(0, potential.Count);

            EdgeInfo mutation = potential[selection];
            mutation.innovation = RegisterMarking(mutation);

            genotype.AddEdge(mutation.source, mutation.destination, mutation.weight, mutation.enabled, mutation.innovation);
        }

        public void MutateNode(Genotype genotype)
        {
            int edgeCount = genotype.edges.Count;

            int selection = RNG.instance.gen.Next(0, edgeCount);

            EdgeInfo edge = genotype.edges[selection];

            if (edge.enabled == false)
            {
                return;
            }

            edge.enabled = false;

            int vertex_new = genotype.vertices[genotype.vertices.Count - 1].index + 1;

            VertexInfo vertex = new VertexInfo(VertexInfo.EType.HIDDEN, vertex_new);

            EdgeInfo first = new EdgeInfo(edge.source, vertex_new, 1.0f, true);
            EdgeInfo second = new EdgeInfo(vertex_new, edge.destination, edge.weight, true);

            first.innovation = RegisterMarking(first);
            second.innovation = RegisterMarking(second);

            genotype.AddVertex(vertex.type, vertex.index);

            genotype.AddEdge(first.source, first.destination, first.weight, first.enabled, first.innovation);
            genotype.AddEdge(second.source, second.destination, second.weight, second.enabled, second.innovation);
        }

        public void MutateEnable(Genotype genotype)
        {
            int edgeCount = genotype.edges.Count;

            List<EdgeInfo> candidates = new List<EdgeInfo>();

            for (int i =0; i < edgeCount; i++)
            {
                if (!genotype.edges[i].enabled)
                {
                    candidates.Add(genotype.edges[i]);
                }
            }

            if (candidates.Count == 0)
            {
                return;
            }

            int selection = RNG.instance.gen.Next(0, candidates.Count);

            EdgeInfo edge = candidates[selection];
            edge.enabled = true;
        }

        public void MutateDisable(Genotype genotype)
        {
            int edgeCount = genotype.edges.Count;

            List<EdgeInfo> candidates = new List<EdgeInfo>();

            for (int i = 0; i < edgeCount; i++)
            {
                if (genotype.edges[i].enabled)
                {
                    candidates.Add(genotype.edges[i]);
                }
            }

            if (candidates.Count == 0)
            {
                return;
            }

            int selection = RNG.instance.gen.Next(0, candidates.Count);

            EdgeInfo edge = candidates[selection];
            edge.enabled = false;
        }

        public void MutateWeight(Genotype genotype)
        {
            int selection = RNG.instance.gen.Next(0, genotype.edges.Count);

            EdgeInfo edge = genotype.edges[selection];

            float roll = (float)RNG.instance.gen.NextDouble();

            if (roll < PETRUB_CHANCE)
            {
                MutateWeightShift(edge, SHIFT_STEP);
            }
            else
            {
                MutateWeightRandom(edge);
            }
        }

        public void MutateWeightShift(EdgeInfo edge, float step)
        {
            float scalar = (float)RNG.instance.gen.NextDouble() * step - step * 0.5f;
            edge.weight += scalar;
        }

        public void MutateWeightRandom(EdgeInfo edge)
        {
            float value = (float)RNG.instance.gen.NextDouble() * 4.0f - 2.0f;
            edge.weight = value;
        }
    }
}
