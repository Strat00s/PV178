/* Wrapper for main logic
 * Gets opcode from input parser and executes the required action
 * Also handles exceptions
 */

using HW02.BussinessContext.Services;
using HW02.Exceptions;
using HW02.Helpers;
namespace HW02
{
    public static class ConsoleApp
    {
        public static void Run(CategoryService categoryService, ProductService productService, IOHelper ioHelper, InputParser inputParser, EventHelper eventHelper)
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
                        eventHelper.LogEvent += IOHelper.HandleEvent;   //attach iohelper to event after seeding is done
                        firstRun++;
                    }
                    //exit on 
                    else if (firstRun == 1)
                        return;


                    switch (inputParser.Parse(IOHelper.ReadLine()))
                    {
                        case OpCode.NONE:                                                                                                      continue;
                        case OpCode.EXIT:                                                                                                      return;
                        case OpCode.HELP:        IOHelper.PrintHelp();                                                                         break;
                        case OpCode.GET_BY_CATG: IOHelper.PrintTable(productService.ListByCategory(inputParser.CId));                          break;
                        case OpCode.ADD_PROD:    productService.Create(inputParser.Name, inputParser.CId, inputParser.Price);                  break;
                        case OpCode.UPD_PROD:    productService.Update(inputParser.PId, inputParser.Name, inputParser.CId, inputParser.Price); break;
                        case OpCode.DEL_PROD:    productService.Delete(inputParser.PId);                                                       break;
                        case OpCode.LST_PROD:    IOHelper.PrintTable(productService.List());                                                   break;
                        case OpCode.ADD_CATG:    categoryService.Create(inputParser.Name);                                                     break;
                        case OpCode.UPD_CATG:    categoryService.Update(inputParser.CId, inputParser.Name);                                    break;
                        case OpCode.DEL_CATG:    categoryService.Delete(inputParser.CId);                                                      break;
                        case OpCode.LST_CATG:    IOHelper.PrintTable(categoryService.List());                                                  break;
                    }

                }
                //handle exceptions
                catch (EntityNotFound ex)
                {
                    if (ex.OpCode == OpCode.DEL_PROD || ex.OpCode == OpCode.UPD_PROD)
                        eventHelper.Log(ex.OpCode, false, null, "Product with id '"+ ex.Id + "' not found");
                    else
                        eventHelper.Log(ex.OpCode, false, null, "Category with id '" + ex.Id + "' not found");
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
