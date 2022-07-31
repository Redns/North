using Newtonsoft.Json;
using North.Common;

namespace North.Data.Entities
{
    public class UserEntity
    {
        public string Id { get; set; }                          // 用户 ID
        public string Name { get; set; }                        // 用户名
        public string Email { get; set; }
        public string Password { get; set; }                    // 密码
        public string Avatar { get; set; }                      // 头像
        public bool IsForbidden { get; set; }                   // 是否被封禁
        public string Token { get; set; }                       // 令牌
        public long TokenExpireTime { get; set; }               // 令牌过期时间
        public Permission Permission { get; set; }              // 用户权限
        public bool IsApiAvailable { get; set; }                // 能否通过API访问
        public long MaxUploadNums { get; set; }                 // 最大上传数量（张）
        public long MaxUploadCapacity { get; set; }             // 最大存储容量（MB）

        public UserEntity(string id, string name, string email, string password, string avatar, bool isForbidden, string token, long tokenExpireTime, Permission permission, bool isApiAvailable, long maxUploadNums, long maxUploadCapacity)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Avatar = avatar;
            IsForbidden = isForbidden;
            Token = token;
            TokenExpireTime = tokenExpireTime;
            Permission = permission;
            IsApiAvailable = isApiAvailable;
            MaxUploadNums = maxUploadNums;
            MaxUploadCapacity = maxUploadCapacity;
        }


        /// <summary>
        /// 判断令牌是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsTokenValid()
        {
            return !string.IsNullOrEmpty(Token) && (TimeHelper.TimeStamp < TokenExpireTime);
        }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }


    /// <summary>
    /// 用户权限
    /// </summary>
    public enum Permission
    {
        Visitor = 0,        // 游客
        User,               // 用户
        Administrator,      // 管理员
        System              // 系统
    }
}
