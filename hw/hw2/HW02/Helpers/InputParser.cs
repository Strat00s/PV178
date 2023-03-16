/* Input parser for reading and parsing input
 * Used to check argument types and argument count
 * Returns "internal" operation opcode and stores the rest of the arguments
 */


using HW02.Exceptions;

namespace HW02.Helpers
{
    public class InputParser
    {
        private int _pId;
        private int _cId;
        private decimal _price;
        private string _name;
        private OpCode _opCode;

        private static readonly string[] _operations = {
            "exit",
            "help",
            "get-products-by-category",
            "add-product", "update-product", "delete-product", "list-products",
            "add-category", "update-category", "delete-category", "list-categories"
        };


        public int PId {get { return _pId;}}
        public int CId { get { return _cId;}}
        public decimal Price { get { return _price;}}
        public string Name { get { return _name;}}

        public InputParser()
        {
            _pId   = 0;
            _cId   = 0;
            _price = 0;
            _name  = string.Empty;
        }


        //check operation argument count (each operation has some number of arguments)
        private void CheckArgCount(int length, int required)
        {
            if (length - 1 != required)
                throw new InvalidArgumentCountException(_opCode, length - 1);
        }

        //throws type error on invalid argument
        public static int ParseInt(OpCode op, string input)
        {
            if (Int32.TryParse(input, out int output))
                return output;

            throw new InvalidArgumentTypeException(op, input);
        }

        //throws type error on invalid argument
        public static decimal ParseDec(OpCode op, string input)
        {
            if (Decimal.TryParse(input, out decimal output))
                return output;
            throw new InvalidArgumentTypeException(op, input);
        }

        public OpCode Parse(string input)
        {
            string[] arguments = input.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            //empty line
            if (arguments.Length == 0)
                return OpCode.NONE;

            _opCode = _operations.Contains(arguments[0]) ? (OpCode)Array.IndexOf(_operations, arguments[0]) : OpCode.OP_ERR;    //convert string operation to internal opcode

            switch (_opCode)
            {
                //argumentless operations
                case OpCode.EXIT:
                case OpCode.HELP:
                case OpCode.LST_PROD:
                case OpCode.LST_CATG:
                    CheckArgCount(arguments.Length, 0);
                    return _opCode;

                //single argument operations
                case OpCode.DEL_PROD:
                    CheckArgCount(arguments.Length, 1);
                    _pId = ParseInt(_opCode, arguments[1]);
                    return _opCode;

                case OpCode.DEL_CATG:
                case OpCode.GET_BY_CATG:
                    CheckArgCount(arguments.Length, 1);
                    _cId = ParseInt(_opCode, arguments[1]);
                    return _opCode;

                case OpCode.ADD_CATG:
                    CheckArgCount(arguments.Length, 1);
                    _name = arguments[1];
                    return _opCode;

                //only 2 argument operation
                case OpCode.UPD_CATG:
                    CheckArgCount(arguments.Length, 2);
                    _cId  = ParseInt(_opCode, arguments[1]);
                    _name = arguments[2];
                    return _opCode;

                //only 3 argument operation
                case OpCode.ADD_PROD:
                    CheckArgCount(arguments.Length, 3);
                    _name  = arguments[1];
                    _cId   = ParseInt(OpCode.ADD_PROD, arguments[2]);
                    _price = ParseDec(OpCode.ADD_PROD, arguments[3]);
                    return _opCode;

                //only 4 argument operation
                case OpCode.UPD_PROD:
                    CheckArgCount(arguments.Length, 4);
                    _name  = arguments[2];
                    _pId   = ParseInt(OpCode.UPD_PROD, arguments[1]);
                    _cId   = ParseInt(OpCode.UPD_PROD, arguments[3]);
                    _price = ParseDec(OpCode.UPD_PROD, arguments[4]);
                    return _opCode;
                
                //op_err opcode
                default:
                    throw new InvalidOpException(arguments[0]);
            }
        }
    }
}
