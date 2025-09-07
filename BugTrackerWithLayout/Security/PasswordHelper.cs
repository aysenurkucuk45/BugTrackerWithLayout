using System;
using System.Security.Cryptography;

namespace BugTrackerWithLayout.Security
{
    public static class PasswordHelper
    {
        // Format: iterations.saltBase64.hashBase64
        public static string HashPassword(string password, int iterations = 100000, int saltSize = 16, int keySize = 32)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var salt = new byte[saltSize];
                rng.GetBytes(salt);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                {
                    var key = pbkdf2.GetBytes(keySize);
                    return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
                }
            }
        }

        public static bool Verify(string password, string stored)
        {
            var parts = stored?.Split('.');
            if (parts == null || parts.Length != 3) return false;

            if (!int.TryParse(parts[0], out var iterations)) return false;
            var salt = Convert.FromBase64String(parts[1]);
            var hash = Convert.FromBase64String(parts[2]);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                var computed = pbkdf2.GetBytes(hash.Length);

                // .NET Framework uyumlu sabit süreli karşılaştırma
                if (computed.Length != hash.Length) return false;
                int diff = 0;
                for (int i = 0; i < hash.Length; i++)
                    diff |= computed[i] ^ hash[i];

                return diff == 0;
            }
        }
    }
}
