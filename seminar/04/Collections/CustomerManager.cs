namespace Collections
{
    /// <summary>
    /// Class handling customers with their orders.
    /// </summary>
    public class CustomerManager
    {
        // todo: 
        // deklarace (a inicializace) vhodne kolekce
        // pro ulozeni zakazniku s jejich objednavkami

        // ...

        /// <summary>
        /// Add customer.
        /// </summary>
        /// <param name="customer">Customer.</param>
        public void AddCustomer(Customer customer)
        {
            // todo: 
            // prida zakaznika do kolekce, nezapomente zkontrolovat duplicitu

            // ...
        }


        /// <summary>
        /// Store given orders to a given customer. 
        /// </summary>
        /// <param name="orders">Orders.</param>
        /// <param name="customer">Customer that should get the orders.</param>
        public void StoreOrders(Order[] orders, Customer customer)
        {
            // todo: 
            // prida objednavky pro daneho zakaznika

            // ...
        }


        /// <summary>
        /// Print out all data.
        /// </summary>
        public void WriteOutAllData()
        {
            // todo: 
            // vypise vsechna ulozena data, pro vypis
            // muzete zvolit libovolny format
            
            // ...
        }
    }
}
