using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW02.Exceptions
{
    public class InvalidArgumentTypeException : Exception
    {
        public OpCode Op { get; }
        public InvalidArgumentTypeException(OpCode op) 
        {
            Op = op;
        }
    }
}
