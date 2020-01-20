using System.Collections.Generic;

namespace nl.FrankvHoof.MachineLearning.QLearning
{
    public struct Replay
    {
        /// <summary>
        /// State for Replay
        /// <para>
        /// [ X-Rotation Paddle, Ball-Position X, Ball-Velocity X ]
        /// </para>
        /// </summary>
        public readonly double[] State;
        /// <summary>
        /// Reward for Replay
        /// </summary>
        public readonly double Reward;

        public List<double> states => new List<double>(State);


        /// <summary>
        /// Creates a Replay to store in Memory
        /// </summary>
        /// <param name="xRotation">X-Rotation Paddle</param>
        /// <param name="ballX">Ball-Position X</param>
        /// <param name="ballVelX">Ball-Velocity X</param>
        /// <param name="reward">Reward for Replay</param>
        public Replay(double xRotation, double ballX, double ballVelX, double reward)
        {
            State = new double[]
            {
            xRotation,
            ballX,
            ballVelX
            };
            Reward = reward;
        }
    }
}