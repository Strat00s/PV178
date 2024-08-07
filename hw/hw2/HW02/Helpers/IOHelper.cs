﻿/* Input and output class
 * Used for reading and writing from/to console
 */

using HW02.BussinessContext;

namespace HW02.Helpers
{
    public interface IOHelper
    {
        //read string from console
        public static string ReadLine()
        {
            Console.Write("Command: ");
            return Console.ReadLine() ?? "";
        }

        //write string to console
        public static void Write(string message)
        {
            Console.Write(message);
        }

        //write line to console
        public static void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        //pretty print table to console
        public static void PrintTable<T>(List<T> entities) where T : Category
        {
            List<int> maxLengths = new() { 2, 4, 10, 5 }; //default field lengths

            //find lengths of every value
            foreach (T entity in entities)
            {
                maxLengths[0] = Math.Max(maxLengths[0], entity.Id.ToString().Length);
                maxLengths[1] = Math.Max(maxLengths[1], entity.Name.Length);
                //extra fields for product
                if (entity is Product product)
                {
                    maxLengths[2] = Math.Max(maxLengths[2], product.CategoryId.ToString().Length);
                    maxLengths[3] = Math.Max(maxLengths[3], product.Price.ToString("0.00").Length);
                }
            }

            //print the table header (with extra fields for product)
            Console.Write("Id".PadRight(maxLengths[0]));
            Console.Write(" | " + "Name".PadRight(maxLengths[1]));
            if (typeof(T) == typeof(Product))
            {
                Console.Write(" | " + "CategoryId".PadRight(maxLengths[2]));
                Console.WriteLine(" | Price");
                Console.WriteLine(new string('-', maxLengths.Sum() + 3 * 3));
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine(new string('-', maxLengths[0] + maxLengths[1] + 3));
            }

            //print individual items
            foreach (var entity in entities) {
                Console.Write(entity.Id.ToString().PadLeft(maxLengths[0]));
                Console.Write(" | " + entity.Name.PadRight(maxLengths[1]));
                //extra fields for product
                if (entity is Product product)
                {
                    Console.Write(" | " + product.CategoryId.ToString().PadLeft(maxLengths[2]));
                    Console.Write(" | " + product.Price.ToString("0.00").PadLeft(maxLengths[3]));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        //print help to console
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

        //event handler for printing information to console (I already have all the information for logs, why not use it for console output as well?)
        public static void HandleEvent(Object? sender, LogEventArgs e)
        {
            string output = "";

            if (!e.Success)
            {
                output += "Operation failed: " + e.Message ?? "";
                Console.WriteLine(output);
                return;
            }

            //write appropriate output
            switch (e.OpCode)
            {
                case OpCode.EXIT:
                case OpCode.HELP:
                case OpCode.GET_BY_CATG:
                case OpCode.LST_CATG:
                case OpCode.LST_PROD: return;

                case OpCode.ADD_PROD: output += "Product '" + e.Entity?.Name + "' (" + e.Entity?.Id + ") added"; break;
                case OpCode.DEL_PROD: output += "Product '" + e.Entity?.Name + "' (" + e.Entity?.Id + ") deleted"; break;
                case OpCode.UPD_PROD: output += "Product '" + e.Entity?.Name + "' (" + e.Entity?.Id + ") updated"; break;

                case OpCode.ADD_CATG: output += "Category '" + e.Entity?.Name + "' (" + e.Entity?.Id + ") added"; break;
                case OpCode.DEL_CATG: output += "Category '" + e.Entity?.Name + "' (" + e.Entity?.Id + ") deleted"; break;
                case OpCode.UPD_CATG: output += "Category '" + e.Entity?.Name + "' (" + e.Entity?.Id + ") updated"; break;

                default: output += "An error occured: " + e.Message ?? ""; break;
            }

            Console.WriteLine(output);
        }
    }
}
