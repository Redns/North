namespace North.Core.Entities
{
    /// <summary>
    /// 邮箱验证类
    /// </summary>
    public class VerifyEmailEntity
    {
        /// <summary>
        /// 验证链接为 verify?type={nameof(VerifyType)}&id={Id}
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 待验证的邮箱地址
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 链接到期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }

        /// <summary>
        /// 验证类型
        /// </summary>
        public VerifyType VerifyType { get; set; }

        public VerifyEmailEntity(string id, string email, DateTime expireTime, VerifyType verifyType)
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
