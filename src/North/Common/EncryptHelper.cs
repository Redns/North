using System.Text;

namespace North.Common
{
    public static class EncryptHelper
    {
        /// <summary>
        /// MD5 加密(32位大写)
        /// </summary>
        /// <param name="s">待加密的字符串</param>
        /// <param name="encoding">字符串编码</param>
        /// <returns></returns>
        public static string MD5(this string s, Encoding? encoding = null)
        {
            if (encoding is null) { encoding = Encoding.UTF8; }
            return BitConverter.ToString(System.Security.Cryptography.MD5.Create()
                                                                         .ComputeHash(encoding.GetBytes(s)))
                                                                         .Replace("-", string.Empty);
        }
    }
}
