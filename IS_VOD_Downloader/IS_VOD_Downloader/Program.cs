
/*
 * 1. Get cookies
 * 2. Get course and term
 *      a. Search for course and check that it exists
 *      b. Write possible course terms
 * 3. Search for all videos
 * 4. Download specific video(s), download all
 * 5. If downloading single, ask for specific quality
 *    If downloading multiple, ask for quality vs size
 * 6. Download everything
 * 7. Call external program to convert it
 */

using IS_VOD_Downloader;

namespace ISVOD
{
    public class Program
    {
        public static async Task Main()
        {
            //setup everyting
            //call ConsoleApp
            var app = new ConsoleApp();
            await app.RunAsync();
        }
    }
}