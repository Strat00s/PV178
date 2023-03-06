using System;

namespace Arrays_03
{
    public class Solution
    {
        // pravidelne 3D pole
        private int[,,] array01 = new int[5, 10, 15];

        // nepravidelne pole nepravidelnych 2D poli
        private int[][][] array02 = new int[5][][];

        // nepravidelne pole pravidelnych 2D poli
        private int[][,] array03 = new int[5][,];

        // nepravidelne pole tvorene pravidelnym 2D polem,
        // obsahujici 2D pole (celkem 4 dimenze)
        private int[,][,] array04 = new int[5, 10][,];

        public void InitializeArrays()
        {
            // vytvoreni generatoru (pseudo)nahodnych cisel,
            // pro generovani lze pouzit metodu Next(...)
            var random = new Random();

            array01 = new int[5, 10, 15];
            for (int i = 0; i <= array01.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= array01.GetUpperBound(1); j++)
                {
                    for (int k = 0; k <= array01.GetUpperBound(2); k++)
                    {
                        array01[i, j, k] = random.Next(100);
                    }
                }
            }


            array02 = new int[5][][];
            for (int i = 0; i <= array02.GetUpperBound(0); i++)
            {
                // generate a random number for inner jagged array length
                var innerJaggedArrayLength = random.Next(100);
                array02[i] = new int[innerJaggedArrayLength][];

                // notice how get upper bound is used (we go inside the jagged array)
                for (int j = 0; j <= array02[i].GetUpperBound(0); j++)
                {
                    var innerArrayLength = random.Next(100);
                    array02[i][j] = new int[innerArrayLength];

                    // now we can finally assign the innermost array
                    for (int k = 0; k <= array02[i][j].GetUpperBound(0); k++)
                    {
                        array02[i][j][k] = random.Next(100);
                    }
                }
            }

            array03 = new int[5][,];
            for (int i = 0; i <= array03.GetUpperBound(0); i++)
            {
                // initialize the inner regular 2D array
                array03[i] = new int[10, 15];

                for (int j = 0; j <= array03[i].GetUpperBound(0); j++)
                {
                    for (int k = 0; k <= array03[i].GetUpperBound(1); k++)
                    {
                        array03[i][j, k] = random.Next(100);
                    }
                }
            }

            array04 = new int[5, 10][,];
            for (int i = 0; i <= array04.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= array04.GetUpperBound(1); j++)
                {
                    // initialize the inner regular 2D array
                    array04[i, j] = new int[15, 20];
                    for (int k = 0; k <= array04[i, j].GetUpperBound(0); k++)
                    {
                        for (int l = 0; l <= array04[i, j].GetUpperBound(1); l++)
                        {
                            array04[i, j][k, l] = random.Next(100);
                        }
                    }
                }
            }
        }
    }
}
