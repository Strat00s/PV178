using HW02.BussinessContext;

namespace HW02.Helpers
{
    public class EventHelper
    {
        public delegate void LogEventhandler(OpCode opCode, bool status, Category? entity = null, string? msg = null);

        public event LogEventhandler? LogEvent;

        public void Log(OpCode opCode, bool status, Category? entity = null, string? msg = null)
        {
            LogEvent?.Invoke(opCode, status, entity, msg);
        }
    }
}
