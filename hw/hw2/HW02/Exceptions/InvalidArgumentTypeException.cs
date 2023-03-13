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
        public string Argument { get; }
        public InvalidArgumentTypeException(OpCode op, string argument) 
        {
            Op = op;
            Argument = argument;
        }
    }
}
