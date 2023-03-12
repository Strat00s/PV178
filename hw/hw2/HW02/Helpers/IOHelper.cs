namespace HW02.Helpers
{
    internal interface IOHelper
    {
        public static string[] ReadLine()
        {
            Console.Write("Command: ");
            return Console.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();  //split input by spaces if there is any, otherwise return empty array
        }

        public static void Print(string message)
        {
            Console.Write(message);
        }

        public static void PrettyPrint(string[] lines)
        {
            Console.Write(lines[0]);
        }

        public static void PrintHelp()
        {
            Console.Write(
                "Possible commands:\n" +
                "  add-product    <Name> <CategoryId> <Price>    Add product <Name> with price <Price> to category <CategoryId> to DB\n" +
                "  delete-product <ProductId>                    Remove product <ProductId> from DB\n" +
                "  list-products                                 List all products\n" +
                "\n" +                                           
                "  add-category <Name>                           Add category <Name>\n" +
                "  delete-category <CategoryId>                  Delete category <CategoryId>\n" +
                "  list-categories                               List all categories\n" +
                "  get-products-by-category <CategoryId>         List all products in category <CategoryId>\n" +
                "\n" +
                "  help                                          Print this help message\n" +
                "  exit                                          Exit the application\n"
                );
        }

        public static string[] ParseArguments(string input)
        {
            return input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
