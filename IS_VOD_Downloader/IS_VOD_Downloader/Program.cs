
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

namespace ISVOD
{
    public class Program
    {
        public static async Task Main()
        {
            //setup everyting
            //call ConsoleApp
            var cookies = new Dictionary<string, string>{
                { "iscreds", "eX-2hwXUi9W4Q_Kf8E4dRM8q" },
                { "issession", "ckzMyXb-7ONBhawLFWrEj9Z6"}
            };
            //Console.WriteLine("iscreds: eX-2hwXUi9W4Q_Kf8E4dRM8q");
            //Console.WriteLine("issession: ckzMyXb-7ONBhawLFWrEj9Z6");
            //Console.Write("Course: ");
            //var course = Console.ReadLine();
            var request = new Request();

            //search for course
            var html = await request.SearchForCourse("IA174");
            //get raw site
            //var html = await request.GetRawString("https://is.muni.cz/auth/el/fi/podzim2021/IA174/index.qwarp");
            Console.WriteLine(html);

            var tmp = await request.GetAsync("https://is.muni.cz/?lang=cs");
            Console.WriteLine(await tmp.ReadAsStringAsync());
        }
    }
}