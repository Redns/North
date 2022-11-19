using SqlSugar;

namespace North.Core.Entities
{
    /// <summary>
    /// 邮箱验证类
    /// </summary>
    public class EmailEntity : Entity
    {
        /// <summary>
        /// 待验证的邮箱地址
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 链接到期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }

        /// <summary>
        /// 验证类型（验证链接为 verify?type={nameof(VerifyType)}&id={Id}）
        /// </summary>
        public VerifyType VerifyType { get; set; }
    }


    /// <summary>
    /// 验证类型
    /// </summary>
    public enum VerifyType
    {
        Register = 0
    }
}
