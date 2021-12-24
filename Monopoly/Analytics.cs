using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly
{
    public class Analytics
    {
        public static Analytics instance = null;

        public int[] bids;
        public int[] money;
        public float[] average;
        public float[] price;

        public int[] trades;
        public int[] exchanges;

        public int[] wins;
        public int max = 0;
        public int min = 0;
        public float[] ratio;

        public Analytics()
        {
            bids = new int[40];
            money = new int[40];
            average = new float[40];
            price = new float[40];

            trades = new int[40];
            exchanges = new int[40];

            wins = new int[40];
            ratio = new float[40];
        }

        public void MakeBid(int index, int bid)
        {
            bids[index]++;
            money[index] += bid;
            average[index] = money[index] / bids[index];
            price[index] = average[index] / MONOPOLY.Board.COSTS[index];
        }

        public void MadeTrade(int index)
        {
            trades[index]++;
        }

        public void MarkWin(int index)
        {
            wins[index]++;

            if (max < wins[index])
            {
                max = wins[index];
            }

            int tempMin = int.MaxValue;

            for (int i = 0; i < 40; i++)
            {
                if (wins[i] != 0 && wins[i] < tempMin)
                {
                    tempMin = wins[i];
                }
            }

            ratio[index] = (float)(wins[index] - tempMin) / (float)(max - tempMin);
        }

    }
}
