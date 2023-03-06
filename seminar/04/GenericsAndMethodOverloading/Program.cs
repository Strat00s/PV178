namespace GenericsAndMethodOverloading
{
    /// <summary>
    /// Ukol:
    /// 
    /// I. ve tride SwapUtils vytvorte dve
    /// pretizene metody: Swap(...) 
    /// prvni z nich bude slouzit k zamene
    /// dvou integeru, druha pak vzajemne 
    /// vymeni dva stringy
    /// 
    /// II. do tridy SwapUtils pridejte 
    /// generickou metodu GenericSwap
    /// ktera zameni 2 promene
    /// 
    /// III. ve tride CollectionUtils
    /// vytvorte dve genericke metody
    /// dle uvedenych instrukci 
    /// 
    /// IV. odkomentovani metod 
    /// SwapUtilTest a CollectionUtilTest
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            SwapUtilTest();
            CollectionUtilTest();
        }

        public static void SwapUtilTest()
        {
            /*
            var firstInt = 1;
            var secondInt = 2;

            var firstString = "Knock, knock.";
            var secondString = "Race condition. Who’s there?";

            Console.WriteLine("Integer swap test:");
            Console.WriteLine($"Before: first: {firstInt}, second: {secondInt}");
            SwapUtils.Swap(ref firstInt, ref secondInt);
            Console.WriteLine($"After: first: {firstInt}, second: {secondInt}" + Environment.NewLine);
            Console.ReadKey();

            Console.WriteLine("String swap test:");
            Console.WriteLine($"Before: first: {firstString}, second: {secondString}");
            SwapUtils.Swap(ref firstString, ref secondString);
            Console.WriteLine($"After: first: {firstString}, second: {secondString}" + Environment.NewLine);
            Console.ReadKey();

            firstInt = 5;
            secondInt = 10;

            firstString = "str1";
            secondString = "str2";


            Console.WriteLine("Generic swap test 01:");
            Console.WriteLine($"Before: first: {firstInt}, second: {secondInt}");
            SwapUtils.GenericSwap(ref firstInt, ref secondInt);
            Console.WriteLine($"After: first: {firstInt}, second: {secondInt}" + Environment.NewLine);

            Console.WriteLine("Generic swap test 02:");
            Console.WriteLine($"Before: first: {firstString}, second: {secondString}");
            SwapUtils.GenericSwap(ref firstString, ref secondString);
            Console.WriteLine($"After: first: {firstString}, second: {secondString}" + Environment.NewLine);
            Console.ReadKey();
            */
        }

        public static void CollectionUtilTest()
        {
            /*
            var list = new List<String>() { "One" };

            Console.WriteLine("AddManyToCollection test:");
            Console.WriteLine("Initial content:");
            WriteAllItems(list);
            CollectionUtils.AddManyToCollection(list, new[] { "Two", "Three" });
            Console.WriteLine("After Addition:");
            WriteAllItems(list);
            Console.WriteLine();
            Console.ReadKey();

            Console.WriteLine("ToArray test:");
            var array = CollectionUtils.ToArray<string>(list);
            Console.WriteLine("Array content:");
            WriteAllItems(array);
            Console.ReadKey();
            */
        }

        private static void WriteAllItems<T>(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Console.Write($"{item}, ");
            }
            Console.WriteLine();
        }
    }
}
