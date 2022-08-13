namespace North.Core.Helper
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
    }
}
