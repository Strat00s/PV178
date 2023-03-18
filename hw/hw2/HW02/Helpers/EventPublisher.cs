/* Event helper for invoking the one and only event for logging, data collecting and printing to console
 * Probably does not have to be a class on it's own, but that's what I found
 * https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-publish-events-that-conform-to-net-framework-guidelines
 */

namespace HW02.Helpers
{
    public class EventPublisher
    {
        public EventHandler<LogEventArgs>? LogEvent;

        public void Log(LogEventArgs e)
        {
            LogEvent?.Invoke(this, e);
        }
    }
}
