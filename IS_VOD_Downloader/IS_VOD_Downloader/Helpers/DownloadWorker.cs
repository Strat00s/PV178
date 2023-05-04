using IS_VOD_Downloader.Structures;
using ISVOD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace IS_VOD_Downloader.Helpers
{
    public static class DownloadWorker
    {
        private static void PrintHex(byte[] data)
        {
            foreach (byte b in data) { Console.Write(b.ToString("X")); }
            Console.WriteLine("");
        }
        public static async Task Download(Request request, List<Segment> segments, Channel<(int, byte[])> downloadCh, ThreadSafeInt progress, string streamUrl )
        {
            var data = new List<byte>();
            foreach (var segment in segments)
            {
                var headers = new Dictionary<string, string>() {
                    { "user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36"},
                    { "range", $"bytes={segment.Start}-{segment.Start + segment.Length - 1}"},
                    //{ "referer", referer}
                };
                var requestUrl = streamUrl + "media.ts?ts=" + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                var response = await request.GetAsync(requestUrl, headers);
                var result = await response.ReadAsByteArrayAsync();

                Console.WriteLine("Downloaded data:");
                PrintHex(result.Take(16).ToArray());

                progress.Increment();
                downloadCh.Writer.TryWrite((segment.Num, result));
            }
        }
    }
}
