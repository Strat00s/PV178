/* Exception for invalid argument count for operation
 * Stores opcode and number of arguments given
 */

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
