// task 4
// private messages
// encrypted with AES-256-GCM

using System;
using System.Security.Cryptography;
using System.Text;

namespace LanChat{
    public static class Crypto{

        static readonly byte[] Salt = Encoding.UTF8.GetBytes("LAN-Chat salt v1");
        const int Iterations = 600000;
        const int KeySize = 32;
        const int NonceSize = 12;
        const int TagSize = 16;

        static byte[] key;

        public static void Init(string passphrase){
            key = Rfc2898DeriveBytes.Pbkdf2(passphrase, Salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        }

        public static bool Encrypt(string to, string text, out string nonceBase64, out string cipherBase64){
            nonceBase64 = null;
            cipherBase64 = null;
            if (key == null) return false;

            byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
            byte[] plaintext = Encoding.UTF8.GetBytes(text);
            byte[] ciphertext = new byte[plaintext.Length];
            byte[] tag = new byte[TagSize];
            byte[] ad = Encoding.UTF8.GetBytes(Protocol.MyName + ">" + to);

            using (var aes = new AesGcm(key, TagSize)){
                aes.Encrypt(nonce, plaintext, ciphertext, tag, ad);
            }

            // wire format: ciphertext followed by tag
            byte[] combined = new byte[ciphertext.Length + TagSize];
            Buffer.BlockCopy(ciphertext, 0, combined, 0, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, combined, ciphertext.Length, TagSize);

            nonceBase64 = Convert.ToBase64String(nonce);
            cipherBase64 = Convert.ToBase64String(combined);
            return true;
        }

        public static bool Decrypt(string from, string nonceBase64, string cipherBase64, out string plaintext){
            plaintext = null;
            if (key == null) return false;

            try{
                byte[] nonce = Convert.FromBase64String(nonceBase64);
                byte[] combined = Convert.FromBase64String(cipherBase64);
                if (combined.Length < TagSize || nonce.Length != NonceSize) return false;

                int cipherLen = combined.Length - TagSize;
                byte[] ciphertext = new byte[cipherLen];
                byte[] tag = new byte[TagSize];
                Buffer.BlockCopy(combined, 0, ciphertext, 0, cipherLen);
                Buffer.BlockCopy(combined, cipherLen, tag, 0, TagSize);

                byte[] ad = Encoding.UTF8.GetBytes(from + ">" + Protocol.MyName);
                byte[] decrypted = new byte[cipherLen];

                using (var aes = new AesGcm(key, TagSize)){
                    aes.Decrypt(nonce, ciphertext, tag, decrypted, ad);
                }

                plaintext = Encoding.UTF8.GetString(decrypted);
                return true;
            }
            catch{
                return false;
            }
        }
    }
}
