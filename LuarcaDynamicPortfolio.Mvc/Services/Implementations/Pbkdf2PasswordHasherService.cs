using System;
using System.Security.Cryptography;
using LuarcaDynamicPortfolio.Mvc.Services.Interfaces;

namespace LuarcaDynamicPortfolio.Mvc.Services.Implementations
{
    /// <summary>
    /// PBKDF2 password hashing implementation strictly compatible with the legacy Web Forms
    /// PasswordHelper (100,000 iterations, 16-byte salt, 32-byte hash, SHA-256).
    /// Optimized with hardware-accelerated constant-time comparison and zero-allocation RNG.
    /// </summary>
    public class Pbkdf2PasswordHasherService : IPasswordHasherService
    {
        private const int SaltSize = 16;       // 128-bit salt
        private const int HashSize = 32;       // 256-bit hash
        private const int Iterations = 100000; // PBKDF2 iteration count

        public void CreatePasswordHash(string plainPassword, out string hash, out string salt)
        {
            byte[] saltBytes = new byte[SaltSize];
            RandomNumberGenerator.Fill(saltBytes); // Zero-allocation, cryptographically secure

            byte[] hashBytes = HashPassword(plainPassword, saltBytes);

            salt = Convert.ToBase64String(saltBytes);
            hash = Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string plainPassword, string storedHash, string storedSalt)
        {
            if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
                return false;

            byte[] saltBytes;
            byte[] storedHashBytes;
            try
            {
                saltBytes = Convert.FromBase64String(storedSalt);
                storedHashBytes = Convert.FromBase64String(storedHash);
            }
            catch
            {
                return false;
            }

            byte[] computedHashBytes = HashPassword(plainPassword, saltBytes);

            // Hardware constant-time equality check to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(computedHashBytes, storedHashBytes);
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(HashSize);
        }
    }
}
