﻿using HW02.AnalyticalDataContext;
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

            //main services setup
            var categoryDB = new CategoryDBContext();
            var productDB  = new ProductDBContext(categoryDB);

            //create services and reference each other
            var productService  = new ProductService(productDB);
            var categoryService = new CategoryService(categoryDB);
            categoryService.SetProductService(productService);
            productService.SetCategoryService(categoryService);

            //input parser and main app logic
            var inputParser = new InputParser();
            var app         = new ConsoleApp(categoryService, productService, inputParser);

            //subscribe to events
            categoryService.LogEvent += logger.HandleEvent;
            categoryService.LogEvent += analytics.HandleEvent;
            productService.LogEvent  += logger.HandleEvent;
            productService.LogEvent  += analytics.HandleEvent;
            app.LogEvent             += logger.HandleEvent; //just logger as the main app handles only exceptions which are useless for analytics

            //fill db
            Seeder.FillDB(categoryService, productService);

            //run the app
            Console.WriteLine("Hello eShop!");
            Console.WriteLine("Type 'help' to list possible commands and uses");
            app.Run();
            Console.WriteLine("Exiting...");
        }
    }
}
