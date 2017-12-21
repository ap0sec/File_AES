using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace AES
{
    class AES
    {
        static void Main(string[] args)
        {
            string key;
            string iv;

            string[] files = Directory.GetFiles(@".", "*", SearchOption.AllDirectories);
            int i;
            Create_Key(out iv, out key);

            for (i = 0; i < files.Length; i++)
            {
                Console.WriteLine(files[i]);

                using (System.IO.FileStream in_stream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                using (System.IO.FileStream out_stream = new FileStream(files[i] + ".aes", FileMode.Create, FileAccess.Write))
                {
                    byte[] bs = new byte[0x1000];
                    for(;;)
                    {
                        int readSize = in_stream.Read(bs, 0, bs.Length);
                        if (readSize == 0)
                            break;

                        //byte型の配列の中身をstringに変えて暗号化する
                        string text = System.Text.Encoding.ASCII.GetString(bs);
                        Encrypt(text, iv, key);

                        bs = Encoding.ASCII.GetBytes(text);

                        out_stream.Write(bs, 0, bs.Length);

                    }
                }
            }
        }

        static void Create_Key(out string iv, out string key)
        {
            int BLOCK_SIZE = 128;
            int KEY_SIZE = 128;

            var csp = new AesCryptoServiceProvider();

            csp.BlockSize = BLOCK_SIZE;
            csp.KeySize = KEY_SIZE;
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;

            csp.GenerateIV();
            csp.GenerateKey();

            Console.WriteLine(Convert.ToBase64String(csp.IV));
            Console.WriteLine(Convert.ToBase64String(csp.Key));

            iv = Convert.ToBase64String(csp.IV);
            key = Convert.ToBase64String(csp.Key);
        }

        static string Encrypt(string prainText, string iv, string key)
        {
            string ciphertext = string.Empty;

            int BLOCK_SIZE = 128;
            int KEY_SIZE = 128;

            var csp = new AesCryptoServiceProvider();
            csp.BlockSize = BLOCK_SIZE;
            csp.KeySize = KEY_SIZE;
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            csp.IV = Convert.FromBase64String(iv);
            csp.Key = Convert.FromBase64String(key);

            using (var outms = new MemoryStream())
            using (var encryptor = csp.CreateEncryptor())
            using (var cs = new CryptoStream(outms, encryptor, CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cs))
            {
                writer.Write(prainText);
                ciphertext = Convert.ToBase64String(outms.ToArray());
            }

            return ciphertext;
        }

    } 
}
