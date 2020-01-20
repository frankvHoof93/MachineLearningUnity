using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using nl.FrankvHoof.MachineLearning.NeuralNetworks.Pong;
using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.PlayerInput
{
    public class ANNController : MonoBehaviour
    {
        #region InnerTypes
        public struct WallDistance
        {
            public float forward;
            public float right;
            public float left;
            public float right45;
            public float left45;
        }
        #endregion

        #region Variables
        public float translation;
        public float rotation;

        #region Editor
        [SerializeField]
        private float viewDistance = 200f;
        [SerializeField]
        private float speed = 50f;
        [SerializeField]
        private float rotationSpeed = 100f;
        [SerializeField]
        private int epochs = 1000;
        #endregion

        #region Private

        private NeuralNetwork network;

        private bool trainingComplete = false;

        private float trainingProgress = 0;
        private double sse = 0;
        private double lastSSE = 1f;
        #endregion
        #endregion


        #region Methods
        #region Unity
        private void OnGUI()
        {
            GUI.Label(new Rect(25, 25, 250, 30), "SSE: " + lastSSE.ToString("n8"));
            GUI.Label(new Rect(25, 40, 250, 30), "Alpha: " + network.Alpha);
            GUI.Label(new Rect(25, 50, 250, 30), "Trained: " + trainingProgress);
        }

        private void Start()
        {
            network = new NeuralNetwork(5, 2, 1, 10, 0.5);
            StartCoroutine(LoadTrainingSet());
        }

        private void Update()
        {
            if (!trainingComplete)
                return;
            List<double> inputs = new List<double>();

            WallDistance dist = RayCast();

            inputs.Add(dist.forward);
            inputs.Add(dist.right);
            inputs.Add(dist.left);
            inputs.Add(dist.right45);
            inputs.Add(dist.left45);

            List<double> calcOutputs = network.CalcOutput(inputs);

            float translationInput = Map(-1, 1, 0, 1, (float)calcOutputs[0]);
            float rotationInput = Map(-1, 1, 0, 1, (float)calcOutputs[1]);
            translation = translationInput * speed * Time.deltaTime;
            rotation = rotationInput * rotationSpeed * Time.deltaTime;
            transform.Translate(0, 0, translation);
            transform.Rotate(0, rotation, 0);
        }
        #endregion

        #region Private
        private IEnumerator LoadTrainingSet()
        {
            string path = Application.dataPath + "/PlayerInput/trainingdata.txt";
            if (File.Exists(path))
            {
                string line;
                // Don't do this in production. Write the length to line 1 instead
                int lineCount = File.ReadAllLines(path).Length;
                StreamReader td = File.OpenText(path);
                List<double> inputs = new List<double>();
                List<double> outputs = new List<double>();

                for (int i = 0; i < epochs; i++)
                {
                    sse = 0;
                    // Reset File Pointer
                    td.BaseStream.Position = 0;
                    string currWeights = network.PrintWeights();
                    while((line = td.ReadLine()) != null)
                    {
                        string[] data = line.Split(',');
                        float err = 0;
                        if (Convert.ToDouble(data[5], CultureInfo.InvariantCulture) != 0 && Convert.ToDouble(data[6], CultureInfo.InvariantCulture) != 0)
                        {
                            inputs.Clear();
                            outputs.Clear();
                            inputs.Add(Convert.ToDouble(data[0], CultureInfo.InvariantCulture));
                            inputs.Add(Convert.ToDouble(data[1], CultureInfo.InvariantCulture));
                            inputs.Add(Convert.ToDouble(data[2], CultureInfo.InvariantCulture));
                            inputs.Add(Convert.ToDouble(data[3], CultureInfo.InvariantCulture));
                            inputs.Add(Convert.ToDouble(data[4], CultureInfo.InvariantCulture));

                            double o1 = Map(0, 1, -1, 1, Convert.ToSingle(data[5], CultureInfo.InvariantCulture));
                            outputs.Add(o1);
                            double o2 = Map(0, 1, -1, 1, Convert.ToSingle(data[6]));
                            outputs.Add(o2);

                            List<double> outputsNetwork = network.Train(inputs, outputs);
                            err = (Mathf.Pow((float)(outputs[0] - outputsNetwork[0]), 2) +
                                Mathf.Pow((float)(outputs[1] - outputsNetwork[1]), 2)) / 2f;
                        }
                        sse += err;
                    }
                    trainingProgress = (float)i / (float)epochs;
                    sse /= (float)lineCount;
                    // If SSE isn't better, reload previous weights and decrease alpha
                    if (lastSSE < sse)
                    {
                        network.LoadWeights(currWeights);
                        network.Alpha = Mathf.Clamp((float)network.Alpha - 0.01f, 0.01f, 0.9f);
                    }
                    else // Increase Alpha
                    {
                        network.Alpha = Mathf.Clamp((float)network.Alpha + 0.01f, 0.01f, 0.9f);
                        lastSSE = sse;
                    }
                    yield return null;
                }
            }
            trainingComplete = true;
        }

        private float Map(float newFrom, float newTo, float oldFrom, float oldTo, float value)
        {
            if (value <= oldFrom)
                return newFrom;
            if (value >= oldTo)
                return newTo;
            return (newTo - newFrom) * ((value - oldFrom) / (oldTo - oldFrom)) + newFrom;
        }

        /// <summary>
        /// Rounds values between 0 and 1 to 0.5
        /// </summary>
        /// <param name="x">Input</param>
        /// <returns>Output after rounding</returns>
        private float RoundToHalf(float x)
        {
            return (float)Math.Round(x, MidpointRounding.AwayFromZero) / 2f;
        }

        /// <summary>
        /// Raycasts for Wall-Distances
        /// </summary>
        /// <returns>Distances retrieved from RayCasts</returns>
        private WallDistance RayCast()
        {
            // RayCast to Walls
            RaycastHit hit;
            WallDistance dist;
            // Fwd
            if (Physics.Raycast(transform.position, transform.forward, out hit, viewDistance))
                dist.forward = 1f - RoundToHalf(hit.distance / viewDistance);
            else
                dist.forward = 0f;
            // Right
            if (Physics.Raycast(transform.position, transform.right, out hit, viewDistance /5f))
                dist.right = 1f - RoundToHalf(hit.distance / viewDistance);
            else
                dist.right = 0f;
            // Left
            if (Physics.Raycast(transform.position, -transform.right, out hit, viewDistance / 5f))
                dist.left = 1f - RoundToHalf(hit.distance / viewDistance);
            else
                dist.left = 0f;
            // Right45
            if (Physics.Raycast(transform.position, (transform.forward + transform.right).normalized, out hit, viewDistance/ 4f))
                dist.right45 = 1f - RoundToHalf(hit.distance / viewDistance);
            else
                dist.right45 = 0f;
            // Left45
            if (Physics.Raycast(transform.position, (transform.forward - transform.right).normalized, out hit, viewDistance / 4f))
                dist.left45 = 1f - RoundToHalf(hit.distance / viewDistance);
            else
                dist.left45 = 0f;
            return dist;
        }
        #endregion
        #endregion
    }
}