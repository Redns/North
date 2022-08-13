using System.Text;

namespace North.Core.Helper
{
    /// <summary>
    /// 加密辅助类
    /// </summary>
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
            return BitConverter.ToString(System.Security.Cryptography.MD5.Create()
                                                                         .ComputeHash((encoding ?? Encoding.UTF8).GetBytes(s)))
                                                                         .Replace("-", string.Empty);
        }
    }
}
