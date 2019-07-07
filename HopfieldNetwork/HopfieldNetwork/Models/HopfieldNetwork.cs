using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopfieldNetwork.Models
{
    public class HopfieldNetwork
    {
        private double[,] weights;
        private int inputLength;

        List<TrainingSet> trainingSets;
        public HopfieldNetwork()
        {
            trainingSets = new List<TrainingSet>();
        }

        public bool AddSet(TrainingSet set)
        {
            if (inputLength == 0)
                inputLength = set.Inputs.Length;
            if (inputLength == set.Inputs.Length)
            {
                trainingSets.Add(set);
                return true;
            }
            return false;
        }

        public void RemoveSet(TrainingSet set)
        {
            trainingSets.Remove(set);
        }

        public void LearnNetwork()
        {
            weights = new double[inputLength, inputLength];

            for(int i = 0; i < inputLength; i++)
            {
                for(int j = 0; j < inputLength; j++)
                {
                    if (i == j)
                        continue;
                    weights[i, j] = trainingSets.Sum(ts => ts.Inputs[i] * ts.Inputs[j]);                  
                }
            }
        }

        public TrainingSet GetOutput(double[] input)
        {
            double[] yt = (double[])input.Clone(),
                yt1 = (double[])input.Clone();

            do
            {
                yt = (double[])yt1.Clone();
                for (int i = 0; i < inputLength; i++)
                {
                    yt1[i] = 0;
                    for (int j = 0; j < inputLength; j++)
                    {
                        yt1[i] += weights[i, j] * yt[j];
                    }
                    yt1[i] = yt1[i] > 0 ? 1 : -1;
                }
            } while (!yt.SequenceEqual(yt1));

            return trainingSets.FirstOrDefault(t => t.Inputs.SequenceEqual(yt));
        }
    }
}
