using System.Text;

namespace Services.Hash
{
    public class Vignere
    {
        private string key;
        public Vignere(string key)
        {
            this.key = key;
        }
        
        private string TextToHex(string input)
        {
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                sb.Append(((int)c).ToString("X2"));
            }
            return sb.ToString();
        }

        private string HexToText(string hexInput)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < hexInput.Length; i += 2)
            {
                int code = Convert.ToInt32(hexInput.Substring(i, 2), 16);
                sb.Append((char)code);
            }
            return sb.ToString();
        }

        public string GenerateKey(string str)
        {
            StringBuilder temp = new StringBuilder();

            if (str.Length < key.Length)
            {
                return key.Substring(0, str.Length);
            }
            int x = key.Length;

            for (int i = 0; ; i++)
            {
                if (x == i)
                    i = 0;
                if (temp.Length == str.Length)
                    break;
                temp.Append(key[i]);
            }
            return temp.ToString();
        }

        public string Encrypt(string str)
        {
            string keyUse = GenerateKey(str);
            string cipher_text = "";

            for (int i = 0; i < str.Length; i++)
            {
                int x = Convert.ToInt32(str[i].ToString(), 16);
                int k = Convert.ToInt32(keyUse[i].ToString(), 16);

                int encryptedValue = (x + k) % 16;

                cipher_text += encryptedValue.ToString("X");
            }
            return cipher_text;
        }

        public string Decrypt(string cipher_text)
        {
            string keyUse = GenerateKey(cipher_text);
            string orig_text = "";

            for (int i = 0; i < cipher_text.Length; i++)
            {
                int x = Convert.ToInt32(cipher_text[i].ToString(), 16);
                int k = Convert.ToInt32(keyUse[i].ToString(), 16);

                int decryptedValue = (x - k + 16) % 16;

                orig_text += decryptedValue.ToString("X");
            }
            return orig_text;
        }
    }
}