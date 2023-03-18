/* Product and category services are very similiar. Probably could've used some parent class and inheritance, but whatever.
 */

using HW02.Exceptions;
using HW02.Helpers;

namespace HW02.BussinessContext.Services
{
    public class ProductService
    {
        private CategoryService? _categoryService;          //category service for checking if category exists
        private readonly ProductDBContext _db;              //DB context for reading and writing
        private readonly EventPublisher _eventPublisher;    //event publisher
        private int _lastId;                                //id assigned to last category


        //Constructor
        public ProductService(ProductDBContext db, EventPublisher eventPublisher)
        {
            _categoryService = null;
            _db              = db;
            _eventPublisher  = eventPublisher;
            _lastId          = 0;

            _eventPublisher.Log(new(OpCode.NONE, true, null, "Product DB loaded"));
        }


        public void SetCategoryService(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        //create new product
        public Product Create(string name, int categoryId, decimal price)
        {
            //check if category does exist
            if (_categoryService?.FindCategory(categoryId) == null)
                throw new EntityNotFound(OpCode.ADD_PROD, categoryId, true);

            Product newProduct = new(++_lastId, name, categoryId, price);   //create new product
            var products = _db.ReadProducts();                              //load db
            products.Add(newProduct);                                       //add product
            _db.SaveProducts(products);                                     //save it
            _eventPublisher.Log(new(OpCode.ADD_PROD, true, newProduct));    //log it
            return newProduct;
        }

        //get list of all products
        public List<Product> List()
        {
            _eventPublisher.Log(new(OpCode.LST_PROD, true));    //log it
            return _db.ReadProducts();                          //return the entiry list of products
        }

        //get list of all products in category
        public List<Product> ListByCategory(int categoryId)
        {
            //check if category exists
            if (_categoryService?.FindCategory(categoryId) == null)
                throw new EntityNotFound(OpCode.LST_PROD, categoryId, true);

            _eventPublisher.Log(new(OpCode.LST_PROD, true));                                //log it
            return _db.ReadProducts().FindAll(product => product.CategoryId == categoryId); //return the list of products
        }

        //update product
        public Product Update(int productId, string newName, int newCategoryId, decimal newPrice)
        {
            //check if category exists; not required as DBContext does that
            if (_categoryService?.FindCategory(newCategoryId) == null)
                throw new EntityNotFound(OpCode.UPD_PROD, newCategoryId, true);
            
            var products       = _db.ReadProducts();                                                                                                //load db
            var product        = products.Find(entity => entity.Id == productId) ?? throw new EntityNotFound(OpCode.UPD_PROD, productId, false);    //check if product exists
            product.Name       = newName;                                                                                                           //update it
            product.CategoryId = newCategoryId;
            product.Price      = newPrice;
            _db.SaveProducts(products);                                                                                                             //save it
            _eventPublisher.Log(new(OpCode.UPD_PROD, true, product));                                                                               //log it
            return product;
        }

        //delete product
        public Product Delete(int productId)
        {
            var products = _db.ReadProducts();                                                                                              //load db
            var product  = products.Find(entity => entity.Id == productId) ?? throw new EntityNotFound(OpCode.DEL_PROD, productId, false);  //check if product exists
            products.RemoveAll(entity => entity.Id == productId);                                                                           //remove it
            _db.SaveProducts(products);                                                                                                     //save it
            _eventPublisher.Log(new(OpCode.DEL_PROD, true, product));                                                                       //log it
            return product;
        }

        //delete products by category; used only with delete-category
        //is logged together with delete-category
        public void DeleteByCategory(int categoryId)
        {
            var products = _db.ReadProducts();                                  //load db
            products.RemoveAll(product => product.CategoryId == categoryId);    //remove all products from category
            _db.SaveProducts(products);                                         //save it
        }
    }
}
