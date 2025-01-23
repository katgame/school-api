using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

namespace school_api.Data.Services
{
    public static class Encryption
    {
        public static string Decrypt(string encryptedText, string key)
        {
            // Split the encrypted text into IV and encrypted content
            string[] parts = encryptedText.Split(':');
            if (parts.Length != 2)
                throw new ArgumentException("Invalid encrypted data format");

            byte[] iv = StringToByteArray(parts[0]); // IV from the first part
            byte[] encryptedBytes = StringToByteArray(parts[1]); // Encrypted content from the second part

            // Ensure the key is 24 bytes for AES-192
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            if (keyBytes.Length != 24)
                throw new ArgumentException("Encryption key must be 24 characters for AES-192");

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    using (MemoryStream ms = new MemoryStream(encryptedBytes))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd(); // Decrypt and return the plaintext
                            }
                        }
                    }
                }
            }
        }

        private static byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}
