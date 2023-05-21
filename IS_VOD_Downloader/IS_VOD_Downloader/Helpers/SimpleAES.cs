using IS_VOD_Downloader.Structures;
using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Channels;

namespace IS_VOD_Downloader.Helpers
{
    public class SimpleAES
    {
        private readonly Aes _aes;

        public SimpleAES(byte[] key, byte[]? initialIv = null)
        {
            _aes = Aes.Create();
            _aes.Key = key;
            _aes.Mode = CipherMode.CBC;
            _aes.Padding = PaddingMode.PKCS7;
            _aes.IV = initialIv ?? new byte[16];
        }

        public byte[] Decrypt(byte[] data)
        {
            using MemoryStream memoryStream = new();
            using ICryptoTransform decryptor = _aes.CreateDecryptor();
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Write);

            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();

            return memoryStream.ToArray();
        }

        public async Task DecryptAsync(int segmentCnt, Channel<(int, byte[])> inputCh, Channel<byte[]> outputCh, ThreadSafeInt decryptProg)
        {
            int currentSegment = 0;
            var inputBuffer = new Dictionary<int, byte[]>();
            //var rawData = new List<byte>();

            while (currentSegment < segmentCnt)
            {
                while (inputCh.Reader.TryRead(out var input))
                {
                    inputBuffer[input.Item1] = input.Item2;
                }

                while (inputBuffer.ContainsKey(currentSegment))
                {
                    //rawData.AddRange(inputBuffer[currentSegment]);
                    outputCh.Writer.TryWrite(Decrypt(inputBuffer[currentSegment]));
                    inputBuffer.Remove(currentSegment);
                    currentSegment++;
                    decryptProg.Increment();
                }

                await Task.WhenAny(inputCh.Reader.WaitToReadAsync().AsTask(), inputCh.Reader.Completion);
            }

            outputCh.Writer.Complete();

            //var decryptedWhole = Decrypt(rawData.ToArray());
            //File.WriteAllBytes(@"C:\Users\Stratos\Documents\0_projects\PV178\IS_VOD_Downloader\IS_VOD_Downloader\bin\Debug\net6.0\test.ts", decryptedWhole);
        }
    }
}