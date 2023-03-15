//TODO change iohelper to event based printing???

using System.Linq;
using HW02.BussinessContext;
using HW02.BussinessContext.FileDatabase;
using HW02.BussinessContext.Services;
using HW02.Exceptions;
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
    public static class ConsoleApp
    {
        //private static OpCode opCode;
        
        public static void Run(CategoryService categoryService, ProductService productService, InputParser inputParser, EventHelper eventHelper)
        {
            int firstRun     = 0;
            OpCode opCode    = OpCode.NONE;

            while (true)
            {
                opCode = OpCode.NONE;
                try
                {
                    if (firstRun == 0)
                    {
                        firstRun++;
                        Seeder.FillDB(categoryService, productService);
                        firstRun++;
                    }
                    else if (firstRun == 1)
                        return;

                    opCode = inputParser.Parse(IOHelper.ReadLine());
                    //I know that this is kinda duplicit code. I just wanted to have the input parsing seperated from the main logic
                    switch (opCode)
                    {
                        case OpCode.NONE:        continue;
                        case OpCode.EXIT:        return;
                        case OpCode.HELP:        IOHelper.PrintHelp();                                                                         break;
                        case OpCode.GET_BY_CATG: IOHelper.PrintProducts(productService.ListByCategory(inputParser.CId));                       break;
                        case OpCode.ADD_PROD:    productService.Create(inputParser.Name, inputParser.CId, inputParser.Price);                  break;
                        case OpCode.UPD_PROD:    productService.Update(inputParser.PId, inputParser.Name, inputParser.CId, inputParser.Price); break;
                        case OpCode.DEL_PROD:    productService.Delete(inputParser.PId);                                                       break;
                        case OpCode.LST_PROD:    IOHelper.PrintProducts(productService.List());                                                break;
                        case OpCode.ADD_CATG:    categoryService.Create(inputParser.Name);                                                     break;
                        case OpCode.UPD_CATG:    categoryService.Update(inputParser.CId, inputParser.Name);                                    break;
                        case OpCode.DEL_CATG:    categoryService.Delete(inputParser.CId);                                                      break;
                        case OpCode.LST_CATG:    IOHelper.PrintCategories(categoryService.List());                                             break;
                    }

                }
                catch (EntityNotFound ex)
                {
                    //IOHelper.WriteLine(nameof(opCode) + "; Entity wiht id '"+ ex.Id +"' not found");
                    //opCode = ex.OpCode;
                    eventHelper.Log(ex.OpCode, false, null, "Entity wiht id '"+ ex.Id + "' not found");
                }
                catch (InvalidArgumentCountException ex)
                {
                    eventHelper.Log(ex.Op, false, null, "Invalid number of arguments: " + ex.Cnt);
                }
                catch (InvalidArgumentTypeException ex)
                {
                    eventHelper.Log(ex.Op, false, null, "Argument '" + ex.Argument + "' is not a number");
                }
                catch (InvalidOpException ex)
                {
                    eventHelper.Log(OpCode.NONE, false, null, "Unknown operation: " + ex.Op);
                }
                catch (Exception ex)
                {
                    //handle exception ExHandler.HandleException(ex);
                    eventHelper.Log(OpCode.NONE, false, null, ex.Message);
                }
            }
        }
    }
}
