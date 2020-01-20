using System.Collections.Generic;
using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.NeuralNetworks.Network
{
    public class NeuralNetwork
    {
        #region InnerClasses
        /// <summary>
        /// Delegate for Activation-Function for a Neuron
        /// </summary>
        /// <param name="value">Input-Value for Function</param>
        /// <returns>Output-Value from Function</returns>
        public delegate double ActivationDelegate(double value);
        #endregion

        #region Variables
        #region Public
        /// <summary>
        /// Number of Inputs for Network
        /// </summary>
        public readonly int NumInputs;
        /// <summary>
        /// Number of Outputs for Network
        /// </summary>
        public readonly int NumOutputs;
        /// <summary>
        /// Number of Hidden Layers
        /// </summary>
        public readonly int NumHiddenLayers;
        /// <summary>
        /// Number of Neurons per Hidden Layer
        /// </summary>
        public readonly int NumNeuronsPerHiddenLayer;
        /// <summary>
        /// Training-Alpha (amount of change per training set)
        /// </summary>
        public double Alpha;
        /// <summary>
        /// Layers in Network
        /// </summary>
        public readonly List<Layer> Layers;
        #endregion

        #region Private
        /// <summary>
        /// ActivationFunction for Neurons
        /// </summary>
        private ActivationDelegate activation;
        /// <summary>
        /// ActivationFunction for Neurons in Output-Layer
        /// </summary>
        private ActivationDelegate activationOutput;
        #endregion
        #endregion

        #region Methods
        #region Constructors
        /// <summary>
        /// Constructor for Neural Network
        /// </summary>
        /// <param name="numIn">Number of Inputs for Network</param>
        /// <param name="numOut">Number of Outputs for Network</param>
        /// <param name="numHidden">Number of Hidden Layers</param>
        /// <param name="numPerHidden">Number of Neurons per Hidden Layer</param>
        /// <param name="alpha">Training-Alpha</param>
        /// <param name="activationFunction">Activation-Function for Neurons</param>
        /// /// <param name="activationFunctionOutput">Activation-Function for Neurons in Output-Layer</param>
        public NeuralNetwork(int numIn, int numOut, int numHidden, int numPerHidden, double alpha, ActivationDelegate activationFunction, ActivationDelegate activationFunctionOutput)
        {
            NumInputs = numIn;
            NumOutputs = numOut;
            NumHiddenLayers = numHidden;
            NumNeuronsPerHiddenLayer = numPerHidden;
            Alpha = alpha;
            activation = activationFunction;
            activationOutput = activationFunctionOutput;
            Layers = new List<Layer>(numHidden + 1);
            if (numHidden > 0)
            {
                // Input-Layer
                Layers.Add(new Layer(NumNeuronsPerHiddenLayer, NumInputs));
                // Hidden Layers
                for (int i = 0; i < numHidden - 1; i++)
                    Layers.Add(new Layer(NumNeuronsPerHiddenLayer, NumNeuronsPerHiddenLayer));
                // Output-Layer
                Layers.Add(new Layer(NumOutputs, NumNeuronsPerHiddenLayer));
            }
            else // Single Layer
                Layers.Add(new Layer(NumOutputs, NumInputs));
        }
        #endregion

        #region Public
        #region Save&Load
        /// <summary>
        /// Loads in Weights from a String
        /// (Weights split by ',')
        /// </summary>
        /// <param name="weightStr">String to load Weights from</param>
        public void LoadWeights(string weightStr)
        {
            if (weightStr == "") return;
            string[] weightValues = weightStr.Split(',');
            int w = 0;
            foreach (Layer l in Layers)
                foreach (Neuron n in l.Neurons)
                    for (int i = 0; i < n.Weights.Count; i++)
                    {
                        n.Weights[i] = System.Convert.ToDouble(weightValues[w]);
                        w++;
                    }
        }
        /// <summary>
        /// Prints current Weights to a String, separated by ','
        /// </summary>
        /// <returns>Current Weights in String-form</returns>
        public string PrintWeights()
        {
            string weightStr = "";
            foreach (Layer l in Layers)
                foreach (Neuron n in l.Neurons)
                    foreach (double w in n.Weights)
                        weightStr += w + ",";
            return weightStr;
        }
        #endregion

        #region Train&Calc
        /// <summary>
        /// Trains Network using Training-Input and -Output
        /// </summary>
        /// <param name="trainingInput">Input for Training</param>
        /// <param name="desiredOutput">Output for Training</param>
        /// <returns>Actual Outputs from Training-Input</returns>
        public List<double> Train(List<double> trainingInput, List<double> desiredOutput)
        {
            List<double> outputValues = new List<double>(desiredOutput.Count);
            outputValues = CalcOutput(trainingInput);
            UpdateWeights(outputValues, desiredOutput);
            return outputValues;
        }
        /// <summary>
        /// Calculates output based on Input-Values & Network-State
        /// </summary>
        /// <param name="inputValues">Input-Values for Network</param>
        /// <returns>Output-Values from Network</returns>
        public List<double> CalcOutput(List<double> inputValues)
        {
            if (inputValues.Count != NumInputs)
            {
                Debug.LogError("ERROR: Number of Inputs must be " + NumInputs);
                return new List<double>();
            }
            int currentInput = 0;
            List<double> inputs = new List<double>(inputValues);
            List<double> outputs = new List<double>();
            // Loop through Layers
            for (int i = 0; i < NumHiddenLayers + 1; i++)
            {
                // Set Inputs to Output of previous layer
                if (i > 0)
                    inputs = new List<double>(outputs);
                outputs.Clear();
                Layer layer = Layers[i];
                // Loop through Neurons in Layer
                for (int j = 0; j < layer.NumNeurons; j++)
                {
                    double N = 0;
                    Neuron neuron = layer.Neurons[j];
                    neuron.Inputs.Clear();
                    // Loop through Inputs for Neuron
                    for (int k = 0; k < neuron.NumInputs; k++)
                    {
                        neuron.Inputs.Add(inputs[currentInput]);
                        N += neuron.Weights[k] * inputs[currentInput];
                        currentInput++;
                    }
                    N -= neuron.Bias;
                    if (i == NumHiddenLayers)
                        neuron.Output = ActivationFunctionOutput(N);
                    else
                        neuron.Output = ActivationFunction(N);
                    if (double.IsNaN(neuron.Output))
                    {
                        Debug.LogError("OUTPUT IS NAN");
                    }
                    outputs.Add(neuron.Output);
                    currentInput = 0;
                }
            }
            return outputs;
        }
        #endregion
        #endregion

        #region Private
        /// <summary>
        /// Updates Weights (Trains Network) based on Expected & Received Outputs using BackPropagation
        /// </summary>
        /// <param name="outputs">Received Outputs</param>
        /// <param name="desiredOutputs">Expected Outputs</param>
        private void UpdateWeights(List<double> outputs, List<double> desiredOutputs)
        {
            double error;
            // Loop through Layers (Back to Front)
            for (int i = NumHiddenLayers; i >= 0; i--)
            {
                Layer layer = Layers[i];
                // Loop through Neurons in Layer
                for (int j = 0; j < layer.NumNeurons; j++)
                {
                    // Find Error-Gradient
                    Neuron neuron = layer.Neurons[j];
                    if (i == NumHiddenLayers) // Output-Layer 
                    {
                        error = desiredOutputs[j] - outputs[j];
                        neuron.ErrorGradient = outputs[j] * (1 - outputs[j]) * error;
                    }
                    else // Other Layers
                    {
                        neuron.ErrorGradient = neuron.Output * (1 - neuron.Output);
                        // Sum of (Weighted) Errors in next Layer
                        double errorGradSum = 0;
                        Layer nextLayer = Layers[i + 1];
                        for (int n = 0; n < nextLayer.NumNeurons; n++)
                            errorGradSum += nextLayer.Neurons[n].ErrorGradient * nextLayer.Neurons[n].Weights[j];
                        neuron.ErrorGradient *= errorGradSum;
                    }
                    // Update Weights for each Input
                    for (int k = 0; k < neuron.NumInputs; k++)
                    {
                        if (i == NumHiddenLayers)
                        {
                            error = desiredOutputs[j] - outputs[j];
                            neuron.Weights[k] += Alpha * neuron.Inputs[k] * error;
                        }
                        else
                            neuron.Weights[k] += Alpha * neuron.Inputs[k] * neuron.ErrorGradient;
                    }
                    // Update Bias for Neuron
                    neuron.Bias -= Alpha * neuron.ErrorGradient;
                }
            }
        }

        #region Activation-Functions
        /// <summary>
        /// Activation-Function for Neuron
        /// </summary>
        /// <param name="value">Input for Neuron</param>
        /// <returns>Output from Neuron</returns>
        private double ActivationFunction(double value)
        {
            return activation(value);
        }
        /// <summary>
        /// Activation-Function for Neurons in Output-Layer
        /// </summary>
        /// <param name="value">Input for Neuron</param>
        /// <returns>Output from Neuron</returns>
        private double ActivationFunctionOutput(double value)
        {
            return activationOutput(value);
        }
        #endregion
        #endregion
        #endregion
    }
}