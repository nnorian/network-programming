using System;
using System.Security.Cryptography;
using System.Text;

namespace LanChat{
    // task 4: private messages are encrypted with AES-256-GCM
    public static class Crypto{
        // fixed for the chat: every peer must derive the same key without talking first
        static readonly byte[] Salt = Encoding.UTF8.GetBytes("LAN-Chat salt v1");
        const int Iterations = 600000;
        const int KeySize = 32;
        const int NonceSize = 12;
        const int TagSize = 16;

        static byte[] key;

        public static void Init(string passphrase){
            key = Rfc2898DeriveBytes.Pbkdf2(passphrase, Salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        }

        public static Message EncryptPm(string to, string text){
            byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
            byte[] plain = Encoding.UTF8.GetBytes(text);
            byte[] box = new byte[plain.Length + TagSize]; // ciphertext, then the tag

            using (AesGcm aes = new AesGcm(key, TagSize)){
                aes.Encrypt(nonce, plain, box.AsSpan(0, plain.Length), box.AsSpan(plain.Length), Names(Protocol.MyName, to));
            }

            Message m = Protocol.New(Protocol.Pm);
            m.Nonce = Convert.ToBase64String(nonce);
            m.Ciphertext = Convert.ToBase64String(box);
            return m;
        }

        // false when the message was changed, sent to someone else, or made with another key
        public static bool TryDecryptPm(Message m, out string text){
            text = null;
            try{
                byte[] nonce = Convert.FromBase64String(m.Nonce);
                byte[] box = Convert.FromBase64String(m.Ciphertext);
                if (nonce.Length != NonceSize || box.Length < TagSize) return false;

                int n = box.Length - TagSize;
                byte[] plain = new byte[n];
                using (AesGcm aes = new AesGcm(key, TagSize)){
                    aes.Decrypt(nonce, box.AsSpan(0, n), box.AsSpan(n), plain, Names(m.From, Protocol.MyName));
                }
                text = Encoding.UTF8.GetString(plain);
                return Protocol.IsValidText(text);
            }
            catch (FormatException){
                return false; // not base64
            }
            catch (CryptographicException){
                return false; // the tag does not match
            }
        }

        // authenticated, not encrypted: the sender and receiver cannot be swapped
        static byte[] Names(string from, string to){
            return Encoding.UTF8.GetBytes(from + ">" + to);
        }
    }
}