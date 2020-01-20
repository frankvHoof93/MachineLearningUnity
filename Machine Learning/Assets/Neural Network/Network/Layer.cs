using System.Collections.Generic;

namespace nl.FrankvHoof.MachineLearning.NeuralNetworks.Network
{
    public class Layer
    {
        #region Variables
        /// <summary>
        /// Number of Neurons in Layer
        /// </summary>
        public readonly int NumNeurons;
        /// <summary>
        /// Neurons in Layer
        /// </summary>
        public readonly List<Neuron> Neurons;
        #endregion

        #region Methods
        /// <summary>
        /// Constructor for a Layer
        /// </summary>
        /// <param name="numNeurons">Number of Neurons in Layer</param>
        /// <param name="numNeuronInputs">Number of Inputs for each Neuron</param>
        public Layer(int numNeurons, int numNeuronInputs)
        {
            NumNeurons = numNeurons;
            Neurons = new List<Neuron>(numNeurons);
            for (int i = 0; i < numNeurons; i++)
                Neurons.Add(new Neuron(numNeuronInputs));
        }
        #endregion
    }
}