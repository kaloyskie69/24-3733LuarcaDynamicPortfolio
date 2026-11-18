using System;
using System.Security.Cryptography;
using LuarcaDynamicPortfolio.Mvc.Services.Interfaces;

namespace LuarcaDynamicPortfolio.Mvc.Services.Implementations
{
    /// <summary>
    /// PBKDF2 password hashing implementation strictly compatible with the legacy Web Forms
    /// PasswordHelper (100,000 iterations, 16-byte salt, 32-byte hash, SHA-256).
    /// </summary>
    public class Pbkdf2PasswordHasherService : IPasswordHasherService
    {
        private const int SaltSize = 16;       // 128-bit salt
        private const int HashSize = 32;       // 256-bit hash
        private const int Iterations = 100000; // PBKDF2 iteration count

        public void CreatePasswordHash(string plainPassword, out string hash, out string salt)
        {
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            byte[] hashBytes = HashPassword(plainPassword, saltBytes);

            salt = Convert.ToBase64String(saltBytes);
            hash = Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string plainPassword, string storedHash, string storedSalt)
        {
            if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
                return false;

            byte[] saltBytes;
            try
            {
                saltBytes = Convert.FromBase64String(storedSalt);
            }
            catch
            {
                return false;
            }

            byte[] computedHash = HashPassword(plainPassword, saltBytes);
            string computedHashStr = Convert.ToBase64String(computedHash);

            return SlowEquals(computedHashStr, storedHash);
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        private static bool SlowEquals(string a, string b)
        {
            if (a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }
}
