/* Category class with required properties
 */

namespace HW02.BussinessContext
{
    public class Category
    {
        public int Id { get; }
        public string Name { get; set; }

        public Category(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
