using System.IO;
using System.Security.Cryptography;
using System.Text;
using DamnLibrary.Utilities.Extensions;

namespace DamnLibrary.Utilities
{
    public static class EncryptionUtilities
    {
        public static byte[] Encrypt(this byte[] bytes, string key)
        {
            var keyBytes = MD5.HashData(Encoding.UTF8.GetBytes(key));
            var ivBytes = MD5.HashData(Encoding.UTF8.GetBytes(key.Reverse()));
            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            
            var encryptor = aes.CreateEncryptor();
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            return memoryStream.ToArray();
        }
        
        public static byte[] Decrypt(this byte[] bytes, string key)
        {
            var keyBytes = MD5.HashData(Encoding.UTF8.GetBytes(key));
            var ivBytes = MD5.HashData(Encoding.UTF8.GetBytes(key.Reverse()));
            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = ivBytes;

            var decryptor = aes.CreateDecryptor();
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            return memoryStream.ToArray();
        }
    }
}