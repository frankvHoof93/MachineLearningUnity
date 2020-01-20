using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.GeneticAlgorithms.MazeWalker
{
    public class MazeBrain : MonoBehaviour
    {
        #region Variables
        #region Constants
        /// <summary>
        /// Amount of Genes in DNA
        /// </summary>
        private const int DNALength = 2;
        #endregion

        #region Public
        /// <summary>
        /// Whether this Brain is Alive
        /// </summary>
        public bool Alive { get; private set; } = true;
        /// <summary>
        /// DNA for this Brain (genes)
        /// </summary>
        public MazeDNA DNA { get; private set; }
        /// <summary>
        /// Distance Traveled by this Brain during its LifeTime
        /// </summary>
        public float TravelDistance { get; private set; }
        #endregion

        #region Editor
        /// <summary>
        /// Eyes-Object used for determining look-direction
        /// </summary>
        [SerializeField]
        private GameObject eyes;
        #endregion

        #region Private
        /// <summary>
        /// Spawn-Position
        /// </summary>
        private Vector3 startPos;
        /// <summary>
        /// Whether the brain can currently see a Wall
        /// </summary>
        private bool canSeeWall;
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Initializes Brain by creating DNA
        /// </summary>
        public void Init()
        {
            // Initialize DNA
            // 0 Forward
            // 1 Angle Turn
            DNA = new MazeDNA(DNALength, 360);
            startPos = transform.position;
        }
        /// <summary>
        /// Kills off a Brain
        /// </summary>
        public void EndLife()
        {
            TravelDistance = 0;
            Alive = false;
        }
        #endregion

        #region Unity
        /// <summary>
        /// Handles collision. Kills off brain if collision was with object tagged 'dead'
        /// </summary>
        /// <param name="collision">Collision-Event</param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "dead")
                EndLife();
        }
        /// <summary>
        /// Handles Raycasting for Walls
        /// </summary>
        private void Update()
        {
            if (!Alive)
                return;
            canSeeWall = false;
            RaycastHit hit;
            Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 0.5f, UnityEngine.Color.red);
            if (Physics.SphereCast(eyes.transform.position, 0.1f, eyes.transform.forward, out hit, 0.5f))
                if (hit.collider.gameObject.tag == "wall")
                    canSeeWall = true;
        }
        /// <summary>
        /// Handles Movement from Genes
        /// </summary>
        private void FixedUpdate()
        {
            if (!Alive)
                return;
            // Read DNA
            float turn = 0;
            float move = DNA[0];
            if (canSeeWall)
                turn = DNA[1];
            // Both values are 0-360
            transform.Translate(0, 0, move * 0.001f);
            transform.Rotate(0, turn, 0);
            // This could be done elsewhere, as it only matters at the end of an epoch
            TravelDistance = Vector3.Distance(startPos, transform.position);
        }
        #endregion
        #endregion
    }
}