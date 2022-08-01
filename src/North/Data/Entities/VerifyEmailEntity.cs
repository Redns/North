namespace North.Data.Entities
{
    /// <summary>
    /// 邮箱验证类
    /// </summary>
    public class VerifyEmailEntity
    {
        public string Id { get; set; }              // 验证链接为 verify/{nameof(VerifyType)}/{Id}
        public string Email { get; set; }           // 待验证的邮箱地址
        public long ExpireTime { get; set; }        // 链接到期时间
        public VerifyType VerifyType { get; set; }  // 验证类型

        public VerifyEmailEntity(string id, string email, long expireTime, VerifyType verifyType)
        {
            Id = id;
            Email = email;
            ExpireTime = expireTime;
            VerifyType = verifyType;
        }
    }


    /// <summary>
    /// 验证类型
    /// </summary>
    public enum VerifyType
    {
        Register = 0
    }
}
