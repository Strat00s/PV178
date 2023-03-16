/* Product and category services are very similiar. Probably could've used some parent class and inheritance, but whatever. */

using HW02.Exceptions;
using HW02.Helpers;

namespace HW02.BussinessContext.Services
{
    public class ProductService
    {
        private readonly ProductDBContext _db;
        private List<Product> _products;
        private int _lastId;
        private EventHelper _eventHelper;


        private Product? FindProduct(int id)
        {
           return _products.Find(x => x.Id == id);
        }

        //Constructor
        public ProductService(ProductDBContext db, EventHelper eventHelper)
        {
            _db       = db;
            _products = _db.ReadProducts();
            _lastId   = 0;
            _eventHelper = eventHelper;

            //get biggest id
            foreach (var product in _products)
            {
                if (_lastId < product.Id)
                    _lastId = product.Id;
            }

            _eventHelper.Log(OpCode.NONE, true, null, "Product DB loaded");
        }


        //Create new product
        public Product Create(string name, int categoryId, decimal price)
        {
            Product newProduct = new(++_lastId, name, categoryId, price);   //create new product with valid id
            _products.Add(newProduct);                                      //add the product
            _db.SaveProducts(_products);                                    //save it
            _eventHelper.Log(OpCode.ADD_PROD, true, newProduct);
            return newProduct;
        }

        public List<Product> List()
        {
            _eventHelper.Log(OpCode.LST_PROD, true);
            return _products;
        }


        public List<Product> ListByCategory(int categoryId)
        {
            List<Product> products = new List<Product>();
            foreach (var product in _products)
            {
                if (product.CategoryId == categoryId)
                    products.Add(product);
            }
            _eventHelper.Log(OpCode.LST_PROD, true);
            return products;
        }

        //Update product
        public Product Update(int productId, string newName, int newCategoryId, decimal newPrice)
        {
            Product? product = FindProduct(productId) ?? throw new EntityNotFound(OpCode.UPD_PROD, productId);
            product.Name       = newName;
            product.CategoryId = newCategoryId;
            product.Price      = newPrice;
            _db.SaveProducts(_products); //save
            _eventHelper.Log(OpCode.UPD_PROD, true, product);
            return product;
        }

        //Delete product
        public Product Delete(int productId)
        {
            Product? product = FindProduct(productId) ?? throw new EntityNotFound(OpCode.DEL_PROD, productId);
            _products.Remove(product);
            _db.SaveProducts(_products);
            _eventHelper.Log(OpCode.DEL_PROD, true, product);
            return product;
        }

        //delete products by category; used only with delete-category
        //is logged together with delete-category
        public void DeleteByCategory(int categoryId)
        {
            _products.RemoveAll(product => product.CategoryId == categoryId);
            _db.SaveProducts(_products)
        }
    }
}
