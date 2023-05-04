using IS_VOD_Downloader.Structures;
using ISVOD;
using System.Threading.Channels;

namespace IS_VOD_Downloader.Helpers
{
    public class DownloadManager
    {
        private Request _request;

        public DownloadManager(Request request)
        {
            _request = request;
        }

        public async Task StartDownload(int workerCnt, ThreadSafeInt progress, List<Segment> segments, string streamUrl, Channel<(int, byte[])> downloadCh)
        {
            if (workerCnt == 0 || workerCnt > Environment.ProcessorCount)
                workerCnt = Environment.ProcessorCount;

            var distributedSegments = segments.Select((x, i) => new { item = x, index = i })
                   .GroupBy(x => x.index % workerCnt)
                   .Select(g => g.Select(x => x.item).ToList())
                   .ToList();

            var workers = new List<Task>();
            for (int i = 0; i < workerCnt; i++)
            {
                workers.Add(DownloadWorker.Download(_request, distributedSegments[i], downloadCh, progress, streamUrl));
            }

            await Task.WhenAll(workers);
        }
    }
}
