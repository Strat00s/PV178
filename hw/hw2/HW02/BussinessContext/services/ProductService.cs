

namespace HW02.BussinessContext.Services
{
    internal class ProductService
    {
        private readonly ProductDBContext _db;
        private List<Product> _products;

        public ProductService(ProductDBContext db)
        {
            _db = db;
            _products = _db.ReadProducts();
            //log operation
        }

        public void Load()
        {
            _products = _db.ReadProducts();
            //log operation
        }

        //TODO tryparse everywhere
        //TODO check if category exists
        public void Create(string name, int categoryId, decimal price)
        {
            Product newProduct;
            
            //increase product id if there is any, otherwise add first product
            if (_products.Any())
                newProduct = new Product(_products.Last().Id + 1, name, categoryId, price);
            else
                newProduct = new Product(1, name, categoryId, price);

            _products.Add(newProduct);   //add the product
            _db.SaveProducts(_products); //save it
            //log action
        }

        public List<List<String>> Read()
        {
            var result = new List<List<String>>();
            foreach (var product in _products)
                result.Add(new List<String>() { product.Id.ToString(), product.Name, product.CategoryId.ToString(), product.Price.ToString() });
            
            return result;  //return the list of strings
            //log action
        }

        //TODO check if category exists
        public void Update(string id, string newName, string newCategoryId, string newPrice)
        {
            int productId;
            int categoryId;
            decimal price;
            if (!Int32.TryParse(id, out productId) || !Int32.TryParse(newCategoryId, out categoryId) || !Decimal.TryParse(newPrice, out price))
                return;//throw syntax error
            //go through all products and search for one with matching id
            for (int i = 0; i < _products.Count; i++)
            {
                //update data on match
                if (_products[i].Id == productId)
                {
                    //log item found
                    _products[i].Name = newName;
                    _products[i].CategoryId = categoryId;
                    _products[i].Price = price;
                    _db.SaveProducts(_products); //save
                    //log action
                    return;
                }
            }
            //throw error that item was not found and update failed
        }

        public void Delete(string id)
        {
            int productId;
            if (!Int32.TryParse(id, out productId))
                return;//throw syntax error
            for (int i = 0; i < _products.Count; i++)
            {
                if (_products[i].Id == productId)
                {
                    _products.RemoveAt(i);
                    _db.SaveProducts(_products);
                    //log succesful deletion
                    break;
                }
            }
            //throw error that item was not found and deletion failed
        }
    }
}
