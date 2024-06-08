using System.Security.Cryptography;

namespace BusinessLayer.Common.Utils
{
    public class AES
    {
        private static byte[] aesKey = new byte[] {
            0xd4, 0xf7, 0x98, 0x7b, 0x7d, 0x8e, 0x3e, 0x3c,
            0xb9, 0x1e, 0xab, 0xe7, 0xb8, 0xe9, 0x17, 0x4f,
            0x4b, 0x7d, 0x83, 0x4e, 0x25, 0xb2, 0x8e, 0xdf,
            0xb5, 0x6f, 0x2e, 0xaf, 0x8d, 0xca, 0xb5, 0x7d
        };

        private static byte[] iv = new byte[] {
            0x12, 0xa7, 0x4a, 0x9e, 0xbc, 0x9e, 0x45, 0xd3,
            0xc2, 0x3e, 0x1c, 0x64, 0xa8, 0xcd, 0xea, 0xbc
        };


        public static string EncryptString(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = aesKey;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string DecryptString(string cipherText, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
}
