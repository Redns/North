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
        public State State { get; set; }                        // 账户状态
        public string Token { get; set; }                       // 令牌
        public ulong TokenExpireTime { get; set; }               // 令牌过期时间
        public Permission Permission { get; set; }              // 用户权限
        public bool IsApiAvailable { get; set; }                // 能否通过API访问
        public ulong MaxUploadNums { get; set; }                 // 最大上传数量（张）
        public ulong MaxUploadCapacity { get; set; }             // 最大存储容量（MB）
        public ulong SingleMaxUploadNums { get; set; }           // 单次最大上传数量（张）
        public ulong SingleMaxUploadCapacity { get; set; }       // 单次最大上传容量（MB）

        public UserEntity(string id, string name, string email, string password, string avatar, State state, string token, ulong tokenExpireTime, Permission permission, bool isApiAvailable, ulong maxUploadNums, ulong maxUploadCapacity, ulong singleMaxUploadNums, ulong singleMaxUploadCapacity)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Avatar = avatar;
            State = state;
            Token = token;
            TokenExpireTime = tokenExpireTime;
            Permission = permission;
            IsApiAvailable = isApiAvailable;
            MaxUploadNums = maxUploadNums;
            MaxUploadCapacity = maxUploadCapacity;
            SingleMaxUploadNums = singleMaxUploadNums;
            SingleMaxUploadCapacity = singleMaxUploadCapacity;
        }


        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <param name="validTime">令牌有效期（ms）</param>
        /// <returns></returns>
        public bool GenerateToken(ulong validTime = 86400000)
        {
            if((State is State.Normal) && IsApiAvailable)
            {
                Token = IdentifyHelper.GenerateId();
                TokenExpireTime = TimeHelper.TimeStamp + validTime;
                return true;
            }
            return false;
        }


        /// <summary>
        /// 判断令牌是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsTokenValid()
        {
            return !string.IsNullOrEmpty(Token) && (TimeHelper.TimeStamp < TokenExpireTime) && IsApiAvailable && (State is State.Normal);
        }


        /// <summary>
        /// 生成 DTO 对象
        /// </summary>
        /// <returns></returns>
        public UserDTOEntity ToDTO()
        {
            return new UserDTOEntity(Name, Email, Avatar, State, Permission,
                                     IsApiAvailable, MaxUploadNums, MaxUploadCapacity,
                                     SingleMaxUploadNums, SingleMaxUploadCapacity);
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
        User = 0,           // 用户
        Administrator,      // 管理员
        System              // 系统
    }


    /// <summary>
    /// 账户状态
    /// </summary>
    public enum State
    {
        Checking = 0,       // 待验证
        Normal,             // 正常
        Forbidden           // 封禁中
    }


    public class UserDTOEntity
    {
        public string Name { get; set; }                        // 用户名
        public string Email { get; set; }
        public string Avatar { get; set; }                      // 头像
        public State State { get; set; }                        // 账户状态
        public Permission Permission { get; set; }              // 用户权限
        public bool IsApiAvailable { get; set; }                // 能否通过API访问
        public ulong MaxUploadNums { get; set; }                 // 最大上传数量（张）
        public ulong MaxUploadCapacity { get; set; }             // 最大存储容量（MB）
        public ulong SingleMaxUploadNums { get; set; }           // 单次最大上传数量（张）
        public ulong SingleMaxUploadCapacity { get; set; }       // 单次最大上传容量（MB）

        public UserDTOEntity(string name, string email, string avatar, State state, Permission permission, bool isApiAvailable, ulong maxUploadNums, ulong maxUploadCapacity, ulong singleMaxUploadNums, ulong singleMaxUploadCapacity)
        {
            Name = name;
            Email = email;
            Avatar = avatar;
            State = state;
            Permission = permission;
            IsApiAvailable = isApiAvailable;
            MaxUploadNums = maxUploadNums;
            MaxUploadCapacity = maxUploadCapacity;
            SingleMaxUploadNums = singleMaxUploadNums;
            SingleMaxUploadCapacity = singleMaxUploadCapacity;
        }
    }
}
