namespace North.Common
{
    public static class IdentifyHelper
    {
        /// <summary>
        /// Unix 时间戳
        /// </summary>
        public static long TimeStamp
        {
            get
            {
                return (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
        }


        /// <summary>
        /// 生成唯一标识符
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GenerateId(IdentifyType type = IdentifyType.GuidWithoutSplit)
        {
            return type switch
            {
                IdentifyType.Guid => Guid.NewGuid().ToString(),
                IdentifyType.GuidWithoutSplit => Guid.NewGuid().ToString().Replace("-", string.Empty),
                IdentifyType.TimeStamp => TimeStamp.ToString(),
                _ => string.Empty
            };
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
}
