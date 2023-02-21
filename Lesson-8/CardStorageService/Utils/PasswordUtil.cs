using CardStorageService.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace CardStorageService.Utils;

public class PasswordUtil
{
    public static (string passwordSalt, string passwordHash) CreatePasswordHash(string password, string secretKey)
    {
        //Generate random salt
        byte[] buffer = new byte[16];
        RandomNumberGenerator.Fill(buffer);

        string passwordSalt = Convert.ToBase64String(buffer);

        //Call method for give password hash
        string passwordHash = GetPasswordHash(password, passwordSalt, secretKey);

        return (passwordSalt, passwordHash);
    }

    public static bool VerifyPassword(string password, string passwordSalt, string passworHash, string secretKey)
    {
        return GetPasswordHash(password, passwordSalt, secretKey) == passworHash;
    }

    public static string GetPasswordHash(string password, string passwordSalt, string secretKey)
    {
        //create string password
        password = $"{password}~{passwordSalt}~{secretKey}";
        byte[] buffer = Encoding.UTF8.GetBytes(password);

        //get hash
        SHA512 sha512 = new SHA512Managed();
        byte[] passwordHash = sha512.ComputeHash(buffer);

        return Convert.ToBase64String(passwordHash);
    }
}