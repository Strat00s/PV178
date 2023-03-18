/* Custom event arguments
 */

using HW02.BussinessContext;

namespace HW02.Helpers
{
    public class LogEventArgs : EventArgs
    {
        public OpCode OpCode { get; private set; }
        public bool Success { get; private set; }
        public Category? Entity { get; private set; }
        public string? Message { get; private set; }

        public LogEventArgs(OpCode opcode, bool success, Category? entity = null, string? message = null)
        {
            OpCode  = opcode;
            Success = success;
            Entity  = entity;
            Message = message;
        }
    }
}
