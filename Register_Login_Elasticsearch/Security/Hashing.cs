
using System.Security.Cryptography;
using System.Text;

namespace Register_Login_Elasticsearch.Security
{
    public class Hashing 
    {
        public static string ToSHA256(string e)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(e));

            var sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
