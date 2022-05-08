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
        /// MD5加密（16位）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5Encrypt16(string str)
        {
            return BitConverter.ToString(MD5.Create()
                                            .ComputeHash(Encoding.Default.GetBytes(str)), 4, 8)
                                            .Replace("-", "");
        }


        /// <summary>
        /// MD5加密（32位）
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <returns></returns>
        public static string MD5Encrypt32(string str)
        {
            return BitConverter.ToString(MD5.Create()
                                            .ComputeHash(Encoding.UTF8.GetBytes(str)))
                                            .Replace("-", "");
        }
    }
}