using System.Security.Cryptography;
using System.Text;

namespace app_dental_api.Middlewares
{
    public class EncryptionService
    {
        private readonly byte[] key;

        public EncryptionService(string key)
        {
            this.key = Convert.FromBase64String(key);
        }

        public string Encrypt(string plainText)
        {
            byte[] nonce = RandomNumberGenerator.GetBytes(12);
            byte[] plaintext = Encoding.UTF8.GetBytes(plainText);
            byte[] cipher = new byte[plaintext.Length];
            byte[] tag = new byte[16];

            using var aesGcm = new AesGcm(this.key, 16);
            aesGcm.Encrypt(nonce, plaintext, cipher, tag);

            byte[] result = new byte[nonce.Length + cipher.Length + tag.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(cipher, 0, result, nonce.Length, cipher.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length + cipher.Length, tag.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string base64Input)
        {
            byte[] all = Convert.FromBase64String(base64Input);
            byte[] nonce = all[..12];
            byte[] tag = all[^16..];
            byte[] cipher = all[12..^16];
            byte[] plain = new byte[cipher.Length];

            using var aesGcm = new AesGcm(this.key, tagSizeInBytes: 16);
            aesGcm.Decrypt(nonce, cipher, tag, plain);

            return Encoding.UTF8.GetString(plain);
        }
    }
}
