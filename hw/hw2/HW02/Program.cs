using HW02.BussinessContext;
using HW02.BussinessContext.FileDatabase;
using HW02.BussinessContext.Services;
using HW02.Helpers;
using HW02.LoggerContext.DB;

namespace HW02
{
    public class Program
    {
        public static void Main()
        {
            //TODO logger will also print :wesmart:
            //TODO: Initialize all clases here, when some dependency needed, insert object through constrcutor
            var loggerDB = new LoggerDBContext();
            LoggerListener logger = new LoggerListener(loggerDB);

            EventHelper eventHelper = new();
            eventHelper.LogEvent += logger.HandleEvent;

            var categoryDB      = new CategoryDBContext();
            var productDB       = new ProductDBContext(categoryDB);
            var categoryService = new CategoryService(categoryDB, eventHelper);
            var productService  = new ProductService(productDB, eventHelper);

            var inputParser         = new InputParser();
            
            //Seeder.FillDB(categoryService, productService);

            Console.WriteLine("Hello eShop!");
            Console.WriteLine("Type 'help' to list possible commands and uses");
            ConsoleApp.Run(categoryService, productService, inputParser, eventHelper);
            Console.WriteLine("Exiting...");
        }
    }
}
