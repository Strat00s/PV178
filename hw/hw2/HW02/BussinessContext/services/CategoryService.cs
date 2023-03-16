/* Product and category services are very similiar. Probably could've used some parent class and inheritance, but whatever. */

using HW02.BussinessContext.FileDatabase;
using HW02.Exceptions;
using HW02.Helpers;

namespace HW02.BussinessContext.Services
{
    public class CategoryService
    {
        private readonly CategoryDBContext _db;
        private List<Category> _categories;

        private ProductService _productService;

        private EventHelper _eventHelper;

        private int _lastId;
       

        private Category? FindCategory(int id)
        {
            return _categories.Find(x => x.Id == id);
        }


        //Constructor
        public CategoryService(CategoryDBContext db, ProductService productService, EventHelper eventHelper)
        { 
            _db             = db;
            _categories     = _db.ReadCategories();
            _productService = productService;
            _eventHelper    = eventHelper;
            _lastId         = 0;

            //get biggest id
            foreach (var product in _categories)
            {
                if (_lastId < product.Id)
                    _lastId = product.Id;
            }

            eventHelper.Log(OpCode.NONE, true, null, "Category DB loaded");
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
            _productService.DeleteByCategory(categoryId);
            _categories.Remove(category);
            _db.SaveCategories(_categories);
            _eventHelper.Log(OpCode.DEL_CATG, true, category);
            return category;
        }
    }
}
