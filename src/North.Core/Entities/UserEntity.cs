using System.Security.Claims;
using System.Text.Json;

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

        public UserEntity(string id, string name, string email, string password, string avatar, State state, Permission permission, bool isApiAvailable, long maxUploadNums, double maxUploadCapacity, long singleMaxUploadNums, double singleMaxUploadCapacity)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Avatar = avatar;
            State = state;
            Permission = permission;
            IsApiAvailable = isApiAvailable;
            MaxUploadNums = maxUploadNums;
            MaxUploadCapacity = maxUploadCapacity;
            SingleMaxUploadNums = singleMaxUploadNums;
            SingleMaxUploadCapacity = singleMaxUploadCapacity;
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


        public ClaimsIdentity ToClaimsIdentify()
        {
            return new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.SerialNumber, Id),
                new Claim(ClaimTypes.Name, Name),
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.Role, Permission.ToString()),
                new Claim(ClaimTypes.Actor, Avatar)
            });
        }


        public override string ToString() => JsonSerializer.Serialize(this, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
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


    /// <summary>
    /// 用户 DTO 实体
    /// </summary>
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


    /// <summary>
    /// 用户 Claim 实体
    /// </summary>
    public class UserClaimEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }

        public UserClaimEntity(string id, string name, string email, string role, string avatar)
        {
            Id = id;
            Name = name;
            Email = email;
            Role = role;
            Avatar = avatar;
        }
    }
}
