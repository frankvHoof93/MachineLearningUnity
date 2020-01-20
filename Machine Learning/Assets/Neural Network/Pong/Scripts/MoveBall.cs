using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.NeuralNetworks.Pong
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MoveBall : MonoBehaviour
    {
        #region Variables
        #region Editor
        /// <summary>
        /// Speed for Movement
        /// </summary>
        [SerializeField]
        [Range(50, 5000)]
        private float speed = 400;
        /// <summary>
        /// Blip-AudioEffect
        /// </summary>
        [SerializeField]
        private AudioSource blip;
        /// <summary>
        /// Blop-AudioEffect
        /// </summary>
        [SerializeField]
        private AudioSource blop;
        #endregion

        #region Private
        /// <summary>
        /// Start-Position for Ball
        /// </summary>
        private Vector3 startPos;
        /// <summary>
        /// RigidBody for Ball
        /// </summary>
        private Rigidbody2D rb2D;
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Resets Ball to original position, and starts moving in a random direction
        /// </summary>
        public void ResetBall()
        {
            transform.position = startPos;
            rb2D.velocity = Vector3.zero;
            Vector3 direction = new Vector3((Random.Range(-1f, 1f) < 0 ? -1 : 1) * Random.Range(100, 300), Random.Range(-100, 100), 0).normalized;
            rb2D.AddForce(direction * speed);
        }
        #endregion

        #region Unity
        /// <summary>
        /// Initializes Ball
        /// </summary>
        private void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();
            startPos = transform.position;
            ResetBall();
        }
        /// <summary>
        /// Resets Ball if Space is pressed
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ResetBall();
        }
        /// <summary>
        /// Handles collision with Walls
        /// </summary>
        /// <param name="collision">Collision that ocurred</param>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "backwall")
            {
                blop.Play();
                // Assign Score
                PongUIManager.Instance.AddScore(collision.gameObject.transform.position.x > 0);
                // Reset Ball
                ResetBall();
            }
            else blip.Play();
        }
        #endregion
        #endregion
    }
}