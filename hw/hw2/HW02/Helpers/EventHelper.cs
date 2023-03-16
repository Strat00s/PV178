/* Event helper for invoking the one and only event for logging, data collecting and printing to console
 */

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
