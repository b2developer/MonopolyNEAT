using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    class OldProgram
    {
       //ic void OLD_MAIN()
       //
       //NetworkFactory.Initialise();
       //
       //Mutation.Initialise();
       //Crossover.Initialise();
       //Population.Initialise();
       //
       //Genotype buggy = NetworkFactory.instance.CreateBuggyNetwork();
       //
       //Phenotype buggy_p = new Phenotype();
       //buggy_p.InscribeGenotype(buggy);
       //buggy_p.ProcessGraph();
       //
       //buggy_p.Propagate(new float[2] { 1.0f, 1.0f });
       //
       //int POPULATION_SIZE = 100;
       //int INPUTS = 2;
       //int OUTPUTS = 1;
       //
       //int ATTEMPTS = 10;
       //
       //float[] X = new float[8] { 0, 0, 0, 1, 1, 0, 1, 1 };
       //float[] Y = new float[4] { 0, 1, 1, 0 };
       //
       //Population.instance.GenerateBasePopulation(POPULATION_SIZE, INPUTS, OUTPUTS);
       //
       //for (int g = 0; g < 50000; g++)
       //{
       //    for (int i = 0; i < POPULATION_SIZE; i++)
       //    {
       //        float totalError = 0.0f;
       //
       //        for (int tries = 10; tries >= 0; tries--)
       //        {
       //            List<int> selections = new List<int> { 0, 1, 2, 3 };
       //
       //            for (int x = 0; x < 4; x++)
       //            {
       //                int index = Mutation.instance.rng.Next(0, selections.Count);
       //                int select = selections[index];
       //                selections.RemoveAt(index);
       //
       //                float[] Yh = Population.instance.population[i].Propagate(new float[2] { X[select * 2], X[select * 2 + 1] });
       //
       //                float error = Math.Abs(Y[select] - Yh[0]);
       //                error *= error;
       //
       //                totalError += error;
       //            }
       //        }
       //
       //        float fitness = (ATTEMPTS * 4 - totalError);
       //
       //        if (fitness > ATTEMPTS * 4 - 0.01f)
       //        {
       //            int breakpoints = 0;
       //
       //            Population.instance.population[i].ResetGraph();
       //
       //            for (int a = ATTEMPTS; a >= 0; a--)
       //            {
       //                List<int> selections = new List<int> { 0, 1, 2, 3 };
       //
       //                for (int x = 0; x < 4; x++)
       //                {
       //                    int index = Mutation.instance.rng.Next(0, selections.Count);
       //                    int select = selections[index];
       //                    selections.RemoveAt(index);
       //
       //                    float[] Yh = Population.instance.population[i].Propagate(new float[2] { X[select * 2], X[select * 2 + 1] });
       //
       //                    float error = Math.Abs(Y[select] - Yh[0]);
       //                    error *= error;
       //
       //                    totalError += error;
       //                }
       //            }
       //        }
       //
       //        Console.WriteLine(fitness);
       //
       //        Population.instance.genetics[i].fitness = fitness;
       //    }
       //
       //    Population.instance.NewGeneration();
       //}
        //}
    }
}
