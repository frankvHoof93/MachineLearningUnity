using System.Linq;
using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.GeneticAlgorithms.FlappyBird
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BirdBrain : MonoBehaviour
    {
        #region InnerClasses
        /// <summary>
        /// Enum used to denote what type of obstacle is currently visible to this brain
        /// </summary>
        private enum VisibleObstacle : int
        {
            None = 0,
            Bottom = 1,
            Top = 2,
            Stalagmite = 3,
            Stalactite = 4
        };
        #endregion

        #region Variables
        #region Constants
        /// <summary>
        /// DNA-Length for BirdDNA
        /// </summary>
        private const int DNALength = 5;
        /// <summary>
        /// Tags for obstacles
        /// </summary>
        private readonly string[] crashWalls = { "wall", "bottom", "top" };
        #endregion

        #region Public
        /// <summary>
        /// Whether this Brain is Alive
        /// </summary>
        public bool Alive { get; private set; } = true;
        /// <summary>
        /// The amount of distance traveled by this brain over its lifetime (set upon death)
        /// </summary>
        public float DistanceTraveled { get; private set; }
        /// <summary>
        /// Duration of LifeTime for Brain
        /// </summary>
        public float LifeTime { get; private set; }
        /// <summary>
        /// Amount of Crashes endured during lifetime
        /// </summary>
        public int Crashes { get; private set; }
        /// <summary>
        /// DNA (Genes) for Brain
        /// </summary>
        public BirdDNA DNA;// { get; private set; }
        #endregion

        #region Editor
        /// <summary>
        /// Transform used for Eye-Position and -Rotation
        /// </summary>
        [SerializeField]
        private Transform eyes;
        /// <summary>
        /// MovementSpeed for bird
        /// </summary>
        [SerializeField]
        [Range(0.01f, 2f)]
        private float moveSpeed = 0.1f;
        #endregion

        #region Private
        /// <summary>
        /// RigidBody controlled by this brain
        /// </summary>
        private Rigidbody2D rb;
        /// <summary>
        /// Currently viewed Obstacle in the path of this bird
        /// </summary>
        private VisibleObstacle obstacle = 0;
        private Vector3 startPosition;
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Initializes Brain, setting default values
        /// </summary>
        public void Init()
        {
            // Initialize DNA
            // 0 Normal Forward
            // 1 Bottom
            // 2 Top
            // 3 Stalagmite
            // 4 Stalactite
            rb = GetComponent<Rigidbody2D>();
            DNA = new BirdDNA(DNALength, 200);
            transform.Translate(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
            startPosition = transform.position;
        }
        #endregion

        #region Unity
        /// <summary>
        /// Kills Bird if colliding with dead-object
        /// </summary>
        /// <param name="collision">Collision that occured</param>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "dead")
                Alive = false;
        }
        /// <summary>
        /// Adds to Crash-counter if colliding with wall
        /// </summary>
        /// <param name="collision">Collision that occured</param>
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (crashWalls.Contains(collision.gameObject.tag))
                Crashes++;
        }
        /// <summary>
        /// Checks for Obstacles
        /// </summary>
        private void Update()
        {
            if (!Alive)
                return;
            LifeTime = BirdPopulationManager.ElapsedTime;
            obstacle = VisibleObstacle.None;
            
            // Fwd
            Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 2f, UnityEngine.Color.red);
            RaycastHit2D hit = Physics2D.Raycast(eyes.transform.position, eyes.transform.forward, 2f);
            if (hit.collider?.gameObject.tag == "wall")
                obstacle = hit.collider.transform.rotation.eulerAngles.y == 0 ? VisibleObstacle.Stalagmite : VisibleObstacle.Stalactite;
            // Up
            Debug.DrawRay(eyes.transform.position, eyes.transform.up * 1.0f, UnityEngine.Color.red);
            hit = Physics2D.Raycast(eyes.transform.position, eyes.transform.up, 1f);
            if (hit.collider?.gameObject.tag == "top")
                obstacle = VisibleObstacle.Top;
            // Dwn
            Debug.DrawRay(eyes.transform.position, -eyes.transform.up * 1.0f, UnityEngine.Color.red);
            hit = Physics2D.Raycast(eyes.transform.position, -eyes.transform.up, 1f);
            if (hit.collider?.gameObject.tag == "bottom")
                obstacle = VisibleObstacle.Bottom;
        }
        /// <summary>
        /// Moves Bird
        /// </summary>
        private void FixedUpdate()
        {
            if (!Alive)
                return;
            Vector2 movementdir = new Vector2
            {
                x = 1,
                // Read from genes
                y = DNA[(int)obstacle]
            };
            rb.AddForce(transform.right * movementdir.x);
            rb.AddForce(transform.up * movementdir.y * moveSpeed);
            DistanceTraveled = transform.position.x - startPosition.x;
        }
        #endregion
        #endregion
    }
}
