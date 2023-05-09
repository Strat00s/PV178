using ISVOD;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using IS_VOD_Downloader.Structures;
using System.Security.Cryptography;
using System.Text;
using System.Reflection.PortableExecutable;
using System;
using System.Diagnostics.CodeAnalysis;
using IS_VOD_Downloader.Helpers;
using static IS_VOD_Downloader.Structures.QueryData;
using System.Threading.Channels;
using IS_VOD_Downloader.Enums;
using System.Net;

namespace IS_VOD_Downloader
{

    //private string
    public class ConsoleApp
    {
        private Request _request;
        private string _baseUrl;
        private bool _hasCookies;   //TODO implement cookie check


        //Helper methods
        public static byte[] ArrayXOR(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                throw new ArgumentException("Both arrays must have the same length.");
            }

            byte[] result = new byte[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                result[i] = (byte)(array1[i] ^ array2[i]);
            }

            return result;
        }

        public static string GetValidFilePath(string filePath)
        {
            string path = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            var forbiddenCharacters = new Dictionary<char, char>()
            {
                {'/','-'},
                {'\\','-'},
                {'<',' '},
                {'>',' '},
                {':','-'},
                {'|',' '},
                {'?',' '},
                {'*',' '}
            };

            foreach (var forbiddenChar in forbiddenCharacters)
            {
                fileName = fileName.Replace(forbiddenChar.Key, forbiddenChar.Value);
            }

            string newFileName = fileName;
            string newFilePath = Path.Combine(path, newFileName + extension);
            int count = 1;

            while (File.Exists(newFilePath))
            {
                newFileName = $"{fileName} ({count})";
                newFilePath = Path.Combine(path, newFileName + extension);
                count++;
            }

            return newFilePath;
        }



        //Format (and "translate") the term string
        private string FormatTerm(string termUri)
        {
            termUri = termUri.Replace(" ", String.Empty).ToLower();
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

        //request methods
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
            var result = await response.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Regex.Unescape(result));

            return htmlDoc.DocumentNode
                .Descendants()
                .Where(node => node.HasClass("course_link"))
                .Select(node => (
                    node.GetDirectInnerText(),
                    node.GetAttributeValue("href", String.Empty)
                ))
                .Where(pair => pair.Item2 != String.Empty)
                .ToList();
        }

        private async Task<List<(string, string)>> GetTermsForCourse(string courseUrl)
        {
            courseUrl = courseUrl.Trim('/');
            var response = await _request.GetAsync(courseUrl);
            var result = await response.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Regex.Unescape(result));

            return htmlDoc.DocumentNode.Descendants("main")
                .First()
                .ChildNodes
                .Where(x => x.Name == "a")
                .Select(node => (
                    FormatTerm(node.GetDirectInnerText()),
                    node.GetAttributeValue("href", String.Empty).Split("/")[^2]
                ))
                .Where(pair => pair.Item2 != String.Empty)
                .Append((FormatTerm(courseUrl.Split("/")[^2]), courseUrl.Split("/")[^2]))
                .Reverse()
                .ToList();
        }

        //get chapter (lecture) with VoDs and redirect links to them
        private async Task<List<(string, string)>> GetChaptersWithVoDs(string syllabusUrl)
        {
            Console.WriteLine(syllabusUrl);
            var response = await _request.GetAsync(syllabusUrl);
            var result = await response.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Regex.Unescape(result));

            return htmlDoc.DocumentNode.Descendants()
                .Where(node =>
                    node.HasClass("io-kapitola-box") &&
                    node.Descendants()
                        .Any(subnode => subnode.HasClass("io-obsahuje-prvek") && subnode.InnerText.Contains("Video"))   //get all chapters with some video(s)
                )
                .Select(node => (
                    node.Descendants()
                        .Where(subnode => subnode.HasClass("io-kapitola-nazev"))
                        .First()
                        .InnerText  //chapter name
                        .Trim(),
                    node.Descendants("a")
                        .First()
                        .GetAttributeValue("data-warp-id", String.Empty)    //redirect link
                ))
                .ToList();
        }

        //get video name, key and path
        private async Task<List<(string, string, string)>> GetVoDsData(string chapterUrl)
        {
            var response = await _request.GetAsync(chapterUrl);
            var result = await response.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Regex.Unescape(result));

            //get raw function text from js function with encode key
            var scriptRaw = htmlDoc.DocumentNode
                .Descendants("script")
                .Where(node => node.InnerText.Contains("encode_key"))
                .First()
                .InnerText
                .Replace(" ", String.Empty);

            //there can be multiple keys
            var matches = Regex.Matches(scriptRaw, @"""id""\s*:\s*""prvek_.+""|""encode_key""\s*:\s*"".+""");

            var keyIdPairs = new List<(string, string)>();
            for (int i = 0; i < matches.Count; i += 2)
            {
                if (matches[i].Value.Contains("id"))
                {
                    keyIdPairs.Add((
                        matches[i + 1].Value.Split(":").Last().Replace("\"", String.Empty).Trim(),
                        matches[i].Value.Split(":").Last().Replace("\"", String.Empty).Trim()
                    ));
                }
                else
                {
                    keyIdPairs.Add((
                        matches[i].Value.Split(":").Last().Replace("\"", String.Empty).Trim(),
                        matches[i + 1].Value.Split(":").Last().Replace("\"", String.Empty).Trim()
                    ));
                }
            }

            //get possible vodsData
            return htmlDoc.DocumentNode
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
                            subnode.GetAttributeValue("id", String.Empty).Replace("\"", String.Empty).Trim()
                        ))
                        .First()
                ))
                .Join(keyIdPairs,
                    vodData => vodData.Item2.Item2,
                    kip => kip.Item2,
                    (vodData, kip) => (vodData.InnerText, kip.Item1, vodData.Item2.Item1)
                )
                .ToList();
        }

        private List<Segment> GetSegments(string masterHeader)
        {
            var matches = Regex.Matches(masterHeader, @"#EXT-X-BYTERANGE:\d+@\d+");
            if (matches.Count > 0)
            {
                return matches
                    .Select((match, index) => (
                        val: match.Value.Replace("#EXT-X-BYTERANGE:", String.Empty).Split("@"),
                        index
                    ))
                    .Select(m => new Segment(m.index, int.Parse(m.val[1]), int.Parse(m.val[0])))
                    .ToList();
            }
            else
                throw new NotImplementedException();
        }

        private async Task<byte[]> GetPrimaryKey(string masterHeader, string authUrl)
        {
            var match = Regex.Match(masterHeader, @"#EXT-X-KEY:.*");
            if (match.Success)
            {
                var metaPath = match.Value.TrimEnd('"').Split("\"")[1];
                var response = await _request.GetAsync(authUrl + metaPath);
                return await response.ReadAsByteArrayAsync();
            }
            return new byte[] {};
        }

        private async Task<string> GetMasterHeader(string masterUrl)
        {
            var response = await _request.GetAsync(masterUrl);
            var result = await response.ReadAsStringAsync();
            return result;
        }


        //Main methods
        private async Task<InternalState> CourseSelect(QueryData queryData)
        {
            //search for course
            var userInput = IOHelper.GetInput("Please select course to search for (e.q IA174): ");
            var coursesData = await IOHelper.AnimateAwaitAsync(SearchForCourse(userInput), "Checking course catalog");
            if (coursesData.Count == 0)
            {
                Console.WriteLine($"No course with code '{userInput}' found!");
                return InternalState.CourseSelect;
            }

            //get course
            var selected = IOHelper.Select(coursesData.Select(c => c.Item1).ToList(), "Please select course");
            var courseFac = coursesData[selected].Item1.Split(":");
            var paths = coursesData[selected].Item2.Split("/");
            queryData.AddFaculty(new(courseFac[0], paths[2]));
            queryData.AddCourse(new(courseFac[1], paths[4]));
            queryData.AddTerm(new(String.Empty, paths[3]));
            return InternalState.TermSelect;
        }

        private async Task<InternalState> TermSelect(QueryData queryData)
        {
            var terms = await IOHelper.AnimateAwaitAsync(GetTermsForCourse(queryData.GetCourseUrl()), "Extracting terms");
            //At least one term has to always exist

            //get term
            var selected = IOHelper.Select(terms.Select(t => t.Item1).ToList(), "Please select term");
            queryData.AddTerm(new(terms[selected].Item1, terms[selected].Item2));
            return InternalState.CookiesSelect;
        }

        private async Task<InternalState> CookiesSelect(QueryData queryData)
        {
            if (_hasCookies)
            {
                var keep = IOHelper.BoolSelect("Yes", "no", "Do you want to use previously stored cookies?");
                if (keep)
                    return InternalState.ChapterVideoSelect;
                _request.ClearCookies();
                queryData.ClearCookies();
            }
            _hasCookies = false;
            var iscreds = IOHelper.GetInput("iscreds: ");
            var issession = IOHelper.GetInput("issession: ");

            //new httpclient to catch redirect (which occures with invalid access)
            try
            {
                var cookieContainer = new CookieContainer();
                cookieContainer.Add(new Uri(_baseUrl), new Cookie("iscreds", iscreds));
                cookieContainer.Add(new Uri(_baseUrl), new Cookie("issession", issession));
                var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    CookieContainer = cookieContainer
                };
                var client = new HttpClient(handler);

                var response = await IOHelper.AnimateAwaitAsync(client.GetAsync(queryData.GetBaseUrl() + "?lang=cs;setlang=cs"), "Testing access");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                Console.WriteLine($"Invalid cookies!");
                return InternalState.CookiesSelect;
            }

            queryData.SetCookies(iscreds, issession);
            _request.SetCookies(iscreds, issession);
            _hasCookies = true;
            return InternalState.ChapterVideoSelect;
        }

        private async Task<InternalState> ChapterVideoSelect(QueryData queryData)
        {
            var chapters = new List<(string, string)>();

            try
            {
                chapters = await IOHelper.AnimateAwaitAsync(GetChaptersWithVoDs(queryData.GetSyllabusUrl()), "Extracting lectures with streams");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Syllabus not found for selected course and term combination!");
                return InternalState.Finished;
            }

            //no chapters
            if (chapters.Count == 0)
            {
                Console.WriteLine($"No chapters with videos found!");
                return InternalState.Finished;
            }

            //go through each chapter and find all videos. If some has multiple, ask which ones to download
            var selectedChapters = IOHelper.MultiSelect(chapters.Select(c => c.Item1).ToList(), "Please select chapter(s)");
            List<(string, List<(string, string, string)>)> multipleStreamsInChapter = new();
            int i = 1;
            foreach (var chapterIndex in selectedChapters)
            {
                var chapterUrl = queryData.GetSyllabusUrl() + $"?prejit={chapters[chapterIndex].Item2}";
                var streamData = await IOHelper.AnimateAwaitAsync(GetVoDsData(chapterUrl), $"Extracting stream data {i++}/{selectedChapters.Count}", true);
                if (streamData.Count == 1)
                {
                    Console.WriteLine(streamData[0].Item3);
                    var streamPathSplits = streamData[0].Item3.Split("/");
                    var streamPath = streamPathSplits[6] + "/" + streamPathSplits[7];
                    queryData.AddStream(chapters[chapterIndex].Item1, streamData[0].Item1, streamData[0].Item2, streamPath);
                    continue;
                }
                multipleStreamsInChapter.Add((chapters[chapterIndex].Item1, streamData));
            }
            IOHelper.FinishContinuous();

            foreach (var streamsInChapter in multipleStreamsInChapter)
            {
                var chapterName = streamsInChapter.Item1;
                var streamsData = streamsInChapter.Item2;
                Console.WriteLine($"Multiple streams found in lecture '{chapterName}':");
                var selectedStreams = IOHelper.MultiSelect(streamsData.Select(v => v.Item1).ToList(), "Please select stream(s) to download");
                foreach (var streamIndex in selectedStreams)
                {
                    var streamPathSplits = streamsData[streamIndex].Item3.Split("/");
                    var streamPath = streamPathSplits[6] + "/" + streamPathSplits[7];
                    queryData.AddStream(chapterName, streamsData[streamIndex].Item1, streamsData[streamIndex].Item2, streamPath);
                }
            }

            return InternalState.QualitySelect;
        }

        //TODO maybe remove?
        private void QualitySelect(QueryData queryData)
        {
            //List<string> options = new List<string> { "High quality", "Low file size" };
            //var selected = IOHelper.Select(new List<string> {"Low quality (480p and down)", "High quality (720p and up)"}, "Please select quality");
            //queryData.SetHighQuality(Convert.ToBoolean(selected));
            queryData.SetHighQuality();
        }

        private async Task<InternalState> Download(QueryData queryData)
        {
            foreach (var stream in queryData.Streams)
            {
                //1. get segments and decryption key
                var streamUrl = queryData.GetFileUrl() + stream.StreamPath + queryData.Quality;
                var masterHeader = await IOHelper.AnimateAwaitAsync(GetMasterHeader(streamUrl + "stream.m3u8"), "Extracting segments");
                var segments = GetSegments(masterHeader);

                var primaryKey = await IOHelper.AnimateAwaitAsync(GetPrimaryKey(masterHeader, queryData.GetBaseUrl()), "Extracting decryption key");
                var decryptionKey = ArrayXOR(primaryKey, Convert.FromHexString(stream.EncodeKey));


                //2 create everything necesary for download
                var downloadedDataCh = Channel.CreateUnbounded<(int, byte[])>();
                var decryptedDataCh = Channel.CreateUnbounded<byte[]>();
                var downloadProg = new ThreadSafeInt(0);
                var decryptProg = new ThreadSafeInt(0);
                var fileName = stream.ChapterName + " - " + stream.VideoName + ".ts";
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                filePath = GetValidFilePath(filePath);

                var simpleAes = new SimpleAES(decryptionKey);
                var downloader = new DownloadManager(_request);
                var fileWriter = new ChunkedFileWriter(filePath);


                //3 start download
                Console.WriteLine($"Currently downloading '{stream.VideoName}' from lecture '{stream.ChapterName}'");
                Console.WriteLine("Current progress:\n");
                var segmentCnt = 0;
                var rawData = new List<byte>();
                var downloadTask = downloader.StartDownload(20, downloadProg, segments, streamUrl, downloadedDataCh);
                var decryptTask = simpleAes.DecryptAsync(segments.Count, downloadedDataCh, decryptedDataCh, decryptProg);
                var progressAnimTask = IOHelper.AnimateProgressAsync(downloadProg, segments.Count, decryptProg, segments.Count);

                while (!decryptTask.IsCompleted || decryptedDataCh.Reader.Count > 0)
                {
                    if (decryptedDataCh.Reader.TryRead(out var decryptedSegment))
                    {
                        rawData.AddRange(decryptedSegment);
                        segmentCnt++;

                        if (segmentCnt >= 50)
                        {
                            segmentCnt = 0;
                            fileWriter.WriteBytes(rawData);
                            rawData.Clear();
                        }
                    }
                    else
                        await Task.WhenAny(decryptTask, decryptedDataCh.Reader.WaitToReadAsync().AsTask());

                }
                fileWriter.WriteBytes(rawData);
                fileWriter.Dispose();

                await Task.WhenAll(downloadTask, decryptTask, progressAnimTask);
            }

            return InternalState.Finished;
        }

        private InternalState Finished()
        {
            var selected = IOHelper.BoolSelect("yes", "No", "Done. Do you want to start over?");

            return selected ? InternalState.CourseSelect : InternalState.Exit;
        }



        public ConsoleApp()
        {
            _baseUrl = "https://is.muni.cz";
            _hasCookies = false;
            _request = new Request();
        }

        public async Task RunAsync()
        {
            QueryData queryData = new(_baseUrl);
            var state = InternalState.CourseSelect;
            bool firstRun = true;

            while (true)
            {
                switch (state)
                {
                    case InternalState.CourseSelect:
                        state = await CourseSelect(queryData);
                        break;
                    case InternalState.TermSelect:
                        state = await TermSelect(queryData);
                        break;
                    case InternalState.CookiesSelect:
                        if (firstRun)
                            Console.WriteLine("Authorization required to continue. Please login to https://is.muni.cz/ in your browser and copy-paste your cookies:");
                        firstRun = false;
                        state = await CookiesSelect(queryData);
                        break;
                    case InternalState.ChapterVideoSelect:
                        state = await ChapterVideoSelect(queryData);
                        break;
                    case InternalState.QualitySelect:
                        QualitySelect(queryData);
                        state = InternalState.Download;
                        break;
                    case InternalState.Download:
                        state = await Download(queryData);
                        break;
                    case InternalState.Finished:
                        state = Finished();
                        break;
                    case InternalState.Exit:
                        return;
                }
            }
        }
    }
}
