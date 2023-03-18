/* Wrapper for main logic
 * Gets opcode from input parser and executes the required action
 * Also handles exceptions
 * 
 * Was a public static class for quite some time but I made it a normal class cause events.
 */

using HW02.BussinessContext.Services;
using HW02.Exceptions;
using HW02.Helpers;

namespace HW02
{
    public class ConsoleApp
    {
        public EventHandler<LogEventArgs>? LogEvent;

        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;
        private readonly InputParser _inputParser;

        public ConsoleApp(CategoryService categoryService, ProductService productService, InputParser inputParser)
        {
            _categoryService = categoryService;
            _productService = productService;
            _inputParser = inputParser;
        }
        public void Run()
        {
            //add iohelper to event
            _categoryService.LogEvent += IOHelper.HandleEvent;
            _productService.LogEvent  += IOHelper.HandleEvent;
            LogEvent                  += IOHelper.HandleEvent;

            while (true)
            {
                try
                {
                    //do appropriate action depending on input
                    switch (_inputParser.Parse(IOHelper.ReadLine()))
                    {
                        case OpCode.NONE:                                                                                                           continue;
                        case OpCode.EXIT:                                                                                                           return;
                        case OpCode.HELP:        IOHelper.PrintHelp();                                                                              break;
                        case OpCode.GET_BY_CATG: IOHelper.PrintTable(_productService.ListByCategory(_inputParser.CId));                             break;   //apparently specifiying the template type is not required...
                        case OpCode.ADD_PROD:    _productService.Create(_inputParser.Name, _inputParser.CId, _inputParser.Price);                   break;
                        case OpCode.UPD_PROD:    _productService.Update(_inputParser.PId, _inputParser.Name, _inputParser.CId, _inputParser.Price); break;
                        case OpCode.DEL_PROD:    _productService.Delete(_inputParser.PId);                                                          break;
                        case OpCode.LST_PROD:    IOHelper.PrintTable(_productService.List());                                                       break;
                        case OpCode.ADD_CATG:    _categoryService.Create(_inputParser.Name);                                                        break;
                        case OpCode.UPD_CATG:    _categoryService.Update(_inputParser.CId, _inputParser.Name);                                      break;
                        case OpCode.DEL_CATG:    _categoryService.Delete(_inputParser.CId);                                                         break;
                        case OpCode.LST_CATG:    IOHelper.PrintTable(_categoryService.List());                                                      break;
                    }

                }
                //handle custom exceptions first
                catch (InvalidArgumentCountException ex)
                {
                    LogEvent?.Invoke(this, new(ex.OpCode, false, null, "Invalid number of arguments: " + ex.Cnt));
                }
                catch (InvalidArgumentTypeException ex)
                {
                    LogEvent?.Invoke(this, new(ex.OpCode, false, null, "Argument '" + ex.Argument + "' is not a number"));
                }
                catch (InvalidOpException ex)
                {
                    LogEvent?.Invoke(this, new(OpCode.NONE, false, null, "Unknown operation: " + ex.Operation));
                }
                catch (Exception ex)
                {
                    LogEvent?.Invoke(this, new(OpCode.NONE, false, null, ex.Message));
                }
            }
        }
    }
}
