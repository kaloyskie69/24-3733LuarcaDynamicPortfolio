using System;
using System.Security.Cryptography;

namespace PersonalWebsiteProject.App_Code
{
    /// <summary>
    /// Handles secure password hashing and verification using PBKDF2 (Rfc2898DeriveBytes).
    /// Passwords are NEVER stored or compared in plain text.
    /// </summary>
    public static class PasswordHelper
    {
        private const int SaltSize = 16;      // 128-bit salt
        private const int HashSize = 32;      // 256-bit hash
        private const int Iterations = 100000; // PBKDF2 iteration count

        /// <summary>
        /// Generates a new random salt and hashes the given plain-text password.
        /// </summary>
        public static void CreatePasswordHash(string plainPassword, out string hash, out string salt)
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

        /// <summary>
        /// Verifies a plain-text password against a stored hash/salt pair.
        /// </summary>
        public static bool VerifyPassword(string plainPassword, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
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

        /// <summary>
        /// Constant-time string comparison to prevent timing attacks.
        /// </summary>
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
