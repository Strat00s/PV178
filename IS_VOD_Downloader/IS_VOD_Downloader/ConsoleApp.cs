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
using IS_VOD_Downloader.Structures;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Dynamic;


//TODO custom course class

namespace IS_VOD_Downloader
{

    //private string
    public class ConsoleApp
    {
        private Request _request;
        private string _baseUrl;
        private bool _hasCookies;   //TODO implement cookie check


        private static string CombineUri(string uri1, string uri2)
        {
            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');
            return string.Format($"{uri1}/{uri2}");
        }

        private async Task<List<(string, string)>> GetVideoNodes(string syllabusUrl)
        {
            var response = await _request.GetAsync(CombineUri(syllabusUrl, "index.qwarp"));
            var result = await response.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Regex.Unescape(result));

            //get lecture names and redirect code
            return htmlDoc.DocumentNode.Descendants()
                .Where(node =>
                    node.HasClass("io-kapitola-box") &&
                    node.Descendants()
                        .Any(subnode => subnode.HasClass("io-obsahuje-prvek") && subnode.InnerText.Contains("Video"))
                )
                .Select(node => (
                    node.Descendants()
                        .Where(subnode => subnode.HasClass("io-kapitola-nazev"))
                        .First()
                        .InnerText
                        .Trim(),
                    node.Descendants("a")
                        .First()
                        .GetAttributeValue("data-warp-id", String.Empty)
                ))
                .ToList();
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
            var response = await _request.GetAsync(CombineUri(_baseUrl, courseUri));
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
                .Reverse()
                .ToList();
        }

        //private async Task<List<(string, string)>> GetChaptersWithVODs(string )

        public ConsoleApp() 
        {
            _baseUrl = "https://is.muni.cz";
            //"https://is.muni.cz/" "auth" "/el/ped/" "podzim2022/ONLINE_A" "/index.qwarp"
            _hasCookies = false;
        }
        public async Task RunAsync()
        {
            _request = new Request();

            //TODO save faculty
            //search for course
            var courses = await SearchForCourse("IA174");
            Console.WriteLine(courses.Count);

            //repeat search
            if (courses.Count == 0)
            {
                Console.WriteLine($"No course with code 'TODO' found!");
                return;
            }

            (string, string)  course;
            if (courses.Count > 1)
            {
                var i = Menu.Select(courses.Select(c => c.Item1).ToList(), "Please select course");
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
                var i = Menu.Select(terms.Select(c => c.Item1).ToList(), "Please select term");
                term = terms[i];
            }
            else
                term = terms.First();

            Console.WriteLine(term.Item1);
            Console.WriteLine(term.Item2);


            //Ask for cookies
            var cookies = new Dictionary<string, string>{
                { "iscreds", "41VQrpku_lgHcW6-UzYhbf_b" },
                { "issession", "HneacmmjmZgrsL4z_zFAIfJT"}
            };
            _request.SetCookies(cookies);
            _baseUrl = CombineUri(_baseUrl, "auth");
            _hasCookies = true;

            //TODO build this url
            //https://is.muni.cz/auth/el/fi/podzim2022/IA174/index.qwarp
            var syllabusUrl = CombineUri(_baseUrl, "el");
            syllabusUrl = CombineUri(syllabusUrl, term.Item2.Replace("/predmet/", String.Empty));
            syllabusUrl = CombineUri(syllabusUrl, "index.qwarp");

            //TODO get all videos
            //TODO check if has access
            var videoNodes = await GetVideoNodes(syllabusUrl);

            Console.WriteLine(videoNodes.Count);

            Console.WriteLine(terms.Count);

            var nodeIds = Menu.MultiSelect(videoNodes.Select(v => v.Item1).ToList(), "Please select leacture(s)");
            //repeat search
            foreach (var nodeId in nodeIds) 
            {
                Console.WriteLine(nodeId);
                Console.WriteLine(videoNodes[nodeId].Item1);
                Console.WriteLine(videoNodes[nodeId].Item2);
            }

            //?prejit=9564869
            //TODO ask for this
            bool preferQuality = true;


            //go through each lecture and find all videos. If some has multiple, ask which ones to download
            foreach (var nodeId in nodeIds)
            {
                var chapter = syllabusUrl + $"?prejit={videoNodes[nodeId].Item2}";
                Console.WriteLine(chapter);
                var response = await _request.GetAsync(chapter);
                var result = await response.ReadAsStringAsync();

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(Regex.Unescape(result));
                var scriptString = htmlDoc.DocumentNode
                    .Descendants("script")
                    .Where(node => node.InnerText.Contains("encode_key"))
                    .First()
                    .InnerText
                    .Replace(" ", String.Empty);

                var matches = Regex.Matches(scriptString, @"""id""\s*:\s*""prvek_.+""|""encode_key""\s*:\s*"".+""");

                var keyItemPairs = new List<(string, string)>();
                for (int i = 0; i < matches.Count; i += 2)
                {
                    if (matches[i].Value.Contains("id"))
                        keyItemPairs.Add((matches[i + 1].Value, matches[i].Value));
                    else
                        keyItemPairs.Add((matches[i].Value, matches[i + 1].Value));
                }

                foreach (var pair in keyItemPairs)
                {
                    Console.WriteLine($"{pair.Item1} {pair.Item2}");
                }

                var videos = htmlDoc.DocumentNode
                    .Descendants()
                    .Where(node => node.HasClass("io-ramecek"))
                    .Select(node => (
                        node.Descendants("a")
                            .First()
                            .InnerText, //video title
                        node.Descendants()
                            .Where(subnode => subnode.HasClass("vidis"))
                            .Select(subnode => (
                                subnode.GetAttributeValue("src", String.Empty),
                                subnode.GetAttributeValue("id", String.Empty)
                            ))
                    ))
                    .ToList();

                foreach (var video in videos)
                {
                    //foreach (var cls in video.GetClasses())
                    //    Console.WriteLine(cls);
                    Console.WriteLine(video.InnerText);
                    foreach (var subitem in video.Item2)
                    {
                        Console.WriteLine($"{subitem.Item1} {subitem.Item2}");
                    }
                }
            }
        }
    }
}
