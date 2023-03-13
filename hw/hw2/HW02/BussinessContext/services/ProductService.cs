

using HW02.Helpers;

namespace HW02.BussinessContext.Services
{
    internal class ProductService
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
        public void Create(string name, int categoryId, decimal price)
        {
            Product newProduct = new(++_lastId, name, categoryId, price);   //create new product with valid id
            _products.Add(newProduct);                                      //add the product
            _db.SaveProducts(_products);                                    //save it
            //log action
        }

        //TODO
        public List<string> Read(string id)
        {
            Product? product = FindProduct(ParseHelper.ParseInt(id));
            return new List<string> {product.Id.ToString(), product.Name, product.CategoryId.ToString(), product.Price.ToString()};
            //log list action
        }

        //Update product
        public void Update(string id, string newName, string newCategoryId, string newPrice)
        {
            Product? product = FindProduct(ParseHelper.ParseInt(id));
            if (product == null)
                ;//throw update failed

            product.Name       = newName;
            product.CategoryId = ParseHelper.ParseInt(newCategoryId);
            product.Price      = ParseHelper.ParseDec(newPrice);
            _db.SaveProducts(_products); //save
            return;
            //log action
        }

        //Delete product
        public void Delete(string id)
        {
            Product? product = FindProduct(ParseHelper.ParseInt(id));
            if (product == null)
                return;//throw delete failed

            _products.Remove(product);
            _db.SaveProducts(_products);
            //log succesful deletion
            return;    
        }
    }
}
