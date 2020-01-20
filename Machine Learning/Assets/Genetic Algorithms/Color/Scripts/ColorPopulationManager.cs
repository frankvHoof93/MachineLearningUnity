using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace nl.FrankvHoof.MachineLearning.GeneticAlgorithms.Color
{
    public class ColorPopulationManager : MonoBehaviour
    {
        #region Variables
        #region Public
        /// <summary>
        /// Total Elapsed Time for current generation
        /// </summary>
        public static float ElapsedTime { get; private set; }
        #endregion

        #region Editor
        /// <summary>
        /// Prefab for single person in population
        /// </summary>
        [SerializeField]
        private GameObject personPrefab;
        /// <summary>
        /// Size of population to generate
        /// </summary>
        [SerializeField]
        private int populationSize = 10;
        /// <summary>
        /// Max lifetime-duration for generation
        /// </summary>
        [SerializeField]
        private float trialTime = 10f;
        #endregion

        #region Private
        /// <summary>
        /// Current Generation-Index
        /// </summary>
        private int currGeneration = 1;
        /// <summary>
        /// Population being Managed
        /// </summary>
        private List<ColorDNA> population = new List<ColorDNA>();
        /// <summary>
        /// GUIStyle for Overlay-Text
        /// </summary>
        private GUIStyle GUIStyle;
        #endregion
        #endregion

        #region Methods
        #region Unity
        /// <summary>
        /// Shows text-overlay
        /// </summary>
        private void OnGUI()
        {
            if (GUIStyle == null)
            {
                GUIStyle = new GUIStyle
                {
                    fontSize = 50
                };
                GUIStyle.normal.textColor = UnityEngine.Color.white;
            }
            GUI.Label(new Rect(10, 10, 100, 20), "Generation: " + currGeneration, GUIStyle);
            GUI.Label(new Rect(10, 65, 100, 20), "Trial Time: " + (int)ElapsedTime, GUIStyle);
        }
        /// <summary>
        /// Creates Initial Population
        /// </summary>
        private void Start()
        {
            for (int i = 0; i < populationSize; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-9f, 9f), Random.Range(-4.5f, 4.5f), 0);
                GameObject go = Instantiate(personPrefab, pos, Quaternion.identity);
                ColorDNA dna = go.GetComponent<ColorDNA>();
                dna.SetColor(new UnityEngine.Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
                dna.SetScale(Random.Range(0.1f, 0.3f));
                population.Add(dna);
            }
        }
        /// <summary>
        /// Breeds populations between trials
        /// </summary>
        private void Update()
        {
            ElapsedTime += Time.deltaTime;
            if (ElapsedTime > trialTime)
            {
                BreedNewPopulation();
                ElapsedTime = 0;
            }
        }
        #endregion

        #region Private
        /// <summary>
        /// Breeds a new Population from the current population
        /// </summary>
        private void BreedNewPopulation()
        {
            List<ColorDNA> newPopulation = new List<ColorDNA>();
            List<ColorDNA> sortedPopulation = population.OrderByDescending(o => o.Alive ? o.LifeTime : trialTime).ToList();
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
        /// Breeds a ColorDNA from 2 parents
        /// </summary>
        /// <param name="parent1">First parent for breeding</param>
        /// <param name="parent2">Second parent for breeding</param>
        /// <returns>ColorDNA bred from parents</returns>
        private ColorDNA Breed(ColorDNA parent1, ColorDNA parent2)
        {
            Vector3 pos = new Vector3(Random.Range(-9f, 9f), Random.Range(-4.5f, 4.5f), 0);
            GameObject offspring = Instantiate(personPrefab, pos, Quaternion.identity);
            // DNA swap
            UnityEngine.Color c1 = parent1.Color;
            UnityEngine.Color c2 = parent2.Color;
            UnityEngine.Color newColor;
            float newScale;
            if (Random.Range(0, 1000) > 5)
            {
                newColor = new UnityEngine.Color
                {
                    r = Random.Range(0, 10) < 5 ? c1.r : c2.r,
                    g = Random.Range(0, 10) < 5 ? c1.g : c2.g,
                    b = Random.Range(0, 10) < 5 ? c1.b : c2.b,
                    a = 1
                };
                newScale = Random.Range(0, 10) < 5 ? parent1.Scale : parent2.Scale;
            }
            else
            {
                newScale = Random.Range(0.1f, 0.3f);
                newColor = Random.ColorHSV();
                newColor.a = 1;
            }
            ColorDNA offspringDNA = offspring.GetComponent<ColorDNA>();
            offspringDNA.SetColor(newColor);
            offspringDNA.SetScale(newScale);
            return offspringDNA;
        }
        #endregion
        #endregion
    }
}