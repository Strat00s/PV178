namespace Patterns_03
{
    /// <summary>
    /// C# 7.0 pattern matching features.
    /// 
    /// This sample is partially based on Filip Opaleny and Milan Mikus materials.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            int? ProcessInput(object o)
            {
                // Pouziti is-patternu, ktery byl predstaven na druhem cviceni 
                return o is int i || o is string s && int.TryParse(s, out i)
                    ? (int?)i
                    : null;
            }

            Console.WriteLine("Enter whole number:");
            var input = Console.ReadLine();
            var parsedInput2 = ProcessInput(input);

            //Obecne switch muze byt v C# 7.0 pouzit na jakemkoliv typu
            switch (parsedInput2)
            {
                // when-pattern umoznuje pridani dodatecnych podminek do case vetve
                case int i when i % 5 == 0 && i % 3 == 0:
                    Console.WriteLine("FizzBuzz");
                    break;
                // na poradi jednotlivych case vetvi zalezi, podobne jako u catch bloku
                case int i when i % 3 == 0:
                    Console.WriteLine("Fizz");
                    break;
                // promenne 'i' ve when maji platnost vzdy pouze v ramci dane vetve
                case int i when i % 5 == 0:
                    Console.WriteLine("Buzz");
                    break;
                // vetev default se vyhodnocuje vzdy jako posledni
                default:
                    Console.WriteLine(parsedInput2);
                    break;
                // podminka na null se vyhodnocuje prednostne jako prvni 
                case null:
                    Console.WriteLine("Null");
                    break;
            }

            // Solution.TestSolution();

            // Uloha:
            // Implementujte nize uvedenou metodu AnalyzeCarPerformance(...), 
            // ktera bere jako parametr instanci tridy Car a pomoci Pattern 
            // Matchingu pak vypise zpravu na zaklade nasledujicich podminek:
            //
            // I.   Pokud se jedna o PersonalCar, s hodnotou zrychleni (0 -> 100) 
            //      mensi nez 7 vterin vypiste zpravu: "Given personal car can accelerate quickly"
            //
            // II.  Pokud se jedna o SportsCar, s hodnotou zrychleni (0 -> 100) 
            //      mensi nez 4 vteriny vypiste zpravu: "Given sports car can accelerate very fast"
            //
            // III. Pokud je splnena podminka (II.) a vozidlo je navic vybaveno systemem Launch Control
            //      (tedy property HasLaunchControl je nastavena na true), pripojte k vypisu zpravu:
            //      " and is equiped with launch control system"
            //
            // IV.  Ve vsech ostatnich pripadech vypiste: "Given car has rather average acceleration",
            //      pokud instance tridy Car bude nastavena na null, vypiste: "N/A"

        }
    }
}
