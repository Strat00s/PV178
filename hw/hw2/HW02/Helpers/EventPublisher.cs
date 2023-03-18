/* Event helper for invoking the one and only event for logging, data collecting and printing to console
 * Probably does not have to be a class on it's own, but that's what I found
 * https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-publish-events-that-conform-to-net-framework-guidelines
 */

namespace HW02.Helpers
{
    public class EventPublisher
    {
        //public delegate void LogEventHandler(Object sender, LogEventArgs args);// OpCode opCode, bool status, Category? entity = null, string? msg = null);

        //public event LogEventHandler? LogEvent;
        public EventHandler<LogEventArgs>? LogEvent;

        //delegate caller
        public void Log(LogEventArgs e)// OpCode opCode, bool status, Category? entity = null, string? msg = null)
        {
            LogEvent?.Invoke(this, e);
            //LogEvent?.Invoke(e);// opCode, status, entity, msg);
        }
    }
}
