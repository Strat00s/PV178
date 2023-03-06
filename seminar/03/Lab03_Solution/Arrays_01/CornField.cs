using System;

namespace Arrays_03
{
    /// <summary>
    /// All credit goes to David Kadlec, author of this demo sample.
    /// 
    /// Predstavuje kukuricne pole s jednou kukurici.
    /// </summary>
    internal class CornField
    {
        private bool[,] mField;
        private readonly Random mRandom = new Random();

        public CornField(int length)
        {
            mField = new bool[length, length];

            // nahodne ukryti jedne kukurice
            mField[mRandom.Next(length), mRandom.Next(length)] = true;
        }


        /// <summary>
        /// Indexer.
        /// 
        /// Podobne jako property
        /// V teto ukazce je pouzit pro pristup k poli
        /// Misto jmena je klicove slovo this.
        /// Za nim jsou parametry v hranatych zavorkach.
        /// </summary>
        public bool this[int x, int y]
        {
            get => mField[x, y];
            set => mField[x, y] = value;
        }
    }
}