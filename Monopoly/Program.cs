using System;
using System.Collections.Generic;
using System.IO;

namespace Monopoly
{
    class Program
    {
        static void Main(string[] args)
        {
            Analytics a = new Analytics();
            Analytics.instance = a;

            string path = "C:\\Users\\Brad\\Desktop\\monopoly_population.txt";

            RNG.Initialise();

            NEAT.NetworkFactory.Initialise();

            NEAT.Mutation.Initialise();
            NEAT.Crossover.Initialise();
            NEAT.Population.Initialise();

            Tournament tournament = new Tournament();

            //SaveState(path, tournament);

            if (File.Exists(path))
            {
                //tournament.Initialise();
                LoadState(path, ref tournament);
            }
            else
            {
                tournament.Initialise();
            }

            for (int i = 0; i < 1000; i++)
            {
                tournament.ExecuteTournament();
                NEAT.Population.instance.NewGeneration();
                SaveState(path, tournament);
            }
            
        }

        public static char DELIM_MAIN = ';';
        public static char DELIM_COMMA = ',';

        public static void SaveState(string target, Tournament tournament)
        {
            Console.WriteLine("SAVING POPULATION");

            string build = "";
            string build2 = "";

            build += NEAT.Population.instance.GENERATION.ToString();
            build += DELIM_MAIN;
            build += tournament.championScore.ToString();
            build += DELIM_MAIN;

            int markings = 0;

            //save markings
            for (int i = 0; i < NEAT.Mutation.instance.historical.Count; i++)
            {
                build += NEAT.Mutation.instance.historical[i].order;
                build += DELIM_COMMA;

                build += NEAT.Mutation.instance.historical[i].source;
                build += DELIM_COMMA;

                build += NEAT.Mutation.instance.historical[i].destination;

                if (i != NEAT.Mutation.instance.historical.Count - 1)
                {
                    build += DELIM_COMMA;
                }

                markings++;
            }

            List<string> net_build = new List<string>();
            int net_count = -1;
            int gene_count = 0;

            build += DELIM_MAIN; 

            //save neworks, species by species
            for (int i = 0; i < NEAT.Population.instance.species.Count; i++)
            {
                net_build.Add("");
                net_count++;

                net_build[net_count] += NEAT.Population.instance.species[i].topFitness.ToString();
                net_build[net_count] += DELIM_COMMA;
                net_build[net_count] += NEAT.Population.instance.species[i].staleness.ToString();

                net_build[net_count] += "&";

                int members = NEAT.Population.instance.species[i].members.Count;

                for (int j = 0; j < members; j++)
                {
                    net_build.Add("");
                    net_count++;
                    gene_count++;

                    Console.WriteLine(gene_count + "/" + NEAT.Population.instance.genetics.Count);

                    NEAT.Genotype genes = NEAT.Population.instance.species[i].members[j];

                    int vertices = genes.vertices.Count;

                    for (int k = 0; k < vertices; k++)
                    {
                        net_build[net_count] += genes.vertices[k].index.ToString();
                        net_build[net_count] += DELIM_COMMA;
                        net_build[net_count] += genes.vertices[k].type.ToString();
                        net_build[net_count] += DELIM_COMMA;
                    }

                    net_build[net_count] += '#';

                    int edges = genes.edges.Count;

                    for (int k = 0; k < edges; k++)
                    {
                        net_build[net_count] += genes.edges[k].source.ToString();
                        net_build[net_count] += DELIM_COMMA;
                        net_build[net_count] += genes.edges[k].destination.ToString();
                        net_build[net_count] += DELIM_COMMA;
                        net_build[net_count] += genes.edges[k].weight.ToString();
                        net_build[net_count] += DELIM_COMMA;
                        net_build[net_count] += genes.edges[k].enabled.ToString();
                        net_build[net_count] += DELIM_COMMA;
                        net_build[net_count] += genes.edges[k].innovation.ToString();
                        net_build[net_count] += DELIM_COMMA;
                    }

                    if (j != members - 1)
                    {
                        net_build[net_count] += "n";
                    }
                }

                if (i != NEAT.Population.instance.species.Count - 1)
                {
                    net_build[net_count] += "&";
                }          
            }

            build2 += DELIM_MAIN;

            using (StreamWriter sw = new StreamWriter(target))
            {
                sw.Write(build);

                foreach (string b in net_build)
                {
                    sw.Write(b);
                }

                sw.Write(build2);
            }

            Console.WriteLine(markings + " MARKINGS");
        }

        public static void LoadState(string location, ref Tournament tournament)
        {
            string load = "";

            using (StreamReader sr = new StreamReader(location))
            {
                load = sr.ReadToEnd();
            }

            string[] parts = load.Split(DELIM_MAIN);

            int gen = int.Parse(parts[0]);
            float score = float.Parse(parts[1]);

            NEAT.Population.instance.GENERATION = gen;
            tournament.championScore = score;

            string markingString = parts[2];
            string[] markingParts = markingString.Split(DELIM_COMMA);

            for (int i = 0; i < markingParts.GetLength(0); i += 3)
            {
                int order = int.Parse(markingParts[i]);
                int source = int.Parse(markingParts[i + 1]);
                int destination = int.Parse(markingParts[i + 2]);

                NEAT.Marking recreation = new NEAT.Marking();

                recreation.order = order;
                recreation.source = source;
                recreation.destination = destination;

                NEAT.Mutation.instance.historical.Add(recreation);
            }

            string networkString = parts[3];
            string[] speciesParts = networkString.Split('&');

            for (int x = 0; x < speciesParts.GetLength(0); x+=2)
            {
                string[] firstParts = speciesParts[x].Split(DELIM_COMMA);

                NEAT.Population.instance.species.Add(new NEAT.Species());
                NEAT.Population.instance.species[NEAT.Population.instance.species.Count - 1].topFitness = float.Parse(firstParts[0]);
                NEAT.Population.instance.species[NEAT.Population.instance.species.Count - 1].staleness = int.Parse(firstParts[1]);

                string[] networkParts = speciesParts[x+1].Split('n');

                for (int i = 0; i < networkParts.GetLength(0); i++)
                {
                    NEAT.Genotype genotype = new NEAT.Genotype();

                    string network = networkParts[i];
                    string[] nparts = network.Split('#');

                    string verts = nparts[0];
                    string[] vparts = verts.Split(',');

                    for (int j = 0; j < vparts.GetLength(0) - 1; j += 2)
                    {
                        int index = int.Parse(vparts[j]);
                        NEAT.VertexInfo.EType type = (NEAT.VertexInfo.EType)Enum.Parse(typeof(NEAT.VertexInfo.EType), vparts[j + 1]);

                        genotype.AddVertex(type, index);
                    }

                    string edges = nparts[1];
                    string[] eparts = edges.Split(',');

                    for (int j = 0; j < eparts.GetLength(0) - 1; j += 5)
                    {
                        int source = int.Parse(eparts[j]);
                        int destination = int.Parse(eparts[j + 1]);
                        float weight = float.Parse(eparts[j + 2]);
                        bool enabled = bool.Parse(eparts[j + 3]);
                        int innovation = int.Parse(eparts[j + 4]);

                        genotype.AddEdge(source, destination, weight, enabled, innovation);
                    }

                    NEAT.Population.instance.species[NEAT.Population.instance.species.Count - 1].members.Add(genotype);
                    NEAT.Population.instance.genetics.Add(genotype);
                }
            }

            NEAT.Population.instance.InscribePopulation();
        }
    }
}
