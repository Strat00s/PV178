using IS_VOD_Downloader.Structures;
using ISVOD;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace IS_VOD_Downloader.Helpers
{
    public class DownloadManager
    {
        private Request _request;

        public DownloadManager(Request request)
        {
            _request = request;
        }
        public async Task StartDownload(int threadCount, ThreadSafeInt progress, List<Segment> segments, string streamUrl, Channel<(int, byte[])> downloadCh)
        {
            if (threadCount == 0 || threadCount > Environment.ProcessorCount)
            {
                threadCount = Environment.ProcessorCount;
            }

            var segmentChunkSize = (int)Math.Ceiling((double)segments.Count / threadCount);
            var segmentChunks = segments.Select((x, i) => new { item = x, index = i })
                   .GroupBy(x => x.index / segmentChunkSize)
                   .Select(g => g.Select(x => x.item).ToList())
                   .ToList();

            var downloadWorker = new List<Task>();
            foreach ( var chunk in segmentChunks)
            {
                downloadWorker.Add(DownloadWorker.Download(_request, chunk, downloadCh, progress, streamUrl));
            }

            await Task.WhenAll(downloadWorker);
        }
    }
}
