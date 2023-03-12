using System.Linq;
using HW02.Helpers;
namespace HW02
{
    internal class ConsoleApp
    {
        static readonly string[] OneArg   = {"help", "exit", "list-products", "list-categories"};
        static readonly string[] TwoArgs  = {"delete-product", "add-category", "delete-category", "get-products-by-category"};
        //static readonly string[] FourArgs = {"add-product"};
        public static void Run()
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
                        if (!OneArg.Contains(arguments[0]))
                            ;//throw invalid operation exception

                    }
                    else if (arguments.Length == 2)
                    {
                        if (!TwoArgs.Contains(arguments[0]))
                            ;//throw invalid operation exception
                    }
                    else if (arguments.Length == 4)
                    {
                        if (arguments[0] != "add-product")
                            ;//throw invalid operation exception

                    }
                    else
                    {
                        //log invalid argument count
                    }
                    
                    switch (arguments[0])
                    {
                        case "add-product":                
                            IOHelper.Print("not implemented yet\n");
                            /*name, categoryid, price*/
                            break;
                        case "delete-product":             
                            IOHelper.Print("not implemented yet\n");
                            /*productid*/
                            break;
                        case "list-product":               
                            IOHelper.Print("not implemented yet\n");
                            break;
                        case "add-category":               
                            IOHelper.Print("not implemented yet\n");
                            /*name*/
                            break;
                        case "delete-category":            
                            IOHelper.Print("not implemented yet\n");
                            /*categoryid*/
                            break;
                        case "list-categories":            
                            IOHelper.Print("not implemented yet\n");
                            break;
                        case "get-products-by-category":   
                            IOHelper.Print("not implemented yet\n");
                            /*categoryid*/
                            break;
                        case "help": 
                            IOHelper.PrintHelp();
                            break;
                        case "exit":
                            IOHelper.Print("not implemented yet\n");
                            break;
                        default:
                            /*throw*/
                            break;
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
