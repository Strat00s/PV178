/* Product and category services are very similiar. Probably could've used some parent class and inheritance, but whatever. */

using HW02.BussinessContext.FileDatabase;
using HW02.Exceptions;

namespace HW02.BussinessContext.Services
{
    public class CategoryService
    {
        private readonly CategoryDBContext _db;
        private List<Category> _categories;
        private int _lastId;


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
        public CategoryService(CategoryDBContext db)
        { 
            _db = db;
            _categories = _db.ReadCategories();
            _lastId = 0;

            //get biggest id
            foreach (var product in _categories)
            {
                if (_lastId < product.Id)
                    _lastId = product.Id;
            }
            //log operation
        }


        //Create new product
        public Category Create(string name)
        {
            Category newCategory = new(++_lastId, name);    //create new category
            _categories.Add(newCategory);                   //add the product
            _db.SaveCategories(_categories);                //save
            //log operation
            return newCategory;
        }

        public List<Category> List()
        {
            return _categories;  //return the list of strings
            //log action
        }

        //Update category
        public Category Update(int categoryId, string newName)
        {
            Category? category = FindCategory(categoryId);
            if (category == null)
                throw new CategoryNotFoundException("Category with id '" + id + "' not found");

            category.Name = newName;
            _db.SaveCategories(_categories);
            //log operation
            return category;
        }

        public Category Delete(int id)
        {
            Category? category = FindCategory(id);
            if (category == null)
                throw new CategoryNotFoundException("Category with id '" + id + "' not found");

            _categories.Remove(category);
            _db.SaveCategories(_categories);
            //log operation
            return category;
        }
    }
}
