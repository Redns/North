using System.Text;

namespace North.Core.Helpers
{
    /// <summary>
    /// 字符串辅助类
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// 判断字符串 s 是否包含数组 values 中的任意元素
        /// </summary>
        /// <param name="s"></param>
        /// <param name="values"></param>
        /// <param name="perfectMatch">若为 true，则只有当字符串 s 与数组中的某个元素相等时，才会返回 true</param>
        /// <returns></returns>
        public static bool Contains(this string s, string[] values, bool perfectMatch = false)
        {
            if (perfectMatch)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (s == values[i]) { return true; }
                }
            }
            else
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (s.Contains(values[i])) { return true; }
                }
            }
            return false;
        }


        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns></returns>
        public static string FormatString(this string s, int maxLength = 60)
        {
            if (s.Length <= maxLength)
            {
                return s;
            }
            else
            {
                var formatDescription = s[..(maxLength - 3)];
                var lastSpliterIndex = formatDescription.Length;
                for (int i = 0; i < formatDescription.Length; i++)
                {
                    if (formatDescription[i] == ' ' || formatDescription[i] == ',')
                    {
                        lastSpliterIndex = i;
                    }
                }
                return $"{formatDescription[0..lastSpliterIndex]}...";
            }
        }


        public static string FormatNumber(this long l)
        {
            if (l < 10000)
            {
                return l.ToString();
            }
            else
            {
                var result = new StringBuilder();
                var numberStr = l.ToString();
                var numberStrLen = numberStr.Length;
                for (int i = 0; i < numberStrLen; i++)
                {
                    result.Append(numberStr[i]);
                    if ((numberStrLen - i - 1) % 4 == 0 && i != numberStr.Length - 1)
                    {
                        result.Append(',');
                        result.Append(' ');
                    }
                }
                return result.ToString();
            }
        }
    }
}
