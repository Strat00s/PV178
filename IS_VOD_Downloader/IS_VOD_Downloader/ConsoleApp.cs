using ISVOD;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using IS_VOD_Downloader.Structures;
using IS_VOD_Downloader.Helpers;
using System.Threading.Channels;
using IS_VOD_Downloader.Enums;
using System.Net;
using Xabe.FFmpeg;

namespace IS_VOD_Downloader
{
    public class ConsoleApp
    {
        private readonly Request _request;
        private readonly string _baseUrl;
        private bool _hasCookies;
        private string? _ffmpegDir;
        private bool _deleteOriginal;


        public ConsoleApp()
        {
            _baseUrl = "https://is.muni.cz/";
            _hasCookies = false;
            _request = new Request();
            _ffmpegDir = null;
            _deleteOriginal = false;
        }

        public async Task RunAsync()
        {
            QueryData queryData = new(_baseUrl);
            var state = InternalState.CourseSelect;
            bool firstRun = true;

            while (true)
            {
                try {
                    switch (state)
                    {
                        case InternalState.CourseSelect:
                            queryData.Clear();
                            state = await CourseSelect(queryData);
                            break;
                        case InternalState.TermSelect:
                            state = await TermSelect(queryData);
                            break;
                        case InternalState.CookiesSelect:
                            if (firstRun)
                                Console.WriteLine("\nAuthorization required to continue. Please login to https://is.muni.cz/ in your browser and copy-paste your cookies:");
                            firstRun = false;
                            state = await CookiesSelect(queryData);
                            break;
                        case InternalState.ChapterVideoSelect:
                            state = await ChapterVideoSelect(queryData);
                            break;
                        case InternalState.QualitySelect:
                            QualitySelect(queryData);
                            state = InternalState.ConversionSelect;
                            break;
                        case InternalState.ConversionSelect:
                            ConversionSelect();
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
                catch (HttpListenerException ex)
                {
                    Console.WriteLine($"Network error: {ex.Message}");
                    state = InternalState.Finished;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return;
                }
            }
        }


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

        //check if path with same name exists and create new (windows style)
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
        private static string FormatTerm(string termUri)
        {
            termUri = termUri.Replace(" ", String.Empty).ToLower();
            if (termUri.Contains("jaro") || termUri.Contains("spring"))
            {
                return string.Concat("Spring ", termUri.AsSpan(termUri.Length - 4));
            }
            if (termUri.Contains("podzim") || termUri.Contains("autumn"))
            {
                return string.Concat("Autumn ", termUri.AsSpan(termUri.Length - 4));
            }
            return termUri;
        }


        //request methods
        private async Task<List<NamePathPair>> SearchForCourse(string courseCode)
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
                .Select(node => new NamePathPair(
                    node.GetDirectInnerText(),  //course code
                    node.GetAttributeValue("href", String.Empty)
                ))
                .Where(pair => pair.Path != String.Empty)
                .ToList();
        }

        private async Task<List<NamePathPair>> GetTermsForCourse(string courseUrl)
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
                .Select(node => new NamePathPair(
                    FormatTerm(node.GetDirectInnerText()),  //term name (en/cz)
                    node.GetAttributeValue("href", String.Empty).Split("/")[^2]
                ))
                .Where(pair => pair.Path != String.Empty)
                .Append(new (FormatTerm(courseUrl.Split("/")[^2]), courseUrl.Split("/")[^2]))
                .Reverse()
                .ToList();
        }

        //get chapter (lecture) with VoDs and redirect links to them
        private async Task<List<NamePathPair>> GetChaptersWithVoDs(string syllabusUrl)
        {
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
                .Select(node => new NamePathPair(
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
        private async Task<List<NameKeyPath>> GetVoDsData(string chapterUrl)
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
                    (vodData, kip) => new NameKeyPath(vodData.InnerText, kip.Item1, vodData.Item2.Item1)
                )
                .ToList();
        }

        //extract segments from master header file
        private static List<Segment> GetSegments(string masterHeader)
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

        //get OPT for decryption key
        private async Task<byte[]> GetPrimaryKey(string masterHeader, string authUrl)
        {
            var match = Regex.Match(masterHeader, @"#EXT-X-KEY:.*");
            if (match.Success)
            {
                var metaPath = match.Value.TrimEnd('"').Split("\"")[1];
                var response = await _request.GetAsync(authUrl + metaPath);
                return await response.ReadAsByteArrayAsync();
            }
            return Array.Empty<byte>();
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
            var selected = IOHelper.Select(coursesData.Select(c => c.Name).ToList(), "Please select course");
            var courseFac = coursesData[selected].Name.Split(":");
            var paths = coursesData[selected].Path.Split("/");
            queryData.AddFaculty(new(courseFac[0], paths[2]));
            queryData.AddCourse(new(courseFac[1], paths[4]));
            queryData.AddTerm(new(String.Empty, paths[3]));
            return InternalState.TermSelect;
        }

        private async Task<InternalState> TermSelect(QueryData queryData)
        {
            var terms = await IOHelper.AnimateAwaitAsync(GetTermsForCourse(queryData.GetCourseUrl()), "Extracting terms");

            //get term
            var selected = IOHelper.Select(terms.Select(t => t.Name).ToList(), "Please select term");
            queryData.AddTerm(new(terms[selected].Name, terms[selected].Path));
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

                async Task<int> action()
                {
                    var response = await client.GetAsync(queryData.GetBaseUrl() + "auth/?lang=cs;setlang=cs");
                    response.EnsureSuccessStatusCode();
                    return 42;
                }

                await IOHelper.AnimateAwaitAsync(action(), "Testing cookies");
            }
            catch (HttpRequestException)
            {
                Console.WriteLine($"Invalid cookies!");
                return InternalState.CookiesSelect;
            }
            catch (CookieException)
            {
                Console.WriteLine($"Invalid cookies format!");
                return InternalState.CookiesSelect;
            }

            queryData.SetCookies(iscreds, issession);
            _request.SetCookies(iscreds, issession);
            _hasCookies = true;
            return InternalState.ChapterVideoSelect;
        }

        private async Task<InternalState> ChapterVideoSelect(QueryData queryData)
        {
            var chapters = new List<NamePathPair>();

            try
            {
                chapters = await IOHelper.AnimateAwaitAsync(GetChaptersWithVoDs(queryData.GetSyllabusUrl()), "Extracting lectures with streams");
            }
            catch (HttpRequestException)
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
            var selectedChapters = IOHelper.MultiSelect(chapters.Select(c => c.Name).ToList(), "Please select chapter(s)");
            List<(string, List<NameKeyPath>)> multipleStreamsInChapter = new();
            int i = 1;
            foreach (var chapterIndex in selectedChapters)
            {
                var chapterUrl = queryData.GetSyllabusUrl() + $"?prejit={chapters[chapterIndex].Path}";
                var streamData = await IOHelper.AnimateAwaitAsync(GetVoDsData(chapterUrl), $"Extracting stream data {i++}/{selectedChapters.Count}", true);
                if (streamData.Count == 1)
                {
                    var streamPathSplits = streamData[0].Path.Split("/");
                    var streamPath = streamPathSplits[6] + "/" + streamPathSplits[7];
                    queryData.AddStream(chapters[chapterIndex].Name, streamData[0].Name, streamData[0].Key, streamPath);
                    continue;
                }
                multipleStreamsInChapter.Add((chapters[chapterIndex].Name, streamData));
            }
            IOHelper.FinishContinuous();

            foreach (var streamsInChapter in multipleStreamsInChapter)
            {
                var chapterName = streamsInChapter.Item1;
                var streamsData = streamsInChapter.Item2;
                Console.WriteLine($"\nMultiple streams found in lecture '{chapterName}':");
                var selectedStreams = IOHelper.MultiSelect(streamsData.Select(v => v.Name).ToList(), "Please select stream(s) to download");
                foreach (var streamIndex in selectedStreams)
                {
                    var streamPathSplits = streamsData[streamIndex].Path.Split("/");
                    var streamPath = streamPathSplits[6] + "/" + streamPathSplits[7];
                    queryData.AddStream(chapterName, streamsData[streamIndex].Name, streamsData[streamIndex].Key, streamPath);
                }
            }

            return InternalState.QualitySelect;
        }

        //some streams only have 1 quality which is media-1, so let's just always download that...
        private static void QualitySelect(QueryData queryData)
        {
            //List<string> options = new List<string> { "High quality", "Low file size" };
            //var selected = IOHelper.Select(new List<string> {"Low quality (480p and down)", "High quality (720p and up)"}, "Please select quality");
            //queryData.SetHighQuality(Convert.ToBoolean(selected));
            queryData.SetHighQuality();
            //Console.WriteLine("");
        }

        private void ConversionSelect()
        {
            Console.WriteLine("");

            if (_ffmpegDir == null)
            {
                Console.WriteLine("The final files can be converted via ffmpeg to mp4 if you provide a valid path to ffmpeg executable.");
                Console.WriteLine("The conversion can take a long time!");
                Console.WriteLine("Files are converted one by one after they are downloaded.");
            }

            bool selected;
            
            //change default option when path already exists
            if (_ffmpegDir == null)
                selected = IOHelper.BoolSelect("yes", "No", "Do you want to convert the downloaded file(s) to mp4?");
            else
                selected = IOHelper.BoolSelect("Yes", "no", "Do you want to convert the downloaded file(s) to mp4?");
            if (!selected) {
                _ffmpegDir = null;
                _deleteOriginal = false;
                return;
            }

            //skip the rest if path already exists
            if (_ffmpegDir != null)
            {
                selected = IOHelper.BoolSelect("Yes", "no", "Do you want to use the same path as before?");
                if (selected)
                {
                    _deleteOriginal = IOHelper.BoolSelect("Yes", "no", "Once done, do you want to delete the original file(s)?");
                    return;
                }
                _ffmpegDir = null;
                return;
            }

            while (true)
            {
                var path = IOHelper.GetInput("Path to ffmpeg/folder containing ffmpeg: ");
                path = path.Trim('"');
                if (Path.IsPathFullyQualified(path) && File.Exists(path))
                {
                    _ffmpegDir = Path.GetDirectoryName(path);
                    break;
                }
                if (Path.IsPathFullyQualified(path) && Directory.Exists(path))
                {
                    _ffmpegDir = path;
                    break;
                }
                Console.WriteLine("Invalid path!");
            }

            _deleteOriginal = IOHelper.BoolSelect("Yes", "no", "Once done, do you want to delete the original file(s)?");
        }

        private async Task<InternalState> Download(QueryData queryData)
        {
            Console.WriteLine("");
            Console.WriteLine("Starting download");
            foreach (var stream in queryData.Streams)
            {
                Console.WriteLine($"\n\nCurrently downloading '{stream.VideoName}' from lecture '{stream.ChapterName}'");
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
                var fileWriter = new FileWriter(filePath);


                //3 start download
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

                        //write to file every 50 decrypted segments
                        if (segmentCnt >= 50)
                        {
                            segmentCnt = 0;
                            fileWriter.WriteBytes(rawData.ToArray());
                            rawData.Clear();
                        }
                    }
                    else
                        await Task.WhenAny(decryptTask, decryptedDataCh.Reader.WaitToReadAsync().AsTask());
                }

                await Task.WhenAll(downloadTask, decryptTask, progressAnimTask);

                fileWriter.WriteBytes(rawData.ToArray());
                fileWriter.Dispose();

                //call ffmpeg and convert the file
                if (_ffmpegDir != null)
                {
                    FFmpeg.SetExecutablesPath(_ffmpegDir);
                    var inputFile = filePath;
                    var outputFile = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + ".mp4");

                    try
                    {
                        var mediaInfo = await FFmpeg.GetMediaInfo(inputFile);
                        var conversion = FFmpeg.Conversions.New()
                            .AddStream(mediaInfo.VideoStreams.First())
                            .AddStream(mediaInfo.AudioStreams.First())
                            .SetOutput(outputFile)
                            .SetOutputFormat(Format.mp4)
                            .UseMultiThread(true)
                            .UseMultiThread(16);


                        int progressBarWidth = Console.WindowWidth - 30;
                        Console.WriteLine("");
                        Console.CursorVisible = false;
                        IOHelper.DrawProgressBar(Console.CursorTop, progressBarWidth, "Converting: ", 0, 100);
                        conversion.OnProgress += (sender, args) =>
                        {
                            IOHelper.DrawProgressBar(Console.CursorTop, progressBarWidth, "Converting: ", args.Percent, 100);
                        };
                        await conversion.Start();
                        Console.CursorVisible = true;
                        Console.WriteLine("");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        var selected = IOHelper.BoolSelect("yes", "No", "Do you want to start over?");
                        if (!selected)
                            return InternalState.Exit;
                    }
                }
                if (_deleteOriginal)
                {
                    try
                    {
                        File.Delete(filePath);
                        Console.WriteLine("[ OK ] Deleting file");  //dirty non-async prints to be somewhat consistent
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[FAIL] Deleting file");
                        Console.WriteLine(ex.Message);
                        var selected = IOHelper.BoolSelect("yes", "No", "Do you want to continue?");
                        if (!selected)
                            return InternalState.Exit;
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Download finished!");
            return InternalState.Finished;
        }

        private static InternalState Finished()
        {
            var selected = IOHelper.BoolSelect("yes", "No", "Do you want to start over?");
            return selected ? InternalState.CourseSelect : InternalState.Exit;
        }
    }
}
