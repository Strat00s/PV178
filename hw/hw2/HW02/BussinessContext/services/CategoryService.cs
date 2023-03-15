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
        private int _lastId;
        private EventHelper _eventHelper;


        //TODO implement it as a binary search
        private Category? FindCategory(int id)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (id == _categories[i].Id)
                    return _categories[i];
            }
            return null;
        }

        //Constructor
        public CategoryService(CategoryDBContext db, EventHelper eventHelper)
        { 
            _db = db;
            _categories = _db.ReadCategories();
            _lastId = 0;
            _eventHelper = eventHelper;

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
            _categories.Remove(category);
            _db.SaveCategories(_categories);
            _eventHelper.Log(OpCode.DEL_CATG, true, category);
            return category;
        }
    }
}
