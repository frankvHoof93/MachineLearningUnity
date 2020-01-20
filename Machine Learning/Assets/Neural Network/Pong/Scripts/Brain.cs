using System.Collections.Generic;
using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.NeuralNetworks.Pong
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Brain : MonoBehaviour
    {
        #region Variables
        #region Constants
        /// <summary>
        /// Min Y-Pos for Paddle
        /// </summary>
        private const float paddleMinY = 8.8f;
        /// <summary>
        /// Max Y-Pos for Paddle
        /// </summary>
        private const float paddleMaxY = 17.4f;
        /// <summary>
        /// Max Y-Speed for Paddle
        /// </summary>
        private const float paddleMaxSpeed = 15f;
        #endregion

        #region Editor
        /// <summary>
        /// GameObject for Ball
        /// </summary>
        [SerializeField]
        private GameObject ball;
        /// <summary>
        /// Layer for RayCasting to backwall from Ball
        /// </summary>
        [SerializeField]
        private LayerMask raycastLayer;
        /// <summary>
        /// Number of Balls Saved
        /// </summary>
        [SerializeField]
        private int numSaved = 0;
        /// <summary>
        /// Number of Balls Missed
        /// </summary>
        [SerializeField]
        private int numMissed = 0;
        /// <summary>
        /// Learning-Rate for Neural Network
        /// </summary>
        [SerializeField]
        private double learningRate = 0.11d;
        #endregion

        #region Private
        /// <summary>
        /// RigidBody for Ball
        /// </summary>
        private Rigidbody2D ballRB;
        /// <summary>
        /// Current Y-Velocity of Paddle
        /// </summary>
        private float yVel;
        /// <summary>
        /// Neural Network for Brain
        /// </summary>
        private NeuralNetwork nn;
        #endregion
        #endregion

        #region Methods
        #region Unity
        /// <summary>
        /// Initializes Brain
        /// </summary>
        private void Start()
        {
            nn = new NeuralNetwork(6, 1, 1, 4, learningRate);
            ballRB = ball.GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Moves Paddle
        /// </summary>
        private void Update()
        {
            // Calculate Y-Pos after Movement
            float posY = Mathf.Clamp(transform.position.y + (yVel * Time.deltaTime * paddleMaxSpeed), paddleMinY, paddleMaxY);

            if (float.IsNaN(posY))
                Debug.LogError($"FOUND NAN WITH INPUTS: {transform.position.y} - {yVel}   =  {transform.position.y + (yVel * Time.deltaTime * paddleMaxSpeed)}");


            // Move Paddle
            transform.position = new Vector3(transform.position.x, posY, transform.position.z);

            // Calculate Y-Velocity for next frame
            if (ballRB.velocity.x < 0) // Headed in other direction
            {
                yVel = 0;
                return;
            }
            // Check where ball is headed
            RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, ballRB.velocity, 1000f, raycastLayer);

            if (hit.collider != null)
            {
                // Reflect off top/bottom
                if (hit.collider.gameObject.tag == "tops")
                {
                    Vector3 reflectionAngle = Vector3.Reflect(ballRB.velocity, hit.normal);
                    hit = Physics2D.Raycast(ball.transform.position, reflectionAngle, 1000f, raycastLayer);
                }
                // Hit BackWall
                if (hit.collider != null && hit.collider.gameObject.tag == "backwall")
                {
                    // Distance for Paddle to Move
                    double dy = hit.point.y - transform.position.y;
                    List<double> output = Run(ball.transform.position.x, ball.transform.position.y,
                                ballRB.velocity.x, ballRB.velocity.y,
                                transform.position.x, transform.position.y,
                                dy);
                    yVel = (float)output[0];

                    if (float.IsNaN(yVel))
                        Debug.LogError($"FOUND NAN WITH INPUTS: {transform.position.y} - {yVel}   =  {transform.position.y + (yVel * Time.deltaTime * paddleMaxSpeed)}");
                }
            }
            else yVel = 0;
        }
        #endregion

        #region Private
        /// <summary>
        /// Runs Inputs through Neural Network
        /// </summary>
        /// <param name="ballX">X-Pos for Ball</param>
        /// <param name="ballY">Y-Pos for Ball</param>
        /// <param name="ballVelX">X-Velocity for Ball</param>
        /// <param name="ballVelY">Y-Velocity for Ball</param>
        /// <param name="padX">X-Pos for Paddle</param>
        /// <param name="padY">Y-Pos for Paddle</param>
        /// <param name="padVel">Velocity for Paddle. Set to NaN when not training</param>
        /// <returns>Outputs from Neural Network</returns>
        private List<double> Run(double ballX, double ballY, double ballVelX, double ballVelY, double padX, double padY, double padVel = double.NaN)
        {
            List<double> inputs = new List<double>(6)
            {
                ballX,
                ballY,
                ballVelX,
                ballVelY,
                padX,
                padY
            };
            if (!double.IsNaN(padVel))
            {
                List<double> outputs = new List<double>(1)
                {
                    padVel
                };
                return nn.Train(inputs, outputs);
            }
            else return nn.CalcOutput(inputs);
        }
        #endregion
        #endregion
    }
}