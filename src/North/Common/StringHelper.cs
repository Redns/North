namespace North.Common
{
    public static class StringHelper
    {
        /// <summary>
        /// 判断字符串 s 是否包含数组 values 中的任意元素
        /// </summary>
        /// <param name="s"></param>
        /// <param name="values"></param>
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
    }
}
