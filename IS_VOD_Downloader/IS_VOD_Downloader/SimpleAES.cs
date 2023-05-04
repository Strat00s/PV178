using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Channels;

public class SimpleAES
{
    private readonly Aes _aes;

    public SimpleAES(byte[] key, byte[] initialIv = null)
    {
        _aes = Aes.Create();
        _aes.Key = key;
        _aes.Mode = CipherMode.CBC;
        _aes.Padding = PaddingMode.PKCS7;
        _aes.IV = initialIv ?? new byte[16]; // Set initial IV to zero if not provided
    }

    public byte[] Decrypt(byte[] data)
    {
        Console.WriteLine("Encrypted data:");
        PrintHex(data.Take(16).ToArray());
        using MemoryStream memoryStream = new MemoryStream();
        using ICryptoTransform decryptor = _aes.CreateDecryptor();
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);

        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();

        int decryptedDataLength = (int)memoryStream.Length;
        byte[] decryptedData = ArrayPool<byte>.Shared.Rent(decryptedDataLength);
        memoryStream.Position = 0;
        memoryStream.Read(decryptedData, 0, decryptedDataLength);

        Console.WriteLine("Decrypted data:");
        PrintHex(decryptedData.Take(16).ToArray());
        return decryptedData;
    }

    private void PrintHex(byte[] data)
    {
        foreach (byte b in data) { Console.Write(b.ToString("X")); }
        Console.WriteLine("");
    }

    public async Task DecryptAsync(int segmentCnt, Channel<(int, byte[])> inputCh, Channel<byte[]> outputCh)
    {
        int currentSegment = 0;
        var inputBuffer = new Dictionary<int, byte[]>();

        while (currentSegment < segmentCnt)
        {
            if (inputCh.Reader.TryRead(out var input))
            {
                inputBuffer[input.Item1] = input.Item2;
            }

            if (inputBuffer.ContainsKey(currentSegment))
            {
                outputCh.Writer.TryWrite(Decrypt(inputBuffer[currentSegment]));
                inputBuffer.Remove(currentSegment);
                currentSegment++;
            }

            else
                await inputCh.Reader.WaitToReadAsync();
        }
    }
}