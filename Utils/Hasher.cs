using System;
using System.Security.Cryptography;

namespace Utils
{
    /// <summary>
    /// Utility for hashing and verifying passwords using PBKDF2 (HMAC-SHA256).
    /// Stored format: {iterations}.{saltBase64}.{hashBase64}
    /// </summary>
    public static class Hasher
    {
        private const int SaltSize = 16;               // 128 bits
        private const int HashSize = 32;               // 256 bits
        private const int DefaultIterations = 100_000; // tune this for your environment

        /// <summary>
        /// Hash a password. Returns a string containing iterations, salt and hash.
        /// </summary>
        public static string HashPassword(string password, int iterations = DefaultIterations)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));
            if (iterations <= 0) throw new ArgumentOutOfRangeException(nameof(iterations));

            var salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verify a password against a stored hash. Returns true if valid.
        /// Also outputs whether the stored hash should be upgraded (e.g. uses fewer iterations).
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(storedHash)) return false;

            var parts = storedHash.Split('.');
            if (parts.Length != 3) return false;

            if (!int.TryParse(parts[0], out var iterations)) return false;

            byte[] salt, expectedHash;
            try
            {
                salt = Convert.FromBase64String(parts[1]);
                expectedHash = Convert.FromBase64String(parts[2]);
            }
            catch (FormatException)
            {
                return false;
            }

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var actualHash = pbkdf2.GetBytes(expectedHash.Length);

            var verified = CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);

            return verified;
        }
    }
}