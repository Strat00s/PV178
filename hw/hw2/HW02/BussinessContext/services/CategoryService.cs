using HW02.BussinessContext.FileDatabase;

namespace HW02.BussinessContext.Services
{
    public class CategoryService
    {
        private readonly CategoryDBContext _db;
        private List<Category> _categories;

        public CategoryService(CategoryDBContext db)
        { 
            _db = db;
            _categories = _db.ReadCategories();
            //log operation
        }

        public void Load()
        {
            _categories = _db.ReadCategories();
            //log operation
        }

        public void Create(string name)
        {
            Category newCategory;

            //increase category id if there are any categories, otherwise add first category
            if (_categories.Any())
                newCategory = new Category(_categories.Last().Id + 1, name);
            else
                newCategory = new Category(1, name);

            _categories.Add(newCategory);       //add the product
            _db.SaveCategories(_categories);    //save
            //log operation
        }

        public List<List<String>> Read()
        {
            var result = new List<List<String>>();
            foreach (var category in _categories)
                result.Add(new List<String>() {category.Id.ToString(), category.Name});

            return result;  //return the list of strings
            //log action
        }

        public void Update(int categoryId, string newName)
        {            
            for (int i = 0; i < _categories.Count; i++)
            {
                if (_categories[i].Id == categoryId)
                {
                    _categories[i].Name = newName;
                    _db.SaveCategories(_categories);
                    //log operation
                    return;
                }
            }
            //throw error that item was not found and update failed
        }

        public void Delete(int id)
        {
            for (int i = 0; i < _categories.Count; i++)
            {
                if (_categories[i].Id == id) 
                {
                    _categories.RemoveAt(i);
                    _db.SaveCategories(_categories);
                    //log operation
                    break;
                }
            }
            //throw error that item was not found and update failed
        }
    }
}
