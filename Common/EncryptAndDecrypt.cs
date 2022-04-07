using System.Security.Cryptography;
using System.Text;

namespace ImageBed.Common
{
    /// <summary>
    /// 加/解密类
    /// </summary>
    public class EncryptAndDecrypt
    {
        /// <summary>
        /// 生成输入字符串的32位大写MD5
        /// </summary>
        /// <param name="src">源字符串</param>
        /// <returns>输入字符串的32位大写MD5</returns>
        public static string Encrypt_MD5(string src)
        {
            using var md5 = MD5.Create();
            var dst = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(src)));
            return dst.Replace("-", "");
        }
    }
}