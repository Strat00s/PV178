/* Product and category services are very similiar. Probably could've used some parent class and inheritance, but whatever. */

using HW02.BussinessContext.FileDatabase;
using HW02.Exceptions;
using HW02.Helpers;

namespace HW02.BussinessContext.Services
{
    public class CategoryService
    {
        private ProductService? _productService;            //product service used to delete all products when category is removed
        private readonly CategoryDBContext _db;             //DB context for read/write to/from file
        private int _lastId;                                //stores last category id

        public EventHandler<LogEventArgs>? LogEvent;

        //Constructor
        public CategoryService(CategoryDBContext db)
        {
            _productService = null;
            _db             = db;
            _lastId         = 0;
        }


        public void SetProductService(ProductService productService)
        {
            _productService = productService;
        }


        //used only in product service to check if category exists during update
        //might not be required as DBcontext checks that too...
        public Category? FindCategory(int id)
        {
            return _db.ReadCategories().Find(category => category.Id == id);
        }

        //Create new product
        public Category Create(string name)
        {
            var categories = _db.ReadCategories();                              //load it
            Category newCategory = new(++_lastId, name);                        //create new category
            categories.Add(newCategory);                                        //add category
            _db.SaveCategories(categories);                                     //save it
            LogEvent?.Invoke(this, new(OpCode.ADD_CATG, true, newCategory));    //log it
            return newCategory;
        }

        //get list of all categories
        public List<Category> List()
        {
            LogEvent?.Invoke(this, new(OpCode.LST_CATG, true)); //log it
            return _db.ReadCategories();                        //return the list of categories
        }

        //Update category
        public Category? Update(int categoryId, string newName)
        {
            var categories = _db.ReadCategories();  //load db

            //check if category exists
            var category = categories.Find(entity => entity.Id == categoryId);
            if (category == null)
            {
                LogEvent?.Invoke(this, new(OpCode.UPD_CATG, false, null, "Category with id '" + categoryId + "' not found"));
                return null;
            }
            category.Name = newName;                                        //update it 
            _db.SaveCategories(categories);                                 //save it
            LogEvent?.Invoke(this, new(OpCode.UPD_CATG, true, category));   //log it
            return category;
        }

        //delete category
        public Category? Delete(int categoryId)
        {
            var categories = _db.ReadCategories();  //load it
            
            //check if category exists and log if it doesn't
            var category = categories.Find(entity => entity.Id == categoryId);
            if (category == null)
            {
                LogEvent?.Invoke(this, new(OpCode.DEL_CATG, false, null, "Category with id '" + categoryId + "' not found"));
                return null;
            }
            _productService?.DeleteByCategory(categoryId);                  //remove all products
            categories.RemoveAll(entity => entity.Id == categoryId);        //remove category
            _db.SaveCategories(categories);                                 //save it
            LogEvent?.Invoke(this, new(OpCode.DEL_CATG, true, category));   //log it
            return category;
        }
    }
}
