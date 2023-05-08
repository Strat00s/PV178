
using IS_VOD_Downloader;

namespace ISVOD
{
    public class Program
    {
        public static async Task Main()
        {
            var app = new ConsoleApp();
            await app.RunAsync();
            Console.WriteLine("See you later...");
        }
    }
}