using System.Security.Cryptography;
using System.Text;

namespace KiloMart.Domain.Register.Utils;

public static class HashHandler
{
    public static string GetHash(string text)
    {
        using HashAlgorithm algorithm = SHA256.Create();
        byte[] bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(text));
        return Convert.ToBase64String(bytes);
    }
}
