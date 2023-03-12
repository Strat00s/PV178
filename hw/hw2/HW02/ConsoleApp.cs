using System.Linq;
using HW02.BussinessContext;
using HW02.BussinessContext.FileDatabase;
using HW02.BussinessContext.Services;
using HW02.Helpers;
namespace HW02
{
    internal class ConsoleApp
    {
        static readonly string[] TwoArgs  = {"delete-product", "add-category", "delete-category", "get-products-by-category"};
        
        public static void Run(CategoryService categoryService, ProductService productService)
        {
            while (true)
            {
                try
                {
                    string[] arguments = IOHelper.ReadLine(); //read
                    //do action
                    if (arguments.Length == 0) 
                    {
                        //log empty actions?
                    }

                    else if (arguments.Length == 1)
                    {
                        if (arguments[0] == "help")
                            IOHelper.PrintHelp();   //log
                        else if (arguments[0] == "exit")
                            return;                 //log
                        else if (arguments[0] == "list-products")
                            IOHelper.PrettyPrint(productService.Read());    //log
                        else if (arguments[0] == "list-categories")
                            IOHelper.PrettyPrint(productService.Read());    //log
                        else
                            ;//throw exception
                    }
                    else if (arguments.Length == 2)
                    {
                        if (arguments[0] == "delete-product")
                            productService.Delete(arguments[1]);
                        else if (arguments[0] == "add-category")

                        else arguments[0] == "delete-category")

                        else arguments[0] == "get-products-by-category")

                        else
                            ;//throw invalid operation exception
                    }
                    else if (arguments.Length == 3)
                    {
                        if (arguments[0] != "update-category")
                            ;//throw invalid operation exception
                    }
                    else if (arguments.Length == 4)
                    {
                        if (arguments[0] != "add-product")
                            ;//throw invalid operation exception
                    }
                    else if (arguments.Length == 5)
                    {
                        if (arguments[0] != "update-product")
                            ;//throw invalid operation exception
                    }
                    else
                    {
                        //log invalid argument count
                    }
                    
                    //write output
                }
                catch (Exception ex)
                {
                    //handle exception ExHandler.HandleException(ex);
                }
            }
        }
    }
}
