using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.Perceptron
{
    [RequireComponent(typeof(Perceptron))]
    public class Throw : MonoBehaviour
    {
        #region Variables
        #region Editor
        /// <summary>
        /// Prefab for Sphere
        /// </summary>
        [SerializeField]
        private GameObject spherePrefab;
        /// <summary>
        /// Prefab for Cube
        /// </summary>
        [SerializeField]
        private GameObject cubePrefab;
        /// <summary>
        /// Green Material
        /// </summary>
        [SerializeField]
        private Material green;
        /// <summary>
        /// Red Material
        /// </summary>
        [SerializeField]
        private Material red;
        #endregion

        #region Private
        /// <summary>
        /// Perceptron
        /// </summary>
        private Perceptron perceptron;
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Sets Perceptron-Reference
        /// </summary>
        private void Awake()
        {
            perceptron = GetComponent<Perceptron>();
        }
        /// <summary>
        /// Throws Objects
        /// </summary>
        private void Update()
        {
            GameObject g = null;
            if (Input.GetKeyDown("1"))
            {
                g = Instantiate(spherePrefab, Camera.main.transform.position, Camera.main.transform.rotation);
                g.GetComponent<Renderer>().material = red;
                g.GetComponent<Rigidbody>().AddForce(0, 0, 500);
                perceptron.SendInput(0, 0, 0);
            }
            else if (Input.GetKeyDown("2"))
            {
                g = Instantiate(spherePrefab, Camera.main.transform.position, Camera.main.transform.rotation);
                g.GetComponent<Renderer>().material = green;
                g.GetComponent<Rigidbody>().AddForce(0, 0, 500);
                perceptron.SendInput(0, 1, 1);
            }
            else if (Input.GetKeyDown("3"))
            {
                g = Instantiate(cubePrefab, Camera.main.transform.position, Camera.main.transform.rotation);
                g.GetComponent<Renderer>().material = red;
                g.GetComponent<Rigidbody>().AddForce(0, 0, 500);
                perceptron.SendInput(1, 0, 1);
            }
            else if (Input.GetKeyDown("4"))
            {
                g = Instantiate(cubePrefab, Camera.main.transform.position, Camera.main.transform.rotation);
                g.GetComponent<Renderer>().material = green;
                g.GetComponent<Rigidbody>().AddForce(0, 0, 500);
                perceptron.SendInput(1, 1, 1);
            }
            if (g != null)
                Destroy(g, 15f);
        }
        #endregion
    }
}