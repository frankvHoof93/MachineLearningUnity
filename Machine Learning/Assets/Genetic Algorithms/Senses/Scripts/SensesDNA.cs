using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace nl.FrankvHoof.MachineLearning.GeneticAlgorithms.Senses
{
    public class SensesDNA
    {
        #region Variables
        /// <summary>
        /// Genes for this DNA
        /// </summary>
        private readonly List<int> genes = new List<int>();
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
        /// Constructor for SensesDNA
        /// </summary>
        /// <param name="len">Length of DNA (Amount of Genes)</param>
        /// <param name="maxVal">Maximum Value for Genes</param>
        public SensesDNA(int len, int maxVal)
        {
            dnaLength = len;
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
        /// Combines 2 DNA's to create Genes for this SensesDNA
        /// </summary>
        /// <param name="d1">Parent 1</param>
        /// <param name="d2">Parent 2</param>
        public void Combine(SensesDNA d1, SensesDNA d2)
        {
            for (int i = 0; i < dnaLength; i++)
            {
                if (i < dnaLength / 2f) // Take first half from first parent
                    genes[i] = d1.genes[i];
                else // Take second half from second parent
                    genes[i] = d2.genes[i];
            }
        }
        #endregion
        #endregion
    }
}