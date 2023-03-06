namespace Arrays
{
    /// <summary>
    /// I. uvedte o jaky typ / jake typy pole
    /// se jedna (pravidelne, vicerozmerne, ...)
    /// a urcete pocet dimenzi pole
    /// 
    /// II. kazde z uvedenych poli inicializujte
    /// nahodne generovanymy cisly od 0 do 100
    /// </summary>
    public class Arrays
    {
        private int[,,] array01 = new int[5, 10, 15];

        private int[][][] array02 = new int[5][][];

        private int[][,] array03 = new int[5][,];

        private int[,][,] array04 = new int[5, 10][,];

        public void InitializeArrays()
        {
            // vytvoreni generatoru (pseudo)nahodnych cisel,
            // pro generovani lze pouzit metodu Next(...)
            var random = new Random();

            //TODO: inicializace poli



        }
    }
}
