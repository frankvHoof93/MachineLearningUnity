using System.Collections.Generic;
using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.NeuralNetworks.FirstNN
{
    public class Brain : MonoBehaviour
    {
        #region Variables
        #region Editor
        /// <summary>
        /// Amount of Epochs to train for
        /// </summary>
        [SerializeField]
        private int trainingEpochs = 1000;
        /// <summary>
        /// LearningRate during training
        /// </summary>
        [SerializeField]
        private double learningRate = 0.8;
        #endregion

        #region Private
        /// <summary>
        /// Neural Network controlling this brain
        /// </summary>
        private NeuralNetwork network;
        #endregion
        #endregion

        #region Methods
        #region Unity
        /// <summary>
        /// Trains Network
        /// </summary>
        private void Start()
        {
            double sumSquareErrors = 0;
            network = new NeuralNetwork(2, 1, 1, 2, learningRate);
            List<double> result;

            for (int i = 0; i < trainingEpochs; i++)
            {
                sumSquareErrors = 0;
                result = Train(1, 1, 0);
                // (Result - DesiredResult)^2
                sumSquareErrors += Mathf.Pow((float)result[0] - 0, 2);
                result = Train(1, 0, 1);
                sumSquareErrors += Mathf.Pow((float)result[0] - 1, 2);
                result = Train(0, 1, 1);
                sumSquareErrors += Mathf.Pow((float)result[0] - 1, 2);
                result = Train(0, 0, 0);
                sumSquareErrors += Mathf.Pow((float)result[0] - 0, 2);
            }
            Debug.Log("Sum Squared Errors: " + sumSquareErrors);

            result = Train(1, 1, 0);
            Debug.Log(string.Format(" 1 1 [0] - [{0}] {1}", Mathf.RoundToInt((float)result[0]), result[0]));
            result = Train(1, 0, 1);
            Debug.Log(string.Format(" 1 0 [1] - [{0}] {1}", Mathf.RoundToInt((float)result[0]), result[0]));
            result = Train(0, 1, 1);
            Debug.Log(string.Format(" 0 1 [1] - [{0}] {1}", Mathf.RoundToInt((float)result[0]), result[0]));
            result = Train(0, 0, 0);
            Debug.Log(string.Format(" 0 0 [0] - [{0}] {1}", Mathf.RoundToInt((float)result[0]), result[0]));
        }
        #endregion

        #region Private
        /// <summary>
        /// Performs a single Training Operation on the Neural Network
        /// </summary>
        /// <param name="in1">Input 1</param>
        /// <param name="in2">Input 2</param>
        /// <param name="o">Desired Output</param>
        /// <returns>Actual Outputs</returns>
        private List<double> Train(double in1, double in2, double o)
        {
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();
            inputs.Add(in1);
            inputs.Add(in2);
            outputs.Add(o);
            return network.Train(inputs, outputs);
        }
        #endregion
        #endregion
    }
}