using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    public class Crossover
    {
        public static Crossover instance = null;

        public float CROSSOVER_CHANCE = 0.75f;

        public float C1 = 1.0f;
        public float C2 = 1.0f;
        public float C3 = 0.4f;
        public float DISTANCE = 1.0f;

        public static void Initialise()
        {
            if (instance == null)
            {
                instance = new Crossover();
            }
        }

        public Crossover()
        {
           
        }

        public Genotype ProduceOffspring(Genotype first, Genotype second)
        {
            List<EdgeInfo> copy_first = new List<EdgeInfo>();
            List<EdgeInfo> copy_second = new List<EdgeInfo>();

            copy_first.AddRange(first.edges);
            copy_second.AddRange(second.edges);

            List<EdgeInfo> match_first = new List<EdgeInfo>();
            List<EdgeInfo> match_second = new List<EdgeInfo>();

            List<EdgeInfo> disjoint_first = new List<EdgeInfo>();
            List<EdgeInfo> disjoint_second = new List<EdgeInfo>();

            List<EdgeInfo> excess_first = new List<EdgeInfo>();
            List<EdgeInfo> excess_second = new List<EdgeInfo>();

            int genes_first = first.edges.Count;
            int genes_second = second.edges.Count;

            int invmax_first = first.edges[first.edges.Count - 1].innovation;
            int invmax_second = second.edges[second.edges.Count - 1].innovation;

            int invmin = invmax_first > invmax_second ? invmax_second : invmax_first;

            for (int i = 0; i < genes_first; i++)
            {
                for (int j = 0; j < genes_second; j++)
                {
                    EdgeInfo info_first = copy_first[i];
                    EdgeInfo info_second = copy_second[j];

                    //matching genes
                    if (info_first.innovation == info_second.innovation)
                    {
                        match_first.Add(info_first);
                        match_second.Add(info_second);

                        copy_first.Remove(info_first);
                        copy_second.Remove(info_second);

                        i--;
                        genes_first--;
                        genes_second--;
                        break;
                    }
                }
            }

            for (int i = 0; i < copy_first.Count; i++)
            {
                if (copy_first[i].innovation > invmin)
                {
                    excess_first.Add(copy_first[i]);
                }
                else
                {
                    disjoint_first.Add(copy_first[i]);
                }
            }

            for (int i = 0; i < copy_second.Count; i++)
            {
                if (copy_second[i].innovation > invmin)
                {
                    excess_second.Add(copy_second[i]);
                }
                else
                {
                    disjoint_second.Add(copy_second[i]);
                }
            }

            Genotype child = new Genotype();

            int matching = match_first.Count;

            for (int i = 0; i < matching; i++)
            {
                int roll = RNG.instance.gen.Next(0, 2);
                
                if (roll == 0 || !match_second[i].enabled)
                {
                    child.AddEdge(match_first[i].source, match_first[i].destination, match_first[i].weight, match_first[i].enabled, match_first[i].innovation);
                }
                else
                {
                    child.AddEdge(match_second[i].source, match_second[i].destination, match_second[i].weight, match_second[i].enabled, match_second[i].innovation);
                }
            }

            for (int i = 0; i < disjoint_first.Count; i++)
            {
                child.AddEdge(disjoint_first[i].source, disjoint_first[i].destination, disjoint_first[i].weight, disjoint_first[i].enabled, disjoint_first[i].innovation);
            }

            for (int i = 0; i < excess_first.Count; i++)
            {
                child.AddEdge(excess_first[i].source, excess_first[i].destination, excess_first[i].weight, excess_first[i].enabled, excess_first[i].innovation);
            }

            child.SortEdges();

            List<int> ends = new List<int>();

            int vertexCount = first.vertices.Count;

            for (int i = 0; i < first.vertices.Count; i++)
            {
                VertexInfo vertex = first.vertices[i];

                if (vertex.type == VertexInfo.EType.HIDDEN)
                {
                    break;
                }

                ends.Add(vertex.index);
                child.AddVertex(vertex.type, vertex.index);
            }

            AddUniqueVertices(child, ends);

            child.SortVertices();

            return child;
        }

        public void AddUniqueVertices(Genotype genotype, List<int> ends)
        {
            List<int> unique = new List<int>();

            int edgeCount = genotype.edges.Count;

            for (int i = 0; i < edgeCount; i++)
            {
                EdgeInfo info = genotype.edges[i];

                if (!ends.Contains(info.source) && !unique.Contains(info.source))
                {
                    unique.Add(info.source);
                }

                if (!ends.Contains(info.destination) && !unique.Contains(info.destination))
                {
                    unique.Add(info.destination);
                }
            }

            int uniques = unique.Count;

            for (int i = 0; i < uniques; i++)
            {
                genotype.AddVertex(VertexInfo.EType.HIDDEN, unique[i]);
            }
        }

        public float SpeciationDistance(Genotype first, Genotype second)
        {
            List<EdgeInfo> copy_first = new List<EdgeInfo>();
            List<EdgeInfo> copy_second = new List<EdgeInfo>();

            copy_first.AddRange(first.edges);
            copy_second.AddRange(second.edges);

            List<EdgeInfo> match_first = new List<EdgeInfo>();
            List<EdgeInfo> match_second = new List<EdgeInfo>();

            List<EdgeInfo> disjoint_first = new List<EdgeInfo>();
            List<EdgeInfo> disjoint_second = new List<EdgeInfo>();

            List<EdgeInfo> excess_first = new List<EdgeInfo>();
            List<EdgeInfo> excess_second = new List<EdgeInfo>();

            int genes_first = first.edges.Count;
            int genes_second = second.edges.Count;

            int invmax_first = first.edges[first.edges.Count - 1].innovation;
            int invmax_second = second.edges[second.edges.Count - 1].innovation;

            int invmin = invmax_first > invmax_second ? invmax_second : invmax_first;

            float diff = 0.0f;

            for (int i = 0; i < genes_first; i++)
            {
                for (int j = 0; j < genes_second; j++)
                {
                    EdgeInfo info_first = copy_first[i];
                    EdgeInfo info_second = copy_second[j];

                    //matching genes
                    if (info_first.innovation == info_second.innovation)
                    {
                        float weightDiff = Math.Abs(info_first.weight - info_second.weight);
                        diff += weightDiff;

                        match_first.Add(info_first);
                        match_second.Add(info_second);

                        copy_first.Remove(info_first);
                        copy_second.Remove(info_second);

                        i--;
                        genes_first--;
                        genes_second--;
                        break;
                    }
                }
            }

            for (int i = 0; i < copy_first.Count; i++)
            {
                if (copy_first[i].innovation > invmin)
                {
                    excess_first.Add(copy_first[i]);
                }
                else
                {
                    disjoint_first.Add(copy_first[i]);
                }
            }

            for (int i = 0; i < copy_second.Count; i++)
            {
                if (copy_second[i].innovation > invmin)
                {
                    excess_second.Add(copy_second[i]);
                }
                else
                {
                    disjoint_second.Add(copy_second[i]);
                }
            }

            int match = match_first.Count;
            int disjoint = disjoint_first.Count + disjoint_second.Count;
            int excess = excess_first.Count + excess_second.Count;

            int n = Math.Max(first.edges.Count, second.edges.Count);

            float E = excess / (float)n;
            float D = disjoint / (float)n;
            float W = diff / (float)match;

            return E * C1 + D * C2 + W * C3;
        }
    }
}
