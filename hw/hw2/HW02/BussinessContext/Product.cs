/* Product class based on category with additional properties
 */

namespace HW02.BussinessContext
{
    public class Product : Category
    {
        public int CategoryId { get; set; }
        public decimal Price { get; set; }

        public Product(int id, string name, int categoryId, decimal price) : base(id, name)
        {
            CategoryId = categoryId;
            Price      = price;
        }
    }
}
