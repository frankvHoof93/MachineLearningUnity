using UnityEngine;
using UnityEngine.UI;

namespace nl.FrankvHoof.MachineLearning.NeuralNetworks.Pong
{
    public class PongUIManager : MonoBehaviour
    {
        #region Variables
        #region Static
        /// <summary>
        /// Singleton-Instance
        /// </summary>
        public static PongUIManager Instance { get; private set; }
        #endregion
        #region Editor
        /// <summary>
        /// Score-Text Player 1
        /// </summary>
        [SerializeField]
        private Text p1ScoreText;
        /// <summary>
        /// Score-Text Player 2
        /// </summary>
        [SerializeField]
        private Text p2ScoreText;
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Add Score to a Player
        /// </summary>
        /// <param name="player1">True for P1, false for P2</param>
        public void AddScore(bool player1)
        {
            Text scoreText = player1 ? p1ScoreText : p2ScoreText;
            int currScore = int.Parse(scoreText.text);
            currScore += 1;
            scoreText.text = currScore.ToString();
        }
        /// <summary>
        /// Resets Score for both players
        /// </summary>
        public void ResetScore()
        {
            p1ScoreText.text = "0";
            p2ScoreText.text = "0";
        }
        #endregion

        #region Unity
        /// <summary>
        /// Initializes Singleton-Instance
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            ResetScore();
        }
        /// <summary>
        /// Resets Score on Input of 'R'
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                ResetScore();
        }
        /// <summary>
        /// Destroys Singleton-Instance
        /// </summary>
        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
        #endregion
        #endregion
    }
}