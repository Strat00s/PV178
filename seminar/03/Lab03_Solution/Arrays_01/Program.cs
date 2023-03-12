using System;

namespace Arrays_03
{
    /// <summary>
    /// Based on demo sample of David Kadlec
    /// </summary>
    class Program
    {
        private static readonly Random Random = new Random();

        static void Main(string[] args)
        {
            // Reseni samostatne prace (zadani viz nize)
            // new Solution().InitializeArrays();
            // Console.WriteLine("Initialization complete...");
            // Console.ReadKey();

            RegularArrayDemo();

            JaggedArrayDemo();

            IndexerDemo();
        }

        private static void RegularArrayDemo()
        {
            // Deklarace s velikosti 10
            int[] arrayInt = new int[10];
            string[] arrayStrings = new string[10];
            Random[] arrayRandom = new Random[10];

            Console.WriteLine(arrayInt[0]);
            Console.WriteLine(arrayStrings[0] ?? "null");
            // bezpecne pouziti Elvis operatoru pro funkci WriteLine
            Console.WriteLine(arrayRandom[0]?.ToString() ?? "null");

            Console.WriteLine();

            // Implicitni deklarace (neni uvedena velikost)
            int[] arrayInt2 = new int[] { 1, 2, 3, 4 };
            string[] arrayStrings2 = { "AA", "BB", "CC" };
            Random[] arrayRandom2 = { new Random(), new Random() };

            Console.WriteLine(arrayInt2[0]);
            Console.WriteLine(arrayStrings2[0] ?? "null");
            Console.WriteLine(arrayRandom2[0]?.ToString() ?? "null");

            Console.WriteLine();

            // Length property na ziskani velikosti
            for (int i = 0; i < arrayStrings2.Length; i++)
            {
                // modifikace vsech prvku pole arrayStrings2
                arrayStrings2[i] += "D";
            }

            // Muzeme pohodle projit hodnoty pres foreach
            // Pole ve foreachi neupravujte, foreach je na cteni nebo vytvareni noveho pole
            foreach (var stringItem in arrayStrings2)
            {
                Console.WriteLine(stringItem);
            }


            // Kopirovani poli 
            // Pole je referencni typ, nestaci newArray = arrayStrings2, to bychom meli akorat dve reference na stejna data!
            var stringLength = arrayStrings2.Length;
            var newArray = new string[stringLength];
            Array.Copy(arrayStrings2, newArray, stringLength);


            // Dvourozmerne pole

            // Deklarace s velikosti 50x100
            int[,] array2D = new int[50, 100];

            // Rucne zadane rozmery
            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    array2D[x, y] = Random.Next(100);
                }
            }

            Console.WriteLine();
            Console.WriteLine(array2D[49, 99]);
            Console.WriteLine();
        }

        /// <summary>
        /// see: http://csharpcorner.mindcrackerinc.netdna-cdn.com/UploadFile/puranindia/jagged-arrays-in-C-Sharp-net/Images/jagged_array.gif
        /// </summary>
        private static void JaggedArrayDemo()
        {
            // nepravidelne 2D pole
            int[][] jaggedArray = new int[10][];

            for (int i = 0; i < 10; i++)
            {
                // Zde si nadefinujeme random velikost kazdeho pole pro dany index
                jaggedArray[i] = new int[Random.Next(100)];
            }

            // Nastaveni hodnot
            for (int x = 0; x < 10; x++)
            {
                // GetUpperBound(0) -> 9 for new int[10][];
                for (int y = 0; y <= jaggedArray[x].GetUpperBound(0); y++)
                {
                    jaggedArray[x][y] = Random.Next(100, 200);
                }
            }
        }

        /// <summary>
        /// see CornField class first
        /// </summary>
        private static void IndexerDemo()
        {
            int length = 10;
            var cornField = new CornField(length);

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (cornField[i, j])
                    {
                        Console.WriteLine($"Found it at: [{i},{j}]");
                    }
                }
            }

            Console.ReadKey();
        }

        //Samostatne procviceni - inicializace poli

        //I. u nize uvedenych poli uvedte, o jaky typ
        //se jedna (pravidelne, vicerozmerne, ...)
        //a urcete pocet dimenzi pole

        // a)
        private int[,,] array01 = new int[5, 10, 15];

        // b)
        private int[][][] array02 = new int[5][][];

        // c)
        private int[][,] array03 = new int[5][,];

        // d)
        private int[,][,] array04 = new int[5, 10][,];


        //II. kazde z uvedenych poli inicializujte
        //nahodne generovanymy cisly od 0 do 100
    }
}
