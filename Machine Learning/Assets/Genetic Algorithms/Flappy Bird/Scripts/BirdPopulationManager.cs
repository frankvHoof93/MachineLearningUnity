using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.GeneticAlgorithms.FlappyBird
{
    public class BirdPopulationManager : MonoBehaviour
    {
        #region Variables
        #region Public
        /// <summary>
        /// Elapsed Time for current Generation
        /// </summary>
        public static float ElapsedTime { get; private set; }
        #endregion

        #region Editor
        /// <summary>
        /// Duration for a single Epoch
        /// </summary>
        [SerializeField]
        private float trialTime = 15;
        /// <summary>
        /// Size for Population
        /// </summary>
        [SerializeField]
        private int populationSize = 50;
        /// <summary>
        /// SpawnPoint for Population
        /// </summary>
        [SerializeField]
        private Transform spawnPoint;
        /// <summary>
        /// Prefab for Population
        /// </summary>
        [SerializeField]
        private GameObject botPrefab;
        [SerializeField]
        private float timeScale = 3f;
        #endregion

        #region Private
        /// <summary>
        /// Population being Managed
        /// </summary>
        private List<BirdBrain> population = new List<BirdBrain>();
        /// <summary>
        /// Generation-#
        /// </summary>
        private int currGeneration = 1;
        /// <summary>
        /// GUIStyle for Overlay
        /// </summary>
        private GUIStyle guiStyle = null;
        #endregion
        #endregion

        #region Methods
        #region Unity
        /// <summary>
        /// Shows text-overlay
        /// </summary>
        private void OnGUI()
        {
            if (guiStyle == null)
            {
                guiStyle = new GUIStyle { fontSize = 25 };
                guiStyle.normal.textColor = UnityEngine.Color.white;
            }
            GUI.BeginGroup(new Rect(10, 10, 250, 180));
            GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
            GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + currGeneration, guiStyle);
            GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", ElapsedTime), guiStyle);
            GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
            GUI.Label(new Rect(10, 100, 200, 30), "Living: " + population.Where(i => i.Alive).Count(), guiStyle);
            GUI.EndGroup();
        }
        /// <summary>
        /// Creates Initial Population
        /// </summary>
        private void Start()
        {
            for (int i = 0; i < populationSize; i++)
            {
                BirdBrain b = Instantiate(botPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<BirdBrain>();
                b.Init();
                population.Add(b);
            }
        }
        /// <summary>
        /// Handles timer for Epoch
        /// </summary>
        private void Update()
        {
            ElapsedTime += Time.deltaTime;
            if (ElapsedTime >= trialTime)
            {
                BreedNewPopulation();
                ElapsedTime = 0;
            }
            Time.timeScale = timeScale;
        }
        #endregion

        #region Private
        /// <summary>
        /// Breeds 2 brains, returning their offspring
        /// </summary>
        /// <param name="parent1">Parent 1 for breeding</param>
        /// <param name="parent2">Parent 2 for breeding</param>
        /// <returns>Offspring</returns>
        private BirdBrain Breed(BirdBrain parent1, BirdBrain parent2)
        {
            BirdBrain offspring = Instantiate(botPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<BirdBrain>();
            offspring.Init();
            offspring.DNA.Combine(parent1.DNA, parent2.DNA);
            if (Random.Range(0, 100) <= 10) // 10% chance to mutate
                offspring.DNA.Mutate();                
            return offspring;
        }
        /// <summary>
        /// Breeds a new population from the existing one
        /// </summary>
        private void BreedNewPopulation()
        {
            List<BirdBrain> sortedPopulation = population.OrderBy(o => o.Alive ? o.DistanceTraveled - o.Crashes * 0.1f : 0).ToList();
            population.Clear();
            // Top 10%
            for (int i = (int)(sortedPopulation.Count * 0.9f); i < sortedPopulation.Count - 1; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    population.Add(Breed(sortedPopulation[i], sortedPopulation[i + 1]));
                    population.Add(Breed(sortedPopulation[i + 1], sortedPopulation[i]));
                }
            }
            for (int i = 0; i < sortedPopulation.Count; i++)
                Destroy(sortedPopulation[i].gameObject);
            currGeneration++;
        }
        #endregion
        #endregion
    }
}
