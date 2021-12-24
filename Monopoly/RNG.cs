using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class RNG
{
    public static RNG instance;
    public Random gen;

    public RNG()
    {
        gen = new Random();
    }

    public static void Initialise()
    {
        if (instance == null)
        {
            instance = new RNG();
        }
    }

    public List<MONOPOLY.Board.CardEntry> Shuffle(List<MONOPOLY.Board.CardEntry> cards)
    {
        List<MONOPOLY.Board.CardEntry> shuffle = new List<MONOPOLY.Board.CardEntry>();

        for (int i = 0; i < cards.Count;)
        {
            int r = gen.Next(0, cards.Count);
            shuffle.Add(cards[r]);
            cards.RemoveAt(r);
        }

        return shuffle;
    }

    public MONOPOLY.NeuralPlayer[] Shuffle(MONOPOLY.NeuralPlayer[] list)
    {
        List<MONOPOLY.NeuralPlayer> container = new List<MONOPOLY.NeuralPlayer>(list);
        List<MONOPOLY.NeuralPlayer> shuffle = new List<MONOPOLY.NeuralPlayer>();

        for (int i = 0; i < container.Count;)
        {
            int r = gen.Next(0, container.Count);
            shuffle.Add(container[r]);
            container.RemoveAt(r);
        }

        return shuffle.ToArray();
    }

    public List<NEAT.Phenotype> Shuffle(List<NEAT.Phenotype> list)
    {
        List<NEAT.Phenotype> shuffle = new List<NEAT.Phenotype>();

        for (int i = 0; i < list.Count;)
        {
            int r = gen.Next(0, list.Count);
            shuffle.Add(list[r]);
            list.RemoveAt(r);
        }

        return shuffle;
    }

    public void DoubleShuffle(List<NEAT.Phenotype> phen, List<NEAT.Genotype> gene, ref List<NEAT.Phenotype> op, ref List<NEAT.Genotype> og)
    {
        for (int i = 0; i < phen.Count;)
        {
            int r = gen.Next(0, phen.Count);
            op.Add(phen[r]);
            og.Add(gene[r]);
            phen.RemoveAt(r);
            gene.RemoveAt(r);
        }
    }
}