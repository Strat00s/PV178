using HW02.BussinessContext;

namespace HW02.Helpers
{
    public interface IOHelper
    {
        //public static string[] ReadLine()
        public static string ReadLine()
        {
            Console.Write("Command: ");
            //return Console.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();  //split input by spaces if there is any, otherwise return empty array
            return Console.ReadLine() ?? "";
        }

        public static void Write(string message)
        {
            Console.Write(message);
        }

        public static void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public static void PrintTable<T>(List<T> entities) where T : Category
        {
            List<int> maxLengths = new List<int>() { 2, 4, 10, 5 }; //default field lengths

            //find lengths of every value
            foreach (T entity in entities)
            {
                maxLengths[0] = Math.Max(maxLengths[0], entity.Id.ToString().Length);
                maxLengths[1] = Math.Max(maxLengths[1], entity.Name.Length);
                //extra fields for product
                if (entity is Product product)
                {
                    maxLengths[2] = Math.Max(maxLengths[2], product.CategoryId.ToString().Length);
                    maxLengths[3] = Math.Max(maxLengths[3], product.Price.ToString().Length);
                }
            }

            //print the table
            Console.Write("Id".PadRight(maxLengths[0]));
            Console.Write(" | " + "Name".PadRight(maxLengths[1]));
            Console.Write(" | " + "CategoryId".PadRight(maxLengths[2]));
            Console.WriteLine(" | Price");
            Console.WriteLine(new string('-', maxLengths.Sum() + 3 * 3));

            foreach (var entity in entities) {
                Console.Write(entity.Id.ToString().PadLeft(maxLengths[0]));
                Console.Write(" | " + entity.Name.PadRight(maxLengths[1]));
                if (entity is Product product)
                {
                    Console.Write(" | " + product.CategoryId.ToString().PadLeft(maxLengths[2]));
                    Console.WriteLine(" | " + product.Price.ToString().PadLeft(maxLengths[3]));
                }
            }
            Console.WriteLine();
        }

        public static void PrintHelp()
        {
            Console.Write(
                "Possible commands:\n" +
                "  add-product    <Name> <CategoryId> <Price>                         Add product <Name> with price <Price> to category <CategoryId> to DB\n" +
                "  update-product <ProductId> <newName> <newCategoryId> <newPrice>    Update product <ProductId>\n" +
                "  delete-product <ProductId>                                         Remove product <ProductId> from DB\n" +
                "  list-products                                                      List all products\n" +
                "\n" +                                           
                "  add-category <Name>                                                Add category <Name>\n" +
                "  update-category <CategoryId> <newName>                             Update category <CategoryId>\n" +
                "  delete-category <CategoryId>                                       Delete category <CategoryId>\n" +
                "  list-categories                                                    List all categories\n" +
                "\n" +
                "  get-products-by-category <CategoryId>                              List all products in category <CategoryId>\n" +
                "\n" +
                "  help                                                               Print this help message\n" +
                "  exit                                                               Exit the application\n"
                );
        }
    }
}
