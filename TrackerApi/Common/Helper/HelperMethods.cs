using System.Security.Cryptography;
using System.Text;

namespace TrackerApi.Common.Helper
{
    public class HelperMethods
    {

        public static string CreateEncryptKey(int size)
        {
            // Generate a cryptographic random number
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        public static string CreateEncryptHash(string text, string key, string format = "SHA1")
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(key))
                return string.Empty;

            string saltAndPassword = String.Concat(text, key);
            var hashAlgorithm = HashAlgorithm.Create(format);
            var hashByteArray = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(saltAndPassword));
            return BitConverter.ToString(hashByteArray).Replace("-", String.Empty);
        }


    }
}
