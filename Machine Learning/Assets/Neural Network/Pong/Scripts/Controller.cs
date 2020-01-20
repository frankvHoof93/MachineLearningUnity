using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.NeuralNetworks.Pong
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Controller : MonoBehaviour
    {
        private Rigidbody2D rb;
        [SerializeField]
        private float speed = 5f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                transform.Translate(Vector3.up * speed);
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                transform.Translate(-Vector3.up * speed);
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 8.8f, 17.4f), transform.position.z);
        }
    }
}