//TODO change iohelper to event based printing???

using System.Linq;
using HW02.BussinessContext;
using HW02.BussinessContext.FileDatabase;
using HW02.BussinessContext.Services;
using HW02.Helpers;
namespace HW02
{
    internal class ConsoleApp
    {
        
        public static void Run(CategoryService categoryService, ProductService productService)
        {
            while (true)
            {
                try
                {
                    /*
                    string[] arguments = IOHelper.ReadLine(); //read
                    //do action
                    if (arguments.Length == 0) 
                    {
                        //log empty actions?
                    }

                    else if (arguments.Length == 1)
                    {
                        if (arguments[0] == "exit")
                            return;                                             //log
                        else if (arguments[0] == "help")
                            IOHelper.PrintHelp();                               //log
                        else if (arguments[0] == "list-products")
                            IOHelper.PrintProducts(productService.List());      //log
                        else if (arguments[0] == "list-categories")
                            IOHelper.PrintCategories(categoryService.List());   //log
                        else
                            ;//throw exception
                    }
                    else if (arguments.Length == 2)
                    {
                        if (arguments[0] == "delete-product")
                        {
                            productService.Delete(ParseHelper.ParseInt(arguments[1]));
                            //invoke event to log and print about deleted entity
                            IOHelper.WriteLine("Product deleted");
                        }
                        else if (arguments[0] == "delete-category")
                        {
                            categoryService.Delete(ParseHelper.ParseInt(arguments[1]));
                            IOHelper.WriteLine("Category deleted");
                        }
                        else if (arguments[0] == "add-category")
                        {
                            categoryService.Create(arguments[1]);
                            IOHelper.WriteLine("Category '" + arguments[1] + "' created");
                        }
                        else if (arguments[0] == "get-products-by-category")
                        {
                            IOHelper.PrintProducts(productService.ListByCategory(ParseHelper.ParseInt(arguments[1])));
                        }
                        else
                            ;//throw invalid operation exception
                    }
                    else if (arguments.Length == 3)
                    {
                        if (arguments[0] != "update-category")
                            ;//throw invalid operation exception
                        categoryService.Update(ParseHelper.ParseInt(arguments[1]), arguments[2]);
                        IOHelper.WriteLine("Category updated");
                    }
                    else if (arguments.Length == 4)
                    {
                        if (arguments[0] != "add-product")
                            ;//throw invalid operation exception
                        productService.Create(arguments[1], ParseHelper.ParseInt(arguments[2]), ParseHelper.ParseDec(arguments[3]));
                        IOHelper.WriteLine("Product '" + arguments[1] + "' added");
                    }
                    else if (arguments.Length == 5)
                    {
                        if (arguments[0] != "update-product")
                            ;//throw invalid operation exception
                        productService.Update(ParseHelper.ParseInt(arguments[1]), arguments[2], ParseHelper.ParseInt(arguments[3]), ParseHelper.ParseDec(arguments[4]));
                        IOHelper.WriteLine("Product updated");
                    }
                    else
                    {
                        //log invalid argument count
                    }
                    */
                    
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
