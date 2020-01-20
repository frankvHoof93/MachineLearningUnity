using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.Perceptron
{
    [System.Serializable]
    public class TrainingSet
    {
        /// <summary>
        /// Training-Inputs
        /// </summary>
        public double[] input;
        /// <summary>
        /// Training-Output
        /// </summary>
        public double output;
    }

    public class Perceptron : MonoBehaviour
    {
        #region Variables
        #region Editor
        /// <summary>
        /// Training-Set for Perceptron
        /// </summary>
        [SerializeField]
        private List<TrainingSet> trainingSet = new List<TrainingSet>();
        [SerializeField]
        private Animator npcAnim;
        [SerializeField]
        private Rigidbody npcRigid;
        /// <summary>
        /// Grapher to draw to
        /// </summary>
        [SerializeField]
        private SimpleGrapher grapher;
        #endregion

        #region Private
        /// <summary>
        /// Input-Weights
        /// </summary>
        private double[] weights = { 0, 0 };
        /// <summary>
        /// Perceptron-Bias
        /// </summary>
        private double bias = 0;
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Sends Input to Perceptron, for live training
        /// </summary>
        /// <param name="i1">Input 1</param>
        /// <param name="i2">Input 2</param>
        /// <param name="o">Output</param>
        public void SendInput(double i1, double i2, double o)
        {
            // Read Perceptron
            double result = CalcOutput(i1, i2);
            Debug.Log(result);
            if (result == 0) // Duck
            {
                npcAnim.SetTrigger("Crouch");
                npcRigid.isKinematic = false;
            }
            else
                npcRigid.isKinematic = true;

            // Add to TrainingSets
            TrainingSet s = new TrainingSet
            {
                input = new double[] { i1, i2 },
                output = o
            };
            trainingSet.Add(s);
            // Train
            Train(1);
        }
        /// <summary>
        /// Loads Weights from File
        /// </summary>
        public void LoadWeights()
        {
            string path = Application.dataPath + "/weights.txt";
            if (File.Exists(path))
            {
                StreamReader sr = File.OpenText(path);
                string line = sr.ReadLine();
                string[] w = line.Split(',');
                weights[0] = System.Convert.ToDouble(w[0]);
                weights[1] = System.Convert.ToDouble(w[1]);
                bias = System.Convert.ToDouble(w[2]);
                Debug.Log("Load Complete");
            }
        }
        /// <summary>
        /// Saves Weights to File
        /// </summary>
        public void SaveWeights()
        {
            string path = Application.dataPath + "/weights.txt";
            StreamWriter sw = File.CreateText(path);
            sw.WriteLine(weights[0] + "," + weights[1] + "," + bias);
            sw.Close();
        }
        #endregion

        #region Unity
        /// <summary>
        /// Runs Training-Session
        /// </summary>
        private void Start()
        {
            InitialiseWeights();
        }
        /// <summary>
        /// Clears TrainingSets
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown("space"))
            {
                InitialiseWeights();
                trainingSet.Clear();
            }
            else if (Input.GetKeyDown("s"))
                SaveWeights();
            else if (Input.GetKeyDown("l"))
                LoadWeights();
        }
        #endregion

        #region Private
        /// <summary>
        /// Trains Perceptron
        /// </summary>
        /// <param name="epochs">Amount of Epochs to train for</param>
        private void Train(int epochs)
        {
            for (int t = 0; t < trainingSet.Count; t++)
                UpdateWeights(t);
        }
        /// <summary>
        /// Calculates output based on weights 
        /// </summary>
        /// <param name="v1">Weights</param>
        /// <param name="v2">Inputs</param>
        /// <returns>Calculated Output</returns>
        private double DotProductBias(double[] v1, double[] v2)
        {
            if (v1 == null || v2 == null)
                return -1;
            if (v1.Length != v2.Length)
                return -1;
            double d = 0;
            for (int x = 0; x < v1.Length; x++)
                d += v1[x] * v2[x];
            return d + bias;
        }
        /// <summary>
        /// Calculates Output for Perceptron
        /// </summary>
        /// <param name="i">Index in TrainingSet</param>
        /// <returns>Output from Perceptron</returns>
        private double CalcOutput(int i)
        {
            double dp = DotProductBias(weights, trainingSet[i].input);
            if (dp > 0)
                return (1);
            return (0);
        }
        /// <summary>
        /// Initializes Weights & Bias to random values
        /// </summary>
        private void InitialiseWeights()
        {
            for (int i = 0; i < weights.Length; i++)
                weights[i] = Random.Range(-1.0f, 1.0f);
            bias = Random.Range(-1.0f, 1.0f);
        }
        /// <summary>
        /// Updates Weightd=s based on training input and output
        /// </summary>
        /// <param name="j">Index in TrainingSet</param>
        /// <returns>Total Error for Index</returns>
        private double UpdateWeights(int j)
        {
            double error = trainingSet[j].output - CalcOutput(j);
            for (int i = 0; i < weights.Length; i++)
                weights[i] = weights[i] + error * trainingSet[j].input[i];
            bias += error;
            return Mathf.Abs((float)error);
        }
        /// <summary>
        /// Calculates Output for Perceptron
        /// </summary>
        /// <param name="i1">First Input for Perceptron</param>
        /// <param name="i2">Second Input for Perceptron</param>
        /// <returns>Output from Perceptron</returns>
        private double CalcOutput(double i1, double i2)
        {
            double[] inp = new double[] { i1, i2 };
            double dp = DotProductBias(weights, inp);
            if (dp > 0)
                return (1);
            return (0);
        }
        /// <summary>
        /// Draws Graph-Points for TrainingSet
        /// </summary>
        private void DrawAllPoints()
        {
            for (int i = 0; i < trainingSet.Count; i++)
            {
                Color c = trainingSet[i].output == 0 ? Color.magenta : Color.green;
                grapher.DrawPoint((float)trainingSet[i].input[0], (float)trainingSet[i].input[1], c);
            }
        }
        /// <summary>
        /// Draws Decision-Boundary
        /// </summary>
        private void DrawLine()
        {
            // dY/dX
            float slope = (float)((-(bias / weights[1])) / (bias / weights[0]));
            // y = 0
            float intercept = (float)(-(bias / weights[1]));
            grapher.DrawRay(slope, intercept, Color.red);
        }
        #endregion
        #endregion
    }
}