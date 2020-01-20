using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace nl.FrankvHoof.MachineLearning.GeneticAlgorithms.Movement
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class MovementBrain : MonoBehaviour
    {
        #region Variables
        #region Constants
        /// <summary>
        /// DNA-Length for MovementDNA
        /// </summary>
        private const int DNALength = 1;
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
        /// DNA (Genes) for Brain
        /// </summary>
        public MovementDNA DNA { get; private set; }
        #endregion

        #region Private
        /// <summary>
        /// ThirdPersonCharacter-Controller for Brain
        /// </summary>
        private ThirdPersonCharacter character;
        /// <summary>
        /// Instantiation-Position
        /// </summary>
        private Vector3 startPos;
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
            // 0 - Forward
            // 1 - Back
            // 2 - Left
            // 3 - Right
            // 4 - Jump
            // 5 - Crouch
            DNA = new MovementDNA(DNALength, 6);
            character = GetComponent<ThirdPersonCharacter>();
            LifeTime = 0;
            Alive = true;
            startPos = transform.position;
        }
        /// <summary>
        /// Ends Life (if Alive).
        /// </summary>
        /// <returns>Distance Traveled</returns>
        public float EndLife()
        {
            if (Alive)
            {
                DistanceTraveled = Vector3.Distance(startPos, new Vector3(transform.position.x, startPos.y, transform.position.z)); // We don't care about y-movement
                Alive = false;
            }
            return DistanceTraveled;
        }
        #endregion

        #region Unity
        /// <summary>
        /// Handles Collisions. Kills Brain if collision with Death-Plane is detected
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (Alive && collision.gameObject.tag == "dead")
                EndLife();
        }

        /// <summary>
        /// Moves Character (if Alive) using DNA
        /// </summary>
        private void FixedUpdate()
        {
            if (Alive)
            {
                int dna = DNA[0];
                float v = dna == 0 ? 1 : dna == 1 ? -1 : 0;
                float h = dna == 2 ? 1 : dna == 3 ? -1 : 0;
                bool jump = dna == 4;
                bool crouch = dna == 5;
                Vector3 move = v * Vector3.forward + h * Vector3.right;
                character.Move(move, crouch, jump);
                // This can be deltaTime, as it will return fixedDeltaTime when called during the FixedUpdate-loop
                LifeTime += Time.deltaTime;
            }
        }
        #endregion
        #endregion
    }
}