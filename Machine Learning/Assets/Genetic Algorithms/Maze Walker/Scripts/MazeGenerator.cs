using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.GeneticAlgorithms.MazeWalker
{
    public class MazeGenerator : MonoBehaviour
    {
        #region Variables
        #region Editor
        /// <summary>
        /// Prefab for Wall-Blocks
        /// </summary>
        [SerializeField]
        private GameObject blockPrefab;
        #endregion

        #region Private
        /// <summary>
        /// Width of Maze
        /// </summary>
        private int width = 40;
        /// <summary>
        /// Depth of Maze
        /// </summary>
        private int depth = 40;
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Creates Maze
        /// </summary>
        private void Awake()
        {
            for (int w = 0; w < width; w++)
                for (int d = 0; d < depth; d++)
                {
                    Vector3 position = transform.position;
                    Vector3 add = new Vector3(w, 0, d);
                    if (w == 0 || d == 0)   //outside walls bottom and left
                        position += add;
                    else if (w < 3 && d < 3)
                        continue;
                    else if (w == width - 1 || d == depth - 1) //outside walls top and right
                        position += add;
                    else if (Random.Range(0, 5) < 1)
                        position += add;
                    if (position != transform.position) // Don't spawn if position was not changed 
                        Instantiate(blockPrefab, position, Quaternion.identity);
                }
        }
        #endregion
    }
}