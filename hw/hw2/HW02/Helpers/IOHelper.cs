using HW02.BussinessContext;

namespace HW02.Helpers
{
    internal interface IOHelper
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

        public static void PrintProducts(List<Product> products)
        {
            Console.WriteLine("Id | Name | CategoryId | Price\n");
            Console.WriteLine("-------------------------------------\n");
            foreach (var product in products)
                Console.WriteLine(product.Id + " | " + product.Name + " | " + product.CategoryId + " | " + product.Price);
        }

        public static void PrintCategories(List<Category> categories)
        {
            Console.WriteLine("Id | Name\n");
            Console.WriteLine("------------------n");
            foreach (var category in categories)
                Console.WriteLine(category.Id + " | " + category.Name);
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
