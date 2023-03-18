/* Custom event arguments
 */

using HW02.BussinessContext;

namespace HW02.Helpers
{
    public class LogEventArgs : EventArgs
    {
        public OpCode OpCode { get; private set; }
        public bool Status { get; private set; }
        public Category? Entity { get; private set; }
        public string? Message { get; private set; }

        public LogEventArgs(OpCode opcode, bool status, Category? entity = null, string? message = null)
        {
            OpCode = opcode;
            Status = status;
            Entity = entity;
            Message = message;
        }
    }
}
