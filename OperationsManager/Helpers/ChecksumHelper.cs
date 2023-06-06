using System.Security.Cryptography;
using System.Text;

namespace OperationsManager.Helpers
{
    public class ChecksumHelper
    {
        public static string ComputeMD5(string input)
        {
            var sb = new StringBuilder();
            using (var md5 = MD5.Create())
            {
                byte[] hashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                foreach (byte b in hashValue)
                {
                    sb.Append($"{b:X2}");
                }
            }
            return sb.ToString();
        }
    }
}
