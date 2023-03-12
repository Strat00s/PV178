namespace Collections
{
    // Ukol:
    // 
    // I. implementace tridy CustomerManager
    // 
    // II. overeni zda program po spusteni vypisuje nize uvedena testovaci data
    class Program
    {
        static void Main(string[] args)
        {
            Test();
        }

        static void Test()
        {
            var manager = new CustomerManager();
            var firstCustomer = new Customer("Tomas Pouzar");
            var secondCustomer = new Customer("Franta Konyvka");
            var thirdCustomer = new Customer("Anna Odstrcilova");

            manager.AddCustomer(firstCustomer);
            manager.AddCustomer(secondCustomer);
            manager.AddCustomer(thirdCustomer);

            manager.StoreOrders(new[]
            {
                new Order(3, "Zlate polomacene"),
                new Order(1, "Bohemia bramburky"),
            },
            firstCustomer);

            manager.StoreOrders(new[]
            {
                new Order(10, "Branik 2l"),
            },
            secondCustomer);

            manager.StoreOrders(new[]
            {
                new Order(2, "Tic Tac"),
                new Order(1, "Mentos freshmint")
            },
            thirdCustomer);

            manager.WriteOutAllData();

            Console.ReadKey();
        }
    }
}
