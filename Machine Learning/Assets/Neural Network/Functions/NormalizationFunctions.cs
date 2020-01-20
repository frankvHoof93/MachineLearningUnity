using System;
using System.Collections.Generic;
using System.Linq;

namespace nl.FrankvHoof.MachineLearning.NeuralNetworks.Functions
{
    public static class NormalizationFunctions
    {
        /// <summary>
        /// Takes a List of values, and modifies it such that each component is in te interval (0,1), and all components add up to to 1
        /// </summary>
        /// <param name="input">Input for Function</param>
        /// <returns>List of values in interval (0,1) which add up to 1</returns>
        public static List<double> SoftMax(List<double> input)
        {
            double max = input.Max();
            double scale = 0f;
            for (int i = 0; i < input.Count; i++)
                scale += Math.Exp(input[i] - max);

            List<double> output = new List<double>();
            for (int i = 0; i < input.Count; i++)
                output.Add(Math.Exp(input[i] - max) / scale);
            return output;
        }
    }
}
