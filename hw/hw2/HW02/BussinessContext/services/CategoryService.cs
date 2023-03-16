/* Product and category services are very similiar. Probably could've used some parent class and inheritance, but whatever. */

using HW02.BussinessContext.FileDatabase;
using HW02.Exceptions;
using HW02.Helpers;

namespace HW02.BussinessContext.Services
{
    public class CategoryService
    {
        //lazy init factory
        private ProductService? _productService;

        private readonly CategoryDBContext _db; //DB handler
        private readonly List<Category> _categories;     //current list of categories
        private readonly EventHelper _eventHelper;       //used for logging events
        private int _lastId;                    //stores last entity id
       

        public Category? FindCategory(int id)
        {
            return _categories.Find(x => x.Id == id);
        }


        //Constructor
        public CategoryService(CategoryDBContext db, EventHelper eventHelper)
        {
            _productService = null;

            _db          = db;
            _categories  = _db.ReadCategories();
            _eventHelper = eventHelper;
            _lastId      = 0;

            //get biggest id
            foreach (var product in _categories)
            {
                if (_lastId < product.Id)
                    _lastId = product.Id;
            }

            eventHelper.Log(OpCode.NONE, true, null, "Category DB loaded");
        }

        //
        public void SetProductService(ProductService productService)
        {
            _productService = productService;
        }


        //Create new product
        public Category Create(string name)
        {
            Category newCategory = new(++_lastId, name);    //create new category
            _categories.Add(newCategory);                   //add the product
            _db.SaveCategories(_categories);                //save
            _eventHelper.Log(OpCode.ADD_CATG, true, newCategory);
            return newCategory;
        }

        public List<Category> List()
        {
            _eventHelper.Log(OpCode.LST_CATG, true);
            return _categories;  //return the list of strings
        }

        //Update category
        public Category Update(int categoryId, string newName)
        {
            Category? category = FindCategory(categoryId) ?? throw new EntityNotFound(OpCode.UPD_CATG, categoryId);
            category.Name = newName;
            _db.SaveCategories(_categories);
            _eventHelper.Log(OpCode.UPD_CATG, true, category);
            return category;
        }

        public Category Delete(int categoryId)
        {
            Category? category = FindCategory(categoryId) ?? throw new EntityNotFound(OpCode.DEL_CATG, categoryId);
            _productService?.DeleteByCategory(categoryId);
            _categories.Remove(category);
            _db.SaveCategories(_categories);
            _eventHelper.Log(OpCode.DEL_CATG, true, category);
            return category;
        }
    }
}
