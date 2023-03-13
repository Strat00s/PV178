using HW02.BussinessContext;
using HW02.BussinessContext.FileDatabase;
using HW02.BussinessContext.Services;
using HW02.Helpers;

namespace HW02
{
    public class Program
    {
        public static void Main()
        {
            //TODO logger will also print :wesmart:
            //TODO: Initialize all clases here, when some dependency needed, insert object through constrcutor
            var categoryDB      = new CategoryDBContext();
            var productDB       = new ProductDBContext(categoryDB);
            var categoryService = new CategoryService(categoryDB);
            var productService  = new ProductService(productDB);
            
            Seeder.FillDB(categoryService, productService);

            Console.WriteLine("Hello eShop!");
            Console.WriteLine("Type 'help' to list possible commands and uses");
            ConsoleApp.Run(categoryService, productService);
            Console.WriteLine("Exiting...");
        }
    }
}
