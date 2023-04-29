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


//TODO custom course class

namespace IS_VOD_Downloader
{

    //private string
    public class ConsoleApp
    {
        private Request _request;
        private string _baseUri;

        private static string CombineUri(string uri1, string uri2)
        {
            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');
            return string.Format($"{uri1}/{uri2}");
        }

        private async Task<List<(string, string)>> SearchForCourse(string courseCode)
        {
            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("type", "result"),
                new KeyValuePair<string, string>("operace", "get_courses"),
                new KeyValuePair<string, string>("filters", "{\"offered\":[\"1\"]}"),
                new KeyValuePair<string, string>("pvysl", "18002909"),
                new KeyValuePair<string, string>("search_text", courseCode),
                //new KeyValuePair<string, string>("search_text_specify", "codes"),
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
                .Where(pair => pair.Item2 != String.Empty)
                .ToList();
        }

        //Format (and "translate") the term string
        private string FormatTerm(string termUri)
        {
            termUri = termUri.Replace(" ", String.Empty);
            if (termUri.Contains("jaro") || termUri.Contains("spring"))
            {
                return "Spring " + termUri.Substring(termUri.Length - 4);
            }
            if (termUri.Contains("podzim") || termUri.Contains("autumn"))
            {
                return "Autumn " + termUri.Substring(termUri.Length - 4);
            }
            return termUri;
        }

        private async Task<List<(string, string)>> GetTermsForCourse(string courseUri)
        {
            var response = await _request.GetAsync(CombineUri(_baseUri, courseUri));
            var result = await response.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Regex.Unescape(result));
            return htmlDoc.DocumentNode.Descendants("main")
                .First()
                .ChildNodes
                .Where(x => x.Name == "a")
                .Select(node => (
                    FormatTerm(node.GetDirectInnerText()),
                    node.GetAttributeValue("href", String.Empty)
                ))
                .Where(pair => pair.Item2 != String.Empty)
                .Append((FormatTerm(courseUri.Split("/")[^2]), courseUri))
                .ToList();
        }

        public ConsoleApp() 
        {
            _baseUri = "https://is.muni.cz";
            //_baseNoAuthUri = new("https://is.muni.cz/");
        }
        public async Task RunAsync()
        {
            var cookies = new Dictionary<string, string>{
                { "iscreds", "wh-ah5WfS7VGaR8KA3qChLNe" },
                { "issession", "HneacmmjmZgrsL4z_zFAIfJT"}
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

            //repeat search
            if (courses.Count == 0)
            {
                Console.WriteLine($"No course with name 'TODO' found!");
                return;
            }

            (string, string)  course;
            if (courses.Count > 1)
            {
                var i = Menu.DrawSelect(courses.Select(c => c.Item1).ToList(), "Please select course");
                course = courses[i];
            }
            else
                course = courses.First();

            Console.WriteLine(course.Item1);
            Console.WriteLine(course.Item2);

            //Get terms
            var terms = await GetTermsForCourse(course.Item2);
            Console.WriteLine(terms.Count);
            //repeat search
            if (terms.Count == 0)
            {
                Console.WriteLine($"No terms found!");
                return;
            }

            (string, string) term;
            if (terms.Count > 1)
            {
                var i = Menu.DrawSelect(terms.Select(c => c.Item1).ToList(), "Please select term");
                term = terms[i];
            }
            else
                term = terms.First();
        }
    }
}
