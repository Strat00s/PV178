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
        public static async Task DownloadAsync(Request request, List<Segment> segments, Channel<(int, byte[])> downloadCh, ThreadSafeInt progress, string streamUrl )
        {
            foreach (var segment in segments)
            {
                var headers = new Dictionary<string, string>() {
                    //{ "user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36"},
                    { "range", $"bytes={segment.Start}-{segment.Start + segment.Length - 1}"},
                };
                var requestUrl = streamUrl + "media.ts?ts=" + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                var response = await request.GetAsync(requestUrl, headers);
                var result = await response.ReadAsByteArrayAsync();

                downloadCh.Writer.TryWrite((segment.Num, result));
                progress.Increment();
            }
        }
    }
}
