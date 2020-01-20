using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.GeneticAlgorithms.Senses
{
    public class SensesBrain : MonoBehaviour
    {
        #region Variables
        #region Constants
        /// <summary>
        /// DNA-Length for SensesDNA
        /// </summary>
        private const int DNALength = 2;
        #endregion

        #region Public
        /// <summary>
        /// Whether this Brain is Alive
        /// </summary>
        public bool Alive { get; private set; } = true;
        /// <summary>
        /// Duration of LifeTime for Brain
        /// </summary>
        public float LifeTime { get; private set; }
        /// <summary>
        /// Time spent walking (moving forward) during Life
        /// </summary>
        public float TravelTime { get; private set; }
        /// <summary>
        /// DNA (Genes) for Brain
        /// </summary>
        public SensesDNA DNA { get; private set; }
        /// <summary>
        /// Whether the Brain can see the ground
        /// </summary>
        public bool CanSeeGround { get; private set; } = true;
        #endregion

        #region Editor
        /// <summary>
        /// Eyes used for checking Ground
        /// </summary>
        [SerializeField]
        private GameObject eyes;
        /// <summary>
        /// Ethan-Prefab which follows this bot
        /// </summary>
        [SerializeField]
        private GameObject ethanPrefab;
        #endregion

        #region Private
        /// <summary>
        /// Ethan-Instance which follows this bot
        /// </summary>
        private GameObject ethanInstance;
        /// <summary>
        /// LayerMask for Raycasting
        /// </summary>
        private LayerMask layer;
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Initializes Brain, setting default values
        /// </summary>
        public void Init(LayerMask layerMask)
        {
            // Initialize DNA
            // 0 - Forward
            // 1 - Left
            // 2 - Right
            DNA = new SensesDNA(DNALength, 3);
            LifeTime = 0;
            Alive = true;
            layer = layerMask;
            ethanInstance = Instantiate(ethanPrefab, transform.position, transform.rotation);
            ethanInstance.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().target = transform;
        }
        /// <summary>
        /// Ends Life (if Alive).
        /// </summary>
        /// <returns>Distance Traveled</returns>
        public void EndLife(bool setColor = false)
        {
            if (Alive)
            {
                Alive = false;
                if (setColor)
                {
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    block.SetColor("_Color", UnityEngine.Color.red);
                    foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
                        renderer.SetPropertyBlock(block);
                }
            }
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
                EndLife(true);
        }

        /// <summary>
        /// Moves Character (if Alive)
        /// </summary>
        private void Update()
        {
            if (!Alive)
                return;
            // Check Eyes
            Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 10f, UnityEngine.Color.red, .5f);
            CanSeeGround = false;
            RaycastHit hit;
            if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10f, out hit, 10f, ~layer))
                if (hit.collider.gameObject.tag == "platform")
                    CanSeeGround = true;
            LifeTime = SensesPopulationManager.ElapsedTime;

            // Read DNA and Act
            float turn = 0;
            float move = 0;
            if (CanSeeGround)
            {
                if (DNA[0] == 0) move = 1;
                else if (DNA[0] == 1) turn = -90;
                else if (DNA[0] == 2) turn = 90;
            }
            else
            {
                if (DNA[1] == 0) move = 1;
                else if (DNA[1] == 1) turn = -90;
                else if (DNA[1] == 2) turn = 90;
            }
            if (move != 0)
                TravelTime += Time.deltaTime;
            transform.Translate(0, 0, move * 0.2f);
            transform.Rotate(0, turn, 0);
        }
        /// <summary>
        /// Destroys following Ethan
        /// </summary>
        private void OnDestroy()
        {
            Destroy(ethanInstance);
        }
        #endregion
        #endregion
    }
}