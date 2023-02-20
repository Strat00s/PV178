namespace TestingClass_03
{
    
    public static class Factorial
    {
        /// <summary>
        /// Metoda pro výpočet faktoriálu. Vstup musí být nezáporný.
        /// </summary>
        /// <param name="x">Vstup.</param>
        /// <returns>Faktoriál čísla x.</returns>
        public static int FactorialGood(int x)
        {
            //TODO: Vyskoušejte si breakpointy a sledujte změnu v proměnných krok po kroku.
            if (x < 0) { throw new ArgumentException(); }
            int output = 1;
            for (int i = 1; i <= x; i++)
            {
                output *= i;
            }
            return output;
        }

        //Níže jsou špatně napsané metody faktoriálů, pro demonstraci unit testů.
        //Tato metoda nevyhazuje vyjímku při záporných hodnotách.
        public static int FactorialBadExceptionless(int x)
        {
            int output = 1;
            for (int i = 1; i <= x; i++)
            {
                output *= i;
            }
            return output;
        }

        //Tato metoda se zacyklí.
        public static int FactorialBadInfinite(int x)
        {
            int output = 1;
            for (int i = 1; true; i++)
            {
                output *= i;
            }
            return output;
        }
    }
}