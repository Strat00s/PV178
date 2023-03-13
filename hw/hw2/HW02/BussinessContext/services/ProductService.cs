/* Product and category services are very similiar. Probably could've used some parent class and inheritance, but whatever. */

namespace HW02.BussinessContext.Services
{
    public class ProductService
    {
        private readonly ProductDBContext _db;
        private List<Product> _products;
        private int _lastId;


        //TODO implement it as a binary search
        private Product? FindProduct(int id)
        {
            for (int i = 0; i < _products.Count; i++)
            {
                if (id == _products[i].Id)
                    return _products[i];
            }
            return null;
        }

        //Constructor
        public ProductService(ProductDBContext db)
        {
            _db       = db;
            _products = _db.ReadProducts();
            _lastId   = 0;

            //get biggest id
            foreach (var product in _products)
            {
                if (_lastId < product.Id)
                    _lastId = product.Id;
            }
            //log operation
        }


        //Create new product
        public Product Create(string name, int categoryId, decimal price)
        {
            Product newProduct = new(++_lastId, name, categoryId, price);   //create new product with valid id
            _products.Add(newProduct);                                      //add the product
            _db.SaveProducts(_products);                                    //save it
            //log action
            return newProduct;
        }

        public List<Product> List()
        {
            return _products;
            //log list action
        }


        public List<Product> ListByCategory(int categoryId)
        {
            List<Product> products = new List<Product>();
            foreach (var product in _products)
            {
                if (product.Id == categoryId)
                    products.Add(product);
            }
            return products;
            //log list action
        }

        //Update product
        public Product Update(int id, string newName, int newCategoryId, decimal newPrice)
        {
            Product? product = FindProduct(id);
            if (product == null)
                return;//throw update failed

            product.Name       = newName;
            product.CategoryId = newCategoryId;
            product.Price      = newPrice;
            _db.SaveProducts(_products); //save
            //log action
            return product;
        }

        //Delete product
        public Product Delete(int id)
        {
            Product? product = FindProduct(id);
            if (product == null)
                return;//throw delete failed

            _products.Remove(product);
            _db.SaveProducts(_products);
            //log succesful deletion
            return product;
        }
    }
}
