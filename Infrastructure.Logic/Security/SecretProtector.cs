using System;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Logic.Security
{
    public static class SecretProtector
    {
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("LogoProject.v1");

        public static string Protect(string plain)
        {
            if (string.IsNullOrEmpty(plain)) return null;
            var bytes = Encoding.UTF8.GetBytes(plain);
            var cipher = ProtectedData.Protect(bytes, Entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(cipher);
        }

        public static string Unprotect(string protectedBase64)
        {
            if (string.IsNullOrEmpty(protectedBase64)) return null;
            var cipher = Convert.FromBase64String(protectedBase64);
            var bytes = ProtectedData.Unprotect(cipher, Entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
