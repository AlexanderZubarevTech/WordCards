using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UpdaterLibrary.Token
{
    internal static class Security
    {
        public static string Encrypt(string text, string key)
        {
            var keyBytes = Encoding.ASCII.GetBytes(key);

            return Encrypt(text, keyBytes);
        }

        private static string Encrypt(string text, byte[] key)
        {
            var result = string.Empty;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write, true))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(text);
                        }
                    }

                    result = Convert.ToBase64String(ms.ToArray());
                }
            }

            return result;
        }

        public static string Decrypt(string text, string key)
        {
            var keyBytes = Encoding.ASCII.GetBytes(key);

            string result;

            try
            {
                result = Decrypt(text, keyBytes);
            }
            catch (Exception)
            {
                result = string.Empty;
            }

            return result;
        }

        private static string Decrypt(string base64, byte[] key)
        {
            var result = string.Empty;

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
            {
                byte[] iv = new byte[16];
                ms.Read(iv, 0, iv.Length);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read, true))
                    {
                        using (MemoryStream output = new MemoryStream())
                        {
                            cs.CopyTo(output);

                            result = Encoding.UTF8.GetString(output.ToArray());
                        }
                    }
                }
            }

            return result;
        }
    }
}
