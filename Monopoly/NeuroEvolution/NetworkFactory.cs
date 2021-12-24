using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    public class NetworkFactory
    {
        public static NetworkFactory instance = null;

        public static void Initialise()
        {
            if (instance == null)
            {
                instance = new NetworkFactory();
            }
        }

        public Genotype CreateBaseGenotype(int inputs, int outputs)
        {
            Genotype network = new Genotype();

            for (int i = 0; i < inputs; i++)
            {
                network.AddVertex(VertexInfo.EType.INPUT, i);
            }

            for (int i = 0; i < outputs; i++)
            {
                network.AddVertex(VertexInfo.EType.OUTPUT, i + inputs);
            }

            network.AddEdge(0, inputs, 0.0f, true, 0);

            //int innovation = 0;
            //
            //for (int i = 0; i < inputs; i++)
            //{
            //    for (int j = 0; j < outputs; j++)
            //    {
            //        int input = i;
            //        int output = j + inputs;
            //
            //        network.AddEdge(input, output, 0.0f, true, innovation);
            //
            //        innovation++;
            //    }
            //}

            return network;
        }

        public void RegisterBaseMarkings(int inputs, int outputs)
        {
            for (int i = 0; i < inputs; i++)
            {
                for (int j = 0; j < outputs; j++)
                {
                    int input = i;
                    int output = j + inputs;

                    EdgeInfo info = new EdgeInfo(input, output, 0.0f, true);

                    Mutation.instance.RegisterMarking(info);
                }
            }
        }

        public Genotype CreateBaseRecurrent()
        {
            Genotype network = new Genotype();

            int nodeNum = 0;

            for (int i = 0; i < 1; i++)
            {
                network.AddVertex(VertexInfo.EType.INPUT, nodeNum);
                nodeNum++;
            }

            for (int i = 0; i < 1; i++)
            {
                network.AddVertex(VertexInfo.EType.OUTPUT, nodeNum);
                nodeNum++;
            }

            network.AddEdge(0, 1, 0.0f, true, 0);
            network.AddEdge(1, 0, 0.0f, true, 1);

            Phenotype physicals = new Phenotype();
            physicals.InscribeGenotype(network);
            physicals.ProcessGraph();

            return network;
        }

        public Genotype CreateBuggyNetwork()
        {
            Genotype network = new Genotype();

            int nodeNum = 0;

            for (int i = 0; i < 2; i++)
            {
                network.AddVertex(VertexInfo.EType.INPUT, nodeNum);
                nodeNum++;
            }

            for (int i = 0; i < 1; i++)
            {
                network.AddVertex(VertexInfo.EType.OUTPUT, nodeNum);
                nodeNum++;
            }

            for (int i = 0; i < 2; i++)
            {
                network.AddVertex(VertexInfo.EType.HIDDEN, nodeNum);
                nodeNum++;
            }

            network.AddEdge(0, 2, 0.0f, true, 0);
            network.AddEdge(1, 2, 0.0f, true, 1);
            network.AddEdge(1, 3, 0.0f, true, 2);
            network.AddEdge(3, 2, 0.0f, true, 3);

            Phenotype physicals = new Phenotype();
            physicals.InscribeGenotype(network);
            physicals.ProcessGraph();

            return network;
        }

        public Phenotype CreateBasePhenotype(int inputs, int outputs)
        {
            Phenotype network = new Phenotype();

            for (int i = 0; i < inputs; i++)
            {
                network.AddVertex(Vertex.EType.INPUT, i);
            }

            for (int i = 0; i < outputs; i++)
            {
                network.AddVertex(Vertex.EType.OUTPUT, i + inputs);
            }

            for (int i = 0; i < inputs; i++)
            {
                for (int j = 0; j < outputs; j++)
                {
                    int input = i;
                    int output = j + inputs;

                    network.AddEdge(input, output, 0.0f, true);
                }
            }

            return network;
        }
    }
}
