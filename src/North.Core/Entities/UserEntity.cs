using Newtonsoft.Json;
using North.Core.Helpers;

namespace North.Core.Entities
{
    public class UserEntity
    {
        /// <summary>
        /// 用户 ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 加密后的密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 账户状态
        /// </summary>
        public State State { get; set; }

        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 令牌过期时间
        /// </summary>
        public long TokenExpireTime { get; set; }

        /// <summary>
        /// 用户权限
        /// </summary>
        public Permission Permission { get; set; }

        /// <summary>
        /// 能否通过API访问
        /// </summary>
        public bool IsApiAvailable { get; set; }

        /// <summary>
        /// 最大上传数量（张）
        /// </summary>
        public long MaxUploadNums { get; set; }

        /// <summary>
        /// 最大存储容量（MB）
        /// </summary>
        public double MaxUploadCapacity { get; set; }

        /// <summary>
        /// 单次最大上传数量（张）
        /// </summary>
        public long SingleMaxUploadNums { get; set; }

        /// <summary>
        /// 单次最大上传容量（MB）
        /// </summary>
        public double SingleMaxUploadCapacity { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public string RegisterTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public UserEntity(string id, string name, string email, string password, string avatar, State state, string token, long tokenExpireTime, Permission permission, bool isApiAvailable, long maxUploadNums, double maxUploadCapacity, long singleMaxUploadNums, double singleMaxUploadCapacity)
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
        public bool GenerateToken(long validTime = 86400000)
        {
            if (State is State.Normal && IsApiAvailable)
            {
                Token = IdentifyHelper.Generate();
                TokenExpireTime = IdentifyHelper.TimeStamp + validTime;
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
            return !string.IsNullOrEmpty(Token) && IdentifyHelper.TimeStamp < TokenExpireTime && IsApiAvailable && State is State.Normal;
        }


        /// <summary>
        /// 生成 DTO 对象
        /// </summary>
        /// <returns></returns>
        public UserDTOEntity ToDTO()
        {
            return new UserDTOEntity(Id, Name, Email, Avatar, State, Permission,
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
        /// <summary>
        /// 用户 ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 账户状态
        /// </summary>
        public State State { get; set; }

        /// <summary>
        /// 用户权限
        /// </summary>
        public Permission Permission { get; set; }

        /// <summary>
        /// 能否通过API访问
        /// </summary>
        public bool IsApiAvailable { get; set; }

        /// <summary>
        /// 最大上传数量（张）
        /// </summary>
        public long MaxUploadNums { get; set; }

        /// <summary>
        /// 最大存储容量（MB）
        /// </summary>
        public double MaxUploadCapacity { get; set; }

        /// <summary>
        /// 单次最大上传数量（张）
        /// </summary>
        public long SingleMaxUploadNums { get; set; }

        /// <summary>
        /// 单次最大上传容量（MB）
        /// </summary>
        public double SingleMaxUploadCapacity { get; set; }

        public UserDTOEntity(string id, string name, string email, string avatar, State state, Permission permission, bool isApiAvailable, long maxUploadNums, double maxUploadCapacity, long singleMaxUploadNums, double singleMaxUploadCapacity)
        {
            Id = id;
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
