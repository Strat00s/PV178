using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW02.Exceptions
{
    public class InvalidArgumentTypeException : Exception
    {
        public OpCode OpCode { get; }
        public string Argument { get; }
        public InvalidArgumentTypeException(OpCode opCode, string argument) 
        {
            OpCode = opCode;
            Argument = argument;
        }
    }
}
