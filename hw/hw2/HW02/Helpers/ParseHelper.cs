using HW02.BussinessContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW02.Helpers
{
    public static class ParseHelper
    {
        //throws syntax error when parsing failes, toherwise returns parsed int
        public static int ParseInt(string input)
        {
            int output;
            if (Int32.TryParse(input, out output))
                return output;

            return 0;//throw syntax error
        }


        //throws syntax error when parsing failes, toherwise returns parsed decimal
        public static decimal ParseDec(string input)
        {
            decimal output;
            if (Decimal.TryParse(input, out output))
                return output;
            return 0;
        }
    }
}
