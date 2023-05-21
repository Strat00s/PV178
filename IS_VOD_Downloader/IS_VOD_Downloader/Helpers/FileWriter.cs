using System;
using System.Collections.Generic;
using System.IO;

namespace IS_VOD_Downloader.Helpers
{
    public class FileWriter : IDisposable
    {
        private readonly FileStream _fileStream;

        public FileWriter(string filePath)
        {
            _fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        public void WriteBytes(byte[] bytes)
        {
            _fileStream.Write(bytes, 0, bytes.Length);
            _fileStream.Flush();
        }

        public void Dispose()
        {
            _fileStream.Close();
            _fileStream.Dispose();
        }
    }
}