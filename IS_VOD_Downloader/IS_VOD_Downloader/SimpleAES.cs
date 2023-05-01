using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;

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
        using MemoryStream memoryStream = new MemoryStream();
        using ICryptoTransform decryptor = _aes.CreateDecryptor();
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);

        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();

        int decryptedDataLength = (int)memoryStream.Length;
        byte[] decryptedData = ArrayPool<byte>.Shared.Rent(decryptedDataLength);
        memoryStream.Position = 0;
        memoryStream.Read(decryptedData, 0, decryptedDataLength);

        return decryptedData;
    }
}