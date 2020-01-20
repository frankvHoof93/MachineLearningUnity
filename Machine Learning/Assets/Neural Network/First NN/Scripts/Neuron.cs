using System.Collections.Generic;
using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.NeuralNetworks.FirstNN
{
    public class Neuron
    {
        #region Variables
        /// <summary>
        /// Number of Inputs for Neuron
        /// </summary>
        public readonly int NumInputs;
        /// <summary>
        /// Weights for Neuron
        /// </summary>
        public readonly List<double> Weights;
        /// <summary>
        /// Inputs for Neuron
        /// </summary>
        public readonly List<double> Inputs;
        /// <summary>
        /// Bias for Neuron
        /// </summary>
        public double Bias { get; internal set; }
        /// <summary>
        /// Output for Neuron
        /// </summary>
        public double Output { get; internal set; }
        /// <summary>
        /// Error-Gradient
        /// </summary>
        public double ErrorGradient { get; internal set; }
        #endregion

        #region Methods
        /// <summary>
        /// Constructor for a Neuron
        /// </summary>
        /// <param name="numInputs">Number of Inputs for Neuron</param>
        public Neuron(int numInputs)
        {
            Bias = Random.Range(-1f, 1f);
            NumInputs = numInputs;
            Weights = new List<double>(NumInputs);
            Inputs = new List<double>(NumInputs);
            for (int i = 0; i < numInputs; i++)
                Weights.Add(Random.Range(-1f, 1f));
        }
        #endregion
    }
}