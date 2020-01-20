using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nl.FrankvHoof.MachineLearning.GeneticAlgorithms.FlappyBird
{
    [System.Serializable]
    public class BirdDNA
    {
        #region Variables
        /// <summary>
        /// Genes for this DNA
        /// </summary>
        public List<int> genes;
        /// <summary>
        /// Amount of Genes in this DNA
        /// </summary>
        private int dnaLength;
        /// <summary>
        /// Maximum Value for Genes in this DNA
        /// </summary>
        private int maxValue;
        #endregion

        #region Operators
        /// <summary>
        /// Indexing-Operator to refer directly to Genes in this DNA
        /// </summary>
        /// <param name="index">Index for Gene</param>
        /// <returns>Value of Gene</returns>
        public int this[int index]
        {
            get { return genes[index]; }
            set { genes[index] = value; }
        }
        #endregion

        #region Methods
        #region Constructors
        /// <summary>
        /// Constructor for BirdDNA
        /// </summary>
        /// <param name="length">Length of DNA (Amount of Genes)</param>
        /// <param name="maxVal">Maximum value for Genes</param>
        public BirdDNA(int length, int maxVal)
        {
            genes = new List<int>(length);
            dnaLength = length;
            maxValue = maxVal;
            SetRandom();
        }
        #endregion

        #region Public
        /// <summary>
        /// Randomizes Genes in DNA
        /// </summary>
        public void SetRandom()
        {
            genes.Clear();
            for (int i = 0; i < dnaLength; i++)
                genes.Add(Random.Range(0, maxValue + 1));
        }
        /// <summary>
        /// Mutates a single Gene
        /// </summary>
        public void Mutate()
        {
            genes[Random.Range(0, dnaLength)] = Random.Range(0, maxValue + 1);
        }
        /// <summary>
        /// Combines 2 DNA's to create Genes for this BirdDNA
        /// </summary>
        /// <param name="d1">Parent 1</param>
        /// <param name="d2">Parent 2</param>
        public void Combine(BirdDNA d1, BirdDNA d2)
        {
            for (int i = 0; i < dnaLength; i++)
                genes[i] = Random.Range(0, 10) < 5 ? d1.genes[i] : d2.genes[i];
        }
        #endregion
        #endregion
    }
}