// task 4
// private messages
// encrypted with AES-256-GCM

using System;
using System.Security.Cryptography;
using System.Text;

namespace LanChat{
    
    static readonly byte[] Salt = Encoding.UTF8.GetBytes("Lan-Chat salt v1");
    const int Iterations = 600000;
    const int KeySize = 32;
    const int NonceSize = 12;
    const int Tagsize = 16;

    static byte[] key;

    public static EncryptPm(string to, string text){

        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
        
    }
}