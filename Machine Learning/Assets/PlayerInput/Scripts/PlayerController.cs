using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.PlayerInput
{
    public class PlayerController : MonoBehaviour
    {
        #region InnerTypes
        public struct WallDistance
        {
            public float forward;
            public float right;
            public float left;
            public float right45;
            public float left45;
        }
        #endregion

        #region Variables
        #region Editor
        /// <summary>
        /// Forward-Speed
        /// </summary>
        [SerializeField]
        private float speed = 10.0f;
        /// <summary>
        /// Rotation-Speed
        /// </summary>
        [SerializeField]
        private float rotationSpeed = 100.0f;
        /// <summary>
        /// Raycast-Distance
        /// </summary>
        [SerializeField]
        private float viewDistance = 200f;
        #endregion

        #region Private
        /// <summary>
        /// Saved Inputs & Raycast-Distances from Player
        /// </summary>
        private List<string> playerInputs = new List<string>();
        /// <summary>
        /// StreamWriter for PlayerInput
        /// </summary>
        private StreamWriter trainingDataFile;
        #endregion
        #endregion

        #region Methods
        #region Unity
        /// <summary>
        /// Creates TextFile for writing out PlayerInputs
        /// </summary>
        private void Start()
        {
            string path = Application.dataPath + "/PlayerInput/trainingdata.txt";
            trainingDataFile = File.CreateText(path);
        }

        /// <summary>
        /// Handles Player-Input and Stores it
        /// </summary>
        private void Update()
        {
            HandleInput();

            Debug.DrawRay(transform.position, transform.forward * viewDistance, Color.red);
            Debug.DrawRay(transform.position, transform.right * viewDistance, Color.green);
            Debug.DrawRay(transform.position, -transform.right * viewDistance, Color.green);

            WallDistance dist = RayCast();
            string values = dist.forward.ToString(CultureInfo.InvariantCulture) + "," + dist.right.ToString(CultureInfo.InvariantCulture) + "," + dist.left.ToString(CultureInfo.InvariantCulture) + "," + dist.right45.ToString(CultureInfo.InvariantCulture) + "," + dist.left45.ToString(CultureInfo.InvariantCulture) + "," + 
                RoundToHalf(Input.GetAxis("Vertical")).ToString(CultureInfo.InvariantCulture) + "," + RoundToHalf(Input.GetAxis("Horizontal")).ToString(CultureInfo.InvariantCulture);
            if (!playerInputs.Contains(values))
                playerInputs.Add(values);
        }

        /// <summary>
        /// Writes PlayerInputs to File
        /// </summary>
        private void OnApplicationQuit()
        {
            for (int i = 0; i < playerInputs.Count; i++)
                trainingDataFile.WriteLine(playerInputs[i]);
            trainingDataFile.Close();
        }
        #endregion

        #region Private
        /// <summary>
        /// Handles PlayerInput
        /// </summary>
        private void HandleInput()
        {
            float translation = Input.GetAxis("Vertical") * speed;
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
            translation *= Time.deltaTime;
            rotation *= Time.deltaTime;
            transform.Translate(0, 0, translation);
            transform.Rotate(0, rotation, 0);
        }
        /// <summary>
        /// Rounds values between 0 and 1 to 0.5
        /// </summary>
        /// <param name="x">Input</param>
        /// <returns>Output after rounding</returns>
        private float RoundToHalf(float x)
        {
            return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2f;
        }
        /// <summary>
        /// Raycasts for Wall-Distances
        /// </summary>
        /// <returns>Distances retrieved from RayCasts</returns>
        private WallDistance RayCast()
        {
            // RayCast to Walls
            RaycastHit hit;
            WallDistance dist;
            // Fwd
            if (Physics.Raycast(transform.position, transform.forward, out hit, viewDistance))
                dist.forward = 1f - RoundToHalf(hit.distance/viewDistance);
            else
                dist.forward = 0f;
            // Right
            if (Physics.Raycast(transform.position, transform.right, out hit, viewDistance))
                dist.right = 1f - RoundToHalf(hit.distance / viewDistance);
            else
                dist.right = 0f;
            // Left
            if (Physics.Raycast(transform.position, -transform.right, out hit, viewDistance))
                dist.left = 1f - RoundToHalf(hit.distance / viewDistance);
            else
                dist.left = 0f;
            // Right45
            if (Physics.Raycast(transform.position, (transform.forward + transform.right).normalized, out hit, viewDistance))
                dist.right45 = 1f - RoundToHalf(hit.distance / viewDistance);
            else
                dist.right45 = 0f;
            // Left45
            if (Physics.Raycast(transform.position, (transform.forward - transform.right).normalized, out hit, viewDistance))
                dist.left45 = 1f - RoundToHalf(hit.distance / viewDistance);
            else
                dist.left45 = 0f;
            return dist;
        }
        #endregion
        #endregion
    }
}