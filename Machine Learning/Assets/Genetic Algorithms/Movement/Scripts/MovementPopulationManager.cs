using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.GeneticAlgorithms.Movement
{
    public class MovementPopulationManager : MonoBehaviour
    {
        #region Variables
        #region Editor
        /// <summary>
        /// Prefab for Person
        /// </summary>
        [SerializeField]
        private GameObject personPrefab;
        /// <summary>
        /// Size for Population
        /// </summary>
        [SerializeField]
        private int populationSize;
        /// <summary>
        /// Max lifetime-duration for generation
        /// </summary>
        [SerializeField]
        private float trialTime = 5;
        #endregion

        #region Private
        /// <summary>
        /// Population being Managed
        /// </summary>
        private readonly List<MovementBrain> population = new List<MovementBrain>();
        /// <summary>
        /// Elapsed Time for current Generation
        /// </summary>
        private float elapsedTime = 0;
        /// <summary>
        /// Index for current Generation
        /// </summary>
        private int currGeneration = 1;
        /// <summary>
        /// GUIStyle for Text-Overlay
        /// </summary>
        private GUIStyle guiStyle;
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
            GUI.BeginGroup(new Rect(10, 10, 250, 150));
            GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
            GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + currGeneration, guiStyle);
            GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", elapsedTime), guiStyle);
            GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
            GUI.EndGroup();
        }
        /// <summary>
        /// Creates Initial Population
        /// </summary>
        private void Start()
        {
            for (int i = 0; i < populationSize; i++)
            {
                Vector3 pos = new Vector3(transform.position.x + Random.Range(-2, 2), transform.position.y, transform.position.z + Random.Range(2, -2));
                MovementBrain b = Instantiate(personPrefab, pos, transform.rotation).GetComponent<MovementBrain>();
                b.Init();
                population.Add(b);
            }
        }
        /// <summary>
        /// Breeds populations between trials
        /// </summary>
        private void Update()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= trialTime)
            {
                BreedNewPopulation();
                elapsedTime = 0;
            }
        }
        #endregion

        #region Private
        /// <summary>
        /// Breeds a new Population from the current population
        /// </summary>
        private void BreedNewPopulation()
        {
            population.ForEach(brain => brain.EndLife());
            List<MovementBrain> sortedPopulation = population.OrderBy(o => o.DistanceTraveled).ThenBy(o => o.Alive ? trialTime : o.LifeTime).ToList();
            population.Clear();

            for (int i = (int)(sortedPopulation.Count / 2f) - 1; i < sortedPopulation.Count - 1; i++)
            {
                population.Add(Breed(sortedPopulation[i], sortedPopulation[i + 1]));
                population.Add(Breed(sortedPopulation[i + 1], sortedPopulation[i]));
            }

            for (int i = 0; i < sortedPopulation.Count; i++)
                Destroy(sortedPopulation[i].gameObject);
            currGeneration++;
        }
        /// <summary>
        /// Breeds a MovementBrain from 2 parents
        /// </summary>
        /// <param name="parent1">First parent for breeding</param>
        /// <param name="parent2">Second parent for breeding</param>
        /// <returns>MovementBrain bred from parents</returns>
        private MovementBrain Breed(MovementBrain b1, MovementBrain b2)
        {
            Vector3 pos = new Vector3(transform.position.x + Random.Range(-2, 2), transform.position.y, transform.position.z + Random.Range(2, -2));
            MovementBrain offspring = Instantiate(personPrefab, pos, transform.rotation).GetComponent<MovementBrain>();
            offspring.Init();
            if (Random.Range(0, 100) == 1) // Mutation
                offspring.DNA.Mutate();
            else
                offspring.DNA.Combine(b1.DNA, b2.DNA);
            return offspring;
        }
        #endregion
        #endregion
    }
}