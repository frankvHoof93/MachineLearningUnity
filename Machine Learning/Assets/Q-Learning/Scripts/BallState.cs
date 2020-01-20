using UnityEngine;
namespace nl.FrankvHoof.MachineLearning.QLearning
{
    public class BallState : MonoBehaviour
    {
        /// <summary>
        /// Public Getter for Dropped
        /// </summary>
        public bool IsDropped => dropped;
        /// <summary>
        /// Whether the Ball has been dropped
        /// </summary>
        [SerializeField]
        public bool dropped = false;
        /// <summary>
        /// Resets Dropped-State
        /// </summary>
        public void ResetState()
        {
            dropped = false;
        }
        /// <summary>
        /// Checks for collision with boundaries
        /// </summary>
        /// <param name="col">Collider collision occured with</param>
        private void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.tag == "drop")
            {
                dropped = true;
                Debug.Log("Oh no. I've been dropped");
            }
        }
    }
}