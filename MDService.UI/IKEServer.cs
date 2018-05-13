using IKE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MDService.UI
{
    public class IKEServer
    {
        BigInteger prime;
        BigInteger mine;
        public byte[] key;

        public byte[] GetKey()
        {
            return key;
        }

        public string GenerateRequest()
        {
            var _strongRng = new StrongNumberProvider();
            prime = BigInteger.GenPseudoPrime(256, 30, _strongRng);
            mine = BigInteger.GenPseudoPrime(256, 30, _strongRng);
            var g = (BigInteger)7;
            StringBuilder rep = new StringBuilder();
            rep.Append(prime.ToString(36));
            rep.Append(" ");
            var send = g.ModPow(mine, prime);
            rep.Append(send.ToString(36));
            return rep.ToString();
        }

        public void HandleResponse(string response)
        {
            var given = new BigInteger(response, 36);
            var key = given.ModPow(mine, prime);
            this.key = key.GetBytes();
        }
        public string Encrypt(string message)
        {
            return Encrypt(message, this.key);
        }
        public string Encrypt(string message,byte [] key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            var byteHash = hashMD5Provider.ComputeHash(key);
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
            var byteBuff = Encoding.UTF8.GetBytes(message);

            string encoded =
                Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return encoded;
        }

        public string Decrypt(string encryptedMessage)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            var byteHash = hashMD5Provider.ComputeHash(key);
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
            var byteBuff = Convert.FromBase64String(encryptedMessage);

            string plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return plaintext;
        }

    }
}
