namespace North.Core.Helper
{
    /// <summary>
    /// 标识符辅助类
    /// </summary>
    public static class IdentifyHelper
    {
        /// <summary>
        /// Unix 时间戳
        /// </summary>
        public static ulong TimeStamp
        {
            get
            {
                return (ulong)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
        }


        /// <summary>
        /// 身份标识符类型
        /// </summary>
        public enum IdentifyType
        {
            Guid = 0,               // GUID
            GuidWithoutSplit,       // GUID（无分隔符）
            TimeStamp               // 时间戳
        }


        /// <summary>
        /// 身份标识符类型转字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToString(this IdentifyType type)
        {
            return type switch
            {
                IdentifyType.Guid => "Guid",
                IdentifyType.GuidWithoutSplit => "GuidWithoutSplit",
                IdentifyType.TimeStamp => "TimeStamp",
                _ => string.Empty
            };
        }


        /// <summary>
        /// 生成标识符
        /// </summary>
        /// <param name="type">标识符类型</param>
        /// <param name="uniqueCheck">标识符唯一性核验</param>
        /// <returns></returns>
        public static string Generate(IdentifyType type = IdentifyType.GuidWithoutSplit, Func<string, bool>? uniqueCheck = null)
        {
            var id = type switch
            {
                IdentifyType.Guid => Guid.NewGuid().ToString(),
                IdentifyType.GuidWithoutSplit => Guid.NewGuid().ToString().Replace("-", string.Empty),
                IdentifyType.TimeStamp => TimeStamp.ToString(),
                _ => string.Empty
            };
            return ((uniqueCheck is null) || uniqueCheck(id)) ? id : Generate(type, uniqueCheck);
        }
    }
}
