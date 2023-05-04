using System;
using System.Collections.Generic;
using System.IO;

public class ChunkedFileWriter : IDisposable
{
    private readonly FileStream _fileStream;

    public ChunkedFileWriter(string filePath)
    {
        _fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
    }

    public void WriteBytes(List<byte> byteList)
    {
        _fileStream.Write(byteList.ToArray(), 0, byteList.Count);
    }

    public void Dispose()
    {
        _fileStream?.Dispose();
    }
}

