using nl.FrankvHoof.MachineLearning.NeuralNetworks.Functions;
using nl.FrankvHoof.MachineLearning.NeuralNetworks.Network;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.QLearning
{
    public class Brain : MonoBehaviour
    {
        /// <summary>
        /// Object being monitored
        /// </summary>
        private Rigidbody ball;
        /// <summary>
        /// State for Object being monitored
        /// </summary>
        [SerializeField]
        private BallState ballState;
        /// <summary>
        /// Reward to associate with actions
        /// </summary>
        float reward = 0f;
        /// <summary>
        /// List of past actions and rewards
        /// </summary>
        List<Replay> replayMemory;
        /// <summary>
        /// Capacity for ReplayMemory
        /// </summary>
        int mCapacity = 10000;
        /// <summary>
        /// How much future states affect rewards
        /// </summary>
        float discount = 0.99f;
        /// <summary>
        /// Chance for picking a random action
        /// </summary>
        float exploreRate = 100f;
        /// <summary>
        /// Maximum allowed ExploreRate
        /// </summary>
        float maxExploreRate = 100f;
        /// <summary>
        /// Minimum allowed ExploreRate
        /// </summary>
        float minExploreRate = 0.01f;
        /// <summary>
        /// Decay-rate for ExploreRate
        /// </summary>
        float exploreDecay = 0.0001f;
        /// <summary>
        /// StartingPosition for Ball
        /// </summary>
        Vector3 ballStartPos;
        /// <summary>
        /// Amount of times ball has been dropped
        /// </summary>
        int failCOunt = 0;
        /// <summary>
        /// Speed at which platform can tilt
        /// </summary>
        float tiltSpeed = 0.5f;
        /// <summary>
        /// Timer for current balancing-attempt
        /// </summary>
        float timer = 0;
        /// <summary>
        /// Current Record-Time for balancing
        /// </summary>
        float recordTime = 0;
        /// <summary>
        /// Neural Network analyzing inputs
        /// </summary>
        private NeuralNetwork network;
        /// <summary>
        /// Learning-Rate
        /// </summary>
        [SerializeField]
        private float alpha = 0.2f;


        void Start()
        {
            network = new NeuralNetwork(3, 2, 1, 6, alpha, ActivationFunctions.TanH, ActivationFunctions.Sigmoid);
            if (replayMemory == null || replayMemory.Capacity != mCapacity)
                replayMemory = new List<Replay>(mCapacity);
            ball = ballState.GetComponent<Rigidbody>();
            ballStartPos = ball.transform.position;
            if (ball == null)
                Debug.LogError("No Rigidbody on Ball");
            Time.timeScale = 5f;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ResetBall();
        }

        private void FixedUpdate()
        {
            timer += Time.deltaTime;
            List<double> states = new List<double>();
            List<double> qs = new List<double>();

            states.Add(transform.rotation.x);
            states.Add(ball.transform.position.z);
            states.Add(ball.angularVelocity.x);

            qs = NormalizationFunctions.SoftMax(network.CalcOutput(states));
            double maxQ = qs.Max();
            int maxQIndex = qs.IndexOf(maxQ);
            exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

            //if(Random.Range(0,100) < exploreRate)
            //	maxQIndex = Random.Range(0,2);

            if (maxQIndex == 0)
                this.transform.Rotate(Vector3.right, tiltSpeed * (float)qs[maxQIndex]);
            else if (maxQIndex == 1)
                this.transform.Rotate(Vector3.right, -tiltSpeed * (float)qs[maxQIndex]);

            if (ball.GetComponent<BallState>().dropped)
                reward = -1.0f;
            else
                reward = 0.1f;

            Replay lastMemory = new Replay(transform.rotation.x,
                                    ball.transform.position.z,
                                    ball.angularVelocity.x,
                                    reward);

            if (replayMemory.Count == mCapacity)
                replayMemory.RemoveAt(0);

            replayMemory.Add(lastMemory);

            if (ballState.dropped)
            {
                for (int i = replayMemory.Count - 1; i >= 0; i--)
                {
                    List<double> toutputsOld = new List<double>();
                    List<double> toutputsNew = new List<double>();
                    toutputsOld = NormalizationFunctions.SoftMax(network.CalcOutput(replayMemory[i].states));

                    double maxQOld = toutputsOld.Max();
                    int action = toutputsOld.IndexOf(maxQOld);

                    double feedback;
                    if (i == replayMemory.Count - 1 || replayMemory[i].Reward == -1)
                        feedback = replayMemory[i].Reward;
                    else
                    {
                        toutputsNew = NormalizationFunctions.SoftMax(network.CalcOutput(replayMemory[i + 1].states));
                        maxQ = toutputsNew.Max();
                        feedback = replayMemory[i].Reward +
                            discount * maxQ;
                    }

                    toutputsOld[action] = feedback;
                    network.Train(replayMemory[i].states, toutputsOld);
                }

                if (timer > recordTime)
                {
                    recordTime = timer;
                }

                timer = 0;

                ballState.dropped = false;
                this.transform.rotation = Quaternion.identity;
                ResetBall();
                replayMemory.Clear();
                failCOunt++;
            }
        }

        private void ResetBall()
        {
            ball.transform.position = ballStartPos;
            ball.velocity = Vector3.zero;
            ball.angularVelocity = Vector3.zero;
        }



        #region Debug
        GUIStyle guiStyle = new GUIStyle();
        /// <summary>
        /// Debug-Display
        /// </summary>
        private void OnGUI()
        {
            guiStyle.fontSize = 25;
            guiStyle.normal.textColor = Color.white;
            GUI.BeginGroup(new Rect(10, 10, 600, 150));
            GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
            GUI.Label(new Rect(10, 25, 500, 30), "Fails: " + failCOunt, guiStyle);
            GUI.Label(new Rect(10, 50, 500, 30), "Explore Rate: " + exploreRate, guiStyle);
            GUI.Label(new Rect(10, 75, 500, 30), "Record-Time: " + recordTime, guiStyle);
            GUI.Label(new Rect(10, 100, 500, 30), "Time: " + timer, guiStyle);
            GUI.EndGroup();
        }
        #endregion
    }
}