using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace System
{
    public class AesCrypto
    {
        private string PrivateKey;
        private string IV;

        private string GenerateString(int count)
        {
            int number;
            string checkCode = String.Empty;

            var random = new Random();

            for (int i = 0; i < count; i++) 
            {
                number = random.Next();
                number = number % 36;
                if (number < 10)
                {
                    number += 48;
                }
                else
                {
                    number += 55; 
                }

                checkCode += ((char)number).ToString();
            }
            return checkCode;
        }

        public AesCrypto(string privateKey = null, string iv = null)
        {
            var rnd = new Random();
            if (privateKey == null)
                privateKey = GenerateString(32);
            if (iv == null)
                iv = GenerateString(16);
            PrivateKey = privateKey;
            IV = iv;
        }

        public string Decrypt(string encryptStr)
        {
            byte[] bKey = Encoding.UTF8.GetBytes(PrivateKey);
            byte[] bIV = Encoding.UTF8.GetBytes(IV);
            byte[] byteArray = Convert.FromBase64String(encryptStr);

            string decrypt = null;
#if !NET451
            var aes = System.Security.Cryptography.Aes.Create();
#else
            var aes = Rijndael.Create();
#endif
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        decrypt = Encoding.UTF8.GetString(mStream.ToArray(), 0, mStream.ToArray().Count());
                    }
                }
            }
            catch { }
#if NET451
            aes.Clear();
#endif

            return decrypt;
        }

        public string Encrypt(string plainStr)
        {
            byte[] bKey = Encoding.UTF8.GetBytes(PrivateKey);
            byte[] bIV = Encoding.UTF8.GetBytes(IV);
            byte[] byteArray = Encoding.UTF8.GetBytes(plainStr);

            string encrypt = null;
#if !NET451
            var aes = System.Security.Cryptography.Aes.Create();
#else
            var aes = Rijndael.Create();
#endif
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        encrypt = Convert.ToBase64String(mStream.ToArray());
                    }
                }
            }
            catch { }
#if NET451
            aes.Clear();
#endif
            return encrypt;
        }
    }
}
