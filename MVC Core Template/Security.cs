using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce
{
    public class Security
    {
        public static byte[] CreateSalt()
        {
            var buffer = new byte[16];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buffer);
            return buffer;
        }

        public static byte[] HashPassword(string password, byte[] salt)
        {
            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 2; // 1 cores
                argon2.Iterations = 2;
                argon2.MemorySize = 64000; // 64 MB

                return argon2.GetBytes(16);
            }
        }

        public static bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = HashPassword(password, salt);
            return hash.SequenceEqual(newHash);
        }

        private static string SitePassword
        {
            get
            {
                return Configuration.configuration["Encryption:Password"] ?? "";
            }
        }

        private static string SiteIV
        {
            get
            {
                return Configuration.configuration["Encryption:IV"] ?? "";
            }
        }

        public static string Encrypt(string Input)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = new PasswordDeriveBytes(SitePassword, null).GetBytes(32);
                    aesAlg.IV = Encoding.UTF8.GetBytes(SiteIV);

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(Input);
                            }
                            return Convert.ToBase64String(msEncrypt.ToArray());
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Ecommerce > Security > Encrypt " + Ex.Message);
            }
        }

        public static string Decrypt(string Input)
        {
            try
            {
                byte[] InputBytes = Convert.FromBase64String(Input);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = new PasswordDeriveBytes(SitePassword, null).GetBytes(32);
                    aesAlg.IV = Encoding.UTF8.GetBytes(SiteIV);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(InputBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Ecommerce > Security > Dencrypt " + Ex.Message);
            }

        }
    }
}
