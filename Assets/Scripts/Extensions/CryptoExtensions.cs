using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class CryptoExtensions
{
    private static readonly string PasswordSalt = "MetelskyiVsevolod";
    private const int KeySize = 256;
    private const int Iterations = 1000;

    public static string EncryptString(this string plainText, string password)
    {
        var saltBytes = Encoding.UTF8.GetBytes(PasswordSalt);
        var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations);

        using (var aes = Aes.Create())
        {
            aes.KeySize = KeySize;
            aes.Key = pbkdf2.GetBytes(aes.KeySize / 8);
            aes.IV = pbkdf2.GetBytes(aes.BlockSize / 8);

            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                var bytes = Encoding.UTF8.GetBytes(plainText);
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }

    public static string DecryptString(this string encryptedText, string password)
    {
        var saltBytes = Encoding.UTF8.GetBytes(PasswordSalt);
        var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations);

        using (var aes = Aes.Create())
        {
            aes.KeySize = KeySize;
            aes.Key = pbkdf2.GetBytes(aes.KeySize / 8);
            aes.IV = pbkdf2.GetBytes(aes.BlockSize / 8);

            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                var bytes = Convert.FromBase64String(encryptedText);
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }
}
