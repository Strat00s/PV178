/* Product and category services are very similiar. Probably could've used some parent class and inheritance, but whatever. */

using HW02.Exceptions;
using HW02.Helpers;

namespace HW02.BussinessContext.Services
{
    public class ProductService
    {
        private CategoryService? _categoryService;

        private readonly ProductDBContext _db;
        private readonly EventHelper _eventHelper;
        private int _lastId;


        //Constructor
        public ProductService(ProductDBContext db, EventHelper eventHelper)
        {
            _categoryService = null;
            _db              = db;
            _eventHelper     = eventHelper;
            _lastId          = 0;

            _eventHelper.Log(OpCode.NONE, true, null, "Product DB loaded");
        }


        public void SetCategoryService(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        //create new product
        public Product Create(string name, int categoryId, decimal price)
        {
            //check if category does exist; not required as DBContext does that
            if (_categoryService?.FindCategory(categoryId) == null)
                throw new EntityNotFound(OpCode.ADD_PROD, categoryId, true);

            Product newProduct = new(++_lastId, name, categoryId, price);   //create new product
            
            //add, save
            var products = _db.ReadProducts();
            products.Add(newProduct);
            _db.SaveProducts(products);

            _eventHelper.Log(OpCode.ADD_PROD, true, newProduct);    //log it
            return newProduct;
        }

        //get list of all products
        public List<Product> List()
        {
            _eventHelper.Log(OpCode.LST_PROD, true);
            return _db.ReadProducts();
        }

        //get list of all products in category
        public List<Product> ListByCategory(int categoryId)
        {
            //check if category exists; not required as DBContext does that
            if (_categoryService?.FindCategory(categoryId) == null)
                throw new EntityNotFound(OpCode.LST_PROD, categoryId, true);

            _eventHelper.Log(OpCode.LST_PROD, true);
            return _db.ReadProducts().FindAll(product => product.CategoryId == categoryId);
        }

        //update product
        public Product Update(int productId, string newName, int newCategoryId, decimal newPrice)
        {
            //check if category exists; not required as DBContext does that
            if (_categoryService?.FindCategory(newCategoryId) == null)
                throw new EntityNotFound(OpCode.UPD_PROD, newCategoryId, true);
            
            var products       = _db.ReadProducts();
            var product        = products.Find(entity => entity.Id == productId) ?? throw new EntityNotFound(OpCode.UPD_PROD, productId, false);    //check if product exists
            product.Name       = newName;                                                                                                           //update it
            product.CategoryId = newCategoryId;
            product.Price      = newPrice;
            _db.SaveProducts(products);                                                                                                             //save it
            _eventHelper.Log(OpCode.UPD_PROD, true, product);                                                                                       //log it
            return product;
        }

        //delete product
        public Product Delete(int productId)
        {
            var products = _db.ReadProducts();
            var product  = products.Find(entity => entity.Id == productId) ?? throw new EntityNotFound(OpCode.DEL_PROD, productId, false);  //check if product exists
            products.RemoveAll(entity => entity.Id == productId);                                                                           //remove it
            _db.SaveProducts(products);                                                                                                     //save it
            _eventHelper.Log(OpCode.DEL_PROD, true, product);                                                                               //log it
            return product;
        }

        //delete products by category; used only with delete-category
        //is logged together with delete-category
        public void DeleteByCategory(int categoryId)
        {
            var products = _db.ReadProducts();
            products.RemoveAll(product => product.CategoryId == categoryId);
            _db.SaveProducts(products);
        }
    }
}
