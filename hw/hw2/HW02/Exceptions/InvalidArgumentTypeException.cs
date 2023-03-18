/* Invalid operation argument type exception
 * Stores opcode and the invalid argument stirng
 */

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
