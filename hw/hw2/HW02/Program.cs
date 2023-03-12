using HW02.BussinessContext;
using HW02.BussinessContext.FileDatabase;
using HW02.BussinessContext.Services;

namespace HW02
{
    public class Program
    {
        public static void Main()
        {
            var categoryDB      = new CategoryDBContext();
            var productDB       = new ProductDBContext(categoryDB);
            var categoryService = new CategoryService(categoryDB);
            var productService  = new ProductService(productDB);
            // TODO: Initialize all clases here, when some dependency needed, insert object through constrcutor
            Console.WriteLine("Hello Ehop!");
            Console.WriteLine("Type 'help' to list possible commands and uses");
            ConsoleApp.Run(categoryService, productService);
        }
    }
}
