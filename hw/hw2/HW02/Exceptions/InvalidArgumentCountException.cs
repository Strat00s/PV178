using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW02.Exceptions
{
    internal class InvalidArgumentCountException : Exception
    {
        public OpCode OpCode { get; }
        public int Cnt { get; }
        public InvalidArgumentCountException(OpCode opCode, int cnt) 
        {
            OpCode = opCode;
            Cnt = cnt;
        }
    }
}
