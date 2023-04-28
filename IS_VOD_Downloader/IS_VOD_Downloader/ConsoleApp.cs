using ISVOD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace IS_VOD_Downloader
{

    //private string
    public class ConsoleApp
    {
        private Request _request;

        private async Task<List<(string, string)>> SearchForCourse(string courseCode)
        {
            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("type", "result"),
                new KeyValuePair<string, string>("operace", "get_courses"),
                new KeyValuePair<string, string>("filters", "{\"offered\":[\"1\"]}"),
                new KeyValuePair<string, string>("pvysl", "18002909"),
                new KeyValuePair<string, string>("search_text", courseCode),
                new KeyValuePair<string, string>("search_text_specify", "codes"),
                new KeyValuePair<string, string>("records_per_page", "50")
            });

            var response = await _request.PostAsync("https://is.muni.cz/predmety/predmety_ajax.pl", requestBody);
            //return await response.ReadAsStringAsync();
            var result = await response.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Regex.Unescape(result));
            return htmlDoc.DocumentNode
                .Descendants(0)
                .Where(node => node.HasClass("course_link"))
                .Select(node => (
                    node.GetDirectInnerText(),
                    node.GetAttributeValue("href", String.Empty)
                ))
                .ToList();
        }

        public ConsoleApp() { }
        public async Task RunAsync()
        {
            var cookies = new Dictionary<string, string>{
                { "iscreds", "qnfwgIGEaOLQx7djfRoRdRxp" },
                { "issession", "ckzMyXb-7ONBhawLFWrEj9Z6"}
            };
            //Console.WriteLine("iscreds: eX-2hwXUi9W4Q_Kf8E4dRM8q");
            //Console.WriteLine("issession: ckzMyXb-7ONBhawLFWrEj9Z6");
            //Console.Write("Course: ");

            _request = new Request();

            //var tmp = await _request.GetAsync("https://is.muni.cz/auth/el/fi/podzim2022/IA174/index.qwarp");
            //Console.WriteLine(await tmp.ReadAsStringAsync());
            //_request.ClearCookies();

            //search for course
            var courses = await SearchForCourse("IA174");
            Console.WriteLine(courses.Count);

            foreach ( var course in courses)
            {
                Console.WriteLine(course.Item1);
                Console.WriteLine(course.Item2);
            }
            //Console.WriteLine(html);
        }
    }
}
