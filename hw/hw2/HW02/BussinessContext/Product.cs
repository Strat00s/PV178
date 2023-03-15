
namespace HW02.BussinessContext
{
    public class Product : Category
    {
        private int _categoryId;
        private decimal _price;

        public int CategoryId
        {
            get 
            { 
                return _categoryId; 
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("CategoryId must be a positive integer");
                _categoryId = value;
            }
        }

        public decimal Price
        {
            get { return _price; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Price must be a positive decimal");
                }
                _price = value;
            }
        }

        public Product(int id, string name, int categoryId, decimal price) : base(id, name)
        {
            _categoryId = categoryId;
            _price = price;
        }
    }
}
