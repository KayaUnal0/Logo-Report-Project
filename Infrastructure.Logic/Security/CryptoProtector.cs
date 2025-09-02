using System;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Logic.Security
{
    // Key derivation: PBKDF2-HMAC-SHA256 (built-in Rfc2898DeriveBytes)
    // Cipher: AES-GCM (authenticated encryption)
    // Output format: Base64 of [salt(16)][nonce(12)][cipher(N)][tag(16)]
    public static class CryptoProtector
    {
        private const int SaltSize = 16;         // bytes (128-bit salt)
        private const int NonceSize = 12;        // bytes (AES-GCM nonce)
        private const int TagSize = 16;          // bytes (AES-GCM tag)
        private const int KeySize = 32;          // bytes (256-bit key)
        private const int Iterations = 100_000;  // PBKDF2 work factor

        public static string Encrypt(string plaintext, string masterSecret)
        {
            if (plaintext is null) return null;

            byte[] plain = Encoding.UTF8.GetBytes(plaintext);
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] key = DeriveKey(masterSecret, salt);
            byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);

            byte[] cipher = new byte[plain.Length];
            byte[] tag = new byte[TagSize];

            using (var aes = new AesGcm(key))
            {
                aes.Encrypt(nonce, plain, cipher, tag);
            }

            var output = new byte[salt.Length + nonce.Length + cipher.Length + tag.Length];
            Buffer.BlockCopy(salt, 0, output, 0, salt.Length);
            Buffer.BlockCopy(nonce, 0, output, salt.Length, nonce.Length);
            Buffer.BlockCopy(cipher, 0, output, salt.Length + nonce.Length, cipher.Length);
            Buffer.BlockCopy(tag, 0, output, salt.Length + nonce.Length + cipher.Length, tag.Length);

            Array.Clear(key, 0, key.Length);
            return Convert.ToBase64String(output);
        }

        public static string Decrypt(string base64, string masterSecret)
        {
            if (string.IsNullOrWhiteSpace(base64)) return null;
            byte[] input = Convert.FromBase64String(base64);

            if (input.Length < SaltSize + NonceSize + TagSize)
                throw new CryptographicException("Ciphertext too short.");

            var salt = new byte[SaltSize];
            var nonce = new byte[NonceSize];
            Buffer.BlockCopy(input, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(input, SaltSize, nonce, 0, NonceSize);

            int cipherLen = input.Length - SaltSize - NonceSize - TagSize;
            var cipher = new byte[cipherLen];
            var tag = new byte[TagSize];
            Buffer.BlockCopy(input, SaltSize + NonceSize, cipher, 0, cipherLen);
            Buffer.BlockCopy(input, SaltSize + NonceSize + cipherLen, tag, 0, TagSize);

            byte[] key = DeriveKey(masterSecret, salt);
            var plain = new byte[cipherLen];

            using (var aes = new AesGcm(key))
            {
                aes.Decrypt(nonce, cipher, tag, plain);
            }

            Array.Clear(key, 0, key.Length);
            return Encoding.UTF8.GetString(plain);
        }

        private static byte[] DeriveKey(string masterSecret, byte[] salt)
        {
            if (string.IsNullOrWhiteSpace(masterSecret))
                throw new InvalidOperationException("Missing master secret (env var).");
            using var pbkdf2 = new Rfc2898DeriveBytes(masterSecret, salt, Iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(KeySize);
        }
    }
}
