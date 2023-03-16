using HW02.BussinessContext;
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

    /* Wrapper for main logic
     * Gets opcode from input parser and executes the required action
     */
    public static class ConsoleApp
    {
        public static void Run(CategoryService categoryService, ProductService productService, InputParser inputParser, EventHelper eventHelper)
        {
            int firstRun = 0;

            while (true)
            {
                try
                {
                    //seeder moved here to allow for exception catching when seeding
                    if (firstRun == 0)
                    {
                        firstRun++;
                        Seeder.FillDB(categoryService, productService);
                        firstRun++;
                    }
                    else if (firstRun == 1)
                        return;


                    switch (inputParser.Parse(IOHelper.ReadLine()))
                    {
                        case OpCode.NONE:        continue;
                        case OpCode.EXIT:        return;
                        case OpCode.HELP:        IOHelper.PrintHelp();                                                                         break;
                        case OpCode.GET_BY_CATG: IOHelper.PrintTable<Product>(productService.ListByCategory(inputParser.CId));                 break;
                        case OpCode.ADD_PROD:    productService.Create(inputParser.Name, inputParser.CId, inputParser.Price);                  break;
                        case OpCode.UPD_PROD:    productService.Update(inputParser.PId, inputParser.Name, inputParser.CId, inputParser.Price); break;
                        case OpCode.DEL_PROD:    productService.Delete(inputParser.PId);                                                       break;
                        case OpCode.LST_PROD:    IOHelper.PrintTable<Product>(productService.List());                                          break;
                        case OpCode.ADD_CATG:    categoryService.Create(inputParser.Name);                                                     break;
                        case OpCode.UPD_CATG:    categoryService.Update(inputParser.CId, inputParser.Name);                                    break;
                        case OpCode.DEL_CATG:    categoryService.Delete(inputParser.CId);                                                      break;
                        case OpCode.LST_CATG:    IOHelper.PrintTable<Category>(categoryService.List());                                        break;
                    }

                }
                //handle exceptions
                catch (EntityNotFound ex)
                {
                    eventHelper.Log(ex.OpCode, false, null, "Entity wiht id '"+ ex.Id + "' not found");
                }
                catch (InvalidArgumentCountException ex)
                {
                    eventHelper.Log(ex.OpCode, false, null, "Invalid number of arguments: " + ex.Cnt);
                }
                catch (InvalidArgumentTypeException ex)
                {
                    eventHelper.Log(ex.OpCode, false, null, "Argument '" + ex.Argument + "' is not a number");
                }
                catch (InvalidOpException ex)
                {
                    eventHelper.Log(OpCode.NONE, false, null, "Unknown operation: " + ex.Operation);
                }
                catch (Exception ex)
                {
                    eventHelper.Log(OpCode.NONE, false, null, ex.Message);
                }
            }
        }
    }
}
