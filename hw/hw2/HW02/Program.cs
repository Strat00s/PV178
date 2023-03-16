using HW02.AnalyticalDataContext;
using HW02.AnalyticalDataContext.DB;
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
            //logger setup
            var loggerDB          = new LoggerDBContext();
            LoggerListener logger = new(loggerDB);

            //analytics setup
            var analyticalDB                 = new AnalyticalDBContext();
            AnalyticalDataListener analytics = new(analyticalDB);

            //IOHelper for reading and writing
            IOHelper ioHelper = new();

            //event setup
            EventHelper eventHelper = new();
            eventHelper.LogEvent += logger.HandleEvent;
            eventHelper.LogEvent += analytics.HandleEvent;

            //main services setup
            var categoryDB      = new CategoryDBContext();
            var productDB       = new ProductDBContext(categoryDB);

            //create services and reference each other
            var productService  = new ProductService(productDB, eventHelper);
            var categoryService = new CategoryService(categoryDB, eventHelper);
            categoryService.SetProductService(productService);
            productService.SetCategoryService(categoryService);


            //other
            var inputParser         = new InputParser();
            

            Console.WriteLine("Hello eShop!");
            Console.WriteLine("Type 'help' to list possible commands and uses");
            ConsoleApp.Run(categoryService, productService, ioHelper, inputParser, eventHelper);
            Console.WriteLine("Exiting...");
        }
    }
}
