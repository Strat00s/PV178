using HW02.BussinessContext;
using HW02.BussinessContext.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW02.Helpers
{
    public static class InputParser
    {
        //private int _pId;
        //private int _cId;
        //private decimal _price;
        //private string _name;

        //public int PId {get { return _pId;}}
        //public int CId { get { return _cId;}}
        //public decimal Price { get { return _price;}}
        //public string Name { get { return _name;}}

        public enum OpCode
        {
            NONE = 0,
            
            EXIT,
            HELP,
            
            ADD_PROD,
            DEL_PROD,
            UPD_PROD,
            LST_PROD,

            GET_BY_CATG,

            ADD_CATG,
            DEL_CATG,
            UPD_CATG,
            LST_CATG,


            OP_ERR  = 100,
            CNT_ERR
        }

        //public InputParser(int pId, int cId, decimal price, string name)
        //{
        //    _pId   = 0;
        //    _cId   = 0;
        //    _price = 0;
        //    _name  = string.Empty;
        //}
        private static readonly string[] _operations = {
            "exit",
            "help",
            "get-products-by-category",
            "add-product", "update-product", "delete-product", "list-products",
            "add-category", "update-category", "delete-category", "list-categories"
        };

        public static (OpCode, List<string>?) Parse(string input)
        {
            string[] arguments = input.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            if (arguments.Length == 0)
                return (OpCode.NONE, null);

            if (!_operations.Contains(arguments[0]))
                throw new InvalidOperationException(arguments[0]);

            else if (arguments.Length == 1)
            {
                if (arguments[0] == _operations[0])
                    return (OpCode.EXIT, null);
                else if (arguments[0] == _operations[1])
                    return (OpCode.HELP, null);
                else if (arguments[0] == _operations[6])
                    return (OpCode.LST_PROD, null);
                else /*if (arguments[0] == _operations[10])*/
                    return (OpCode.LST_CATG, null);
            }
            else if (arguments.Length == 2)
            {
                if (arguments[0] == _operations[5])
                    return (OpCode.DEL_PROD, null);
                else if (arguments[0] == _operations[9])
                    return (OpCode.DEL_CATG, null);
                else if (arguments[0] == _operations[7])
                    return (OpCode.ADD_CATG, null);
                else /*if (arguments[0] == _operations[2])*/
                    return (OpCode.GET_BY_CATG, null);
            }
            else if (arguments.Length == 3)
                return (OpCode.UPD_CATG, null);
            else if (arguments.Length == 4)
                return ( OpCode.ADD_PROD, null);
            else if (arguments.Length == 5)
                return (OpCode.UPD_PROD, null);
            else
                return (OpCode.CNT_ERR, null);
        }
    }
}
