using System;
using System.Security.Cryptography;

namespace BugTrackerWithLayout.Security
{
    public static class PasswordHelper
    {
        // Format: {iterations}.{saltBase64}.{hashBase64}
        public static string HashPassword(string password, int iterations = 100_000)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var salt = new byte[16];
                rng.GetBytes(salt);

                // Not: HashAlgorithmName.SHA256 .NET 4.7+ ile kullanılabilir.
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                {
                    var hash = pbkdf2.GetBytes(32); // 256-bit
                    return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
                }
            }
        }

        public static bool Verify(string password, string stored)
        {
            var parts = stored.Split('.');
            if (parts.Length != 3) return false;

            if (!int.TryParse(parts[0], out var iterations)) return false;
            var salt = Convert.FromBase64String(parts[1]);
            var expected = Convert.FromBase64String(parts[2]);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                var computed = pbkdf2.GetBytes(expected.Length);
                return FixedTimeEquals(computed, expected);
            }
        }

        // .NET Framework uyumlu sabit-zaman karşılaştırma
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
