namespace Collections
{
    /// <summary>
    /// Represents order, each customer can have multiple orders.
    /// </summary>
    public class Order
    {
        public int Amount { get; private set; }

        public string GoodsDescription { get; private set; }

        public Order(int amount, string goodsDescription)
        {
            Amount = amount;
            GoodsDescription = goodsDescription;
        }

        public override string ToString()
        {
            return GoodsDescription + $" - {Amount}x";
        }
    }
}
