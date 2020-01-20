using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.GeneticAlgorithms.Color
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class ColorDNA : MonoBehaviour
    {
        #region Variables
        #region Public
        /// <summary>
        /// Whether this Object is Alive
        /// </summary>
        public bool Alive { get; private set; } = true;
        /// <summary>
        /// Scale(-Gene) for Object
        /// </summary>
        public float Scale { get; private set; }
        /// <summary>
        /// Color(-Gene) for Object
        /// </summary>
        public UnityEngine.Color Color { get; private set; }
        /// <summary>
        /// Duration of Object's Life
        /// </summary>
        public float LifeTime { get; private set; }
        #endregion

        #region Private
        /// <summary>
        /// Renderer for Object
        /// </summary>
        private SpriteRenderer spriteRenderer;
        /// <summary>
        /// Collider for Object
        /// </summary>
        private Collider2D coll;
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Sets Color-Genes
        /// </summary>
        /// <param name="color">Color to set</param>
        public void SetColor(UnityEngine.Color color)
        {
            color.a = 1;
            Color = color;
            spriteRenderer.color = color;
        }
        /// <summary>
        /// Sets Scale-Gene
        /// </summary>
        /// <param name="scale">Scale to set</param>
        public void SetScale(float scale)
        {
            Scale = scale;
            transform.localScale = scale * Vector3.one;
        }
        #endregion

        #region Unity
        /// <summary>
        /// Grabs references to Components
        /// </summary>
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            coll = GetComponent<Collider2D>();
        }
        /// <summary>
        /// Handles Click-Collision
        /// </summary>
        private void OnMouseDown()
        {
            Alive = true;
            LifeTime = ColorPopulationManager.ElapsedTime;
            spriteRenderer.enabled = false;
            coll.enabled = false;
        }
        #endregion
        #endregion
    }
}