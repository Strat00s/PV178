//TODO change iohelper to event based printing???

using System.Linq;
using HW02.BussinessContext;
using HW02.BussinessContext.FileDatabase;
using HW02.BussinessContext.Services;
using HW02.Helpers;
namespace HW02
{
    public enum OpCode
    {
        EXIT,
        HELP,

        GET_BY_CATG,

        ADD_PROD,
        UPD_PROD,
        DEL_PROD,
        LST_PROD,

        ADD_CATG,
        UPD_CATG,
        DEL_CATG,
        LST_CATG,

        NONE = 99,
        OP_ERR,
        CNT_ERR
    }
    internal class ConsoleApp
    {
        private static OpCode _opCode;
        public static void Run(CategoryService categoryService, ProductService productService, InputParser inputParser)
        {
            while (true)
            {
                try
                {
                    _opCode = inputParser.Parse(IOHelper.ReadLine());
                    //I know that there is duplicit code. I just wanted to have the input parsing seperated from the main logic
                    switch (_opCode)
                    {
                        case OpCode.EXIT: return;
                        case OpCode.HELP: IOHelper.PrintHelp(); break;
                        case OpCode.GET_BY_CATG: IOHelper.PrintProducts(productService.ListByCategory(inputParser.CId)); break ;
                        case OpCode.ADD_PROD: productService.Create(inputParser.Name, inputParser.CId, inputParser.Price); break;
                        case OpCode.UPD_PROD: productService.Update(inputParser.PId, inputParser.Name, inputParser.CId, inputParser.Price); break;
                        case OpCode.DEL_PROD: productService.Delete(inputParser.PId); break;
                        case OpCode.LST_PROD: IOHelper.PrintProducts(productService.List()); break;
                        case OpCode.ADD_CATG: categoryService.Create(inputParser.Name); break;
                        case OpCode.UPD_CATG: categoryService.Update(inputParser.CId, inputParser.Name); break;
                        case OpCode.DEL_CATG: categoryService.Delete(inputParser.CId); break;
                        case OpCode.LST_CATG: IOHelper.PrintCategories(categoryService.List()); break;
                    }
                    //write output
                }
                catch (Exception ex)
                {
                    //handle exception ExHandler.HandleException(ex);
                    IOHelper.WriteLine(nameof(ex));
                }
            }
        }
    }
}
