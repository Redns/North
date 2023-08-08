using Microsoft.AspNetCore.Authentication.Cookies;
using SqlSugar;
using System.Security.Claims;
using System.Text.Json;

namespace North.Core.Entities
{
    /// <summary>
    /// 用户实体
    /// </summary>
    [SugarTable("Users")]
    public class UserEntity : Entity
    {
        #region 账户信息（用户名、注册邮箱、密码、头像、注册时间）
        /// <summary>
        /// 用户名
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 注册邮箱
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 加密后的密码
        /// </summary>
        [SugarColumn(Length = 32)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 头像
        /// </summary>
        [SugarColumn(Length = 256)]
        public string Avatar { get; set; } = string.Empty;

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegisterTime { get; set; } = DateTime.Now;
        #endregion

        #region 权限信息（账户状态、角色、API访问控制、令牌）
        /// <summary>
        /// 账户状态
        /// </summary>
        public UserState State { get; set; } = UserState.Checking;

        /// <summary>
        /// 用户权限
        /// </summary>
        public UserPermission Permission { get; set; } = UserPermission.User;

        /// <summary>
        /// 用户角色
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string Role => Permission switch
        {
            UserPermission.User => "User",
            UserPermission.System => "System",
            _ => "UnKnown"
        };

        /// <summary>
        /// 能否通过API访问
        /// </summary>
        public bool IsApiAvailable { get; set; } = true;

        /// <summary>
        /// 用户 Claims 信息最后一次的修改时间，修改后用户需要同步信息/重新登录
        /// </summary>
        public DateTime LastModifyTime { get; set; }

        /// <summary>
        /// 用户组 ID
        /// </summary>
        public Guid UserGroupId { get; set; }
        #endregion

        #region 图片上传限制
        /// <summary>
        /// 最大上传数量（张）
        /// </summary>
        public int MaxUploadNums { get; set; } = 0;

        /// <summary>
        /// 最大存储容量（MB）
        /// </summary>
        public double MaxUploadCapacity { get; set; } = 0;

        /// <summary>
        /// 最大存储容量（Byte）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public double MaxUploadCapacityByte => MaxUploadCapacity * 1024 * 1024;

        /// <summary>
        /// 总上传数量（张）
        /// </summary>
        public int TotalUploadedNums { get; set; } = 0;

        /// <summary>
        /// 总上传容量
        /// </summary>
        public double TotalUploadedCapacity { get; set; } = 0;

        /// <summary>
        /// 总上传容量（Byte）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public double TotalUploadedCapacityByte => TotalUploadedCapacity * 1024 * 1024;

        /// <summary>
        /// 单次最大上传数量（张）
        /// </summary>
        public int SingleMaxUploadNums { get; set; } = 0;

        /// <summary>
        /// 单张图片最大尺寸（MB）
        /// </summary>
        public double SingleMaxUploadSize { get; set; } = 0;

        /// <summary>
        /// 单张图片最大尺寸（Byte）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public double SingleMaxUploadSizeByte => SingleMaxUploadSize * 1024 * 1024;

        /// <summary>
        /// 图片剩余上传数量（张）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int UplaodRemainedNums => MaxUploadNums - TotalUploadedNums;

        /// <summary>
        /// 图片剩余上传容量（MB）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public double UploadRemainedCapacity => MaxUploadCapacity - TotalUploadedCapacity;

        /// <summary>
        /// 图片剩余上传容量（Byte）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public double UploadRemainedCapacityByte => UploadRemainedCapacity * 1024 * 1024;
        #endregion

        #region 导航属性
        /// <summary>
        /// 用户图片
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(ImageEntity.UserId))]
        public List<ImageEntity> Images { get; set; }

        /// <summary>
        /// 登录历史
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(UserLoginHistoryEntity.UserId))]
        public List<UserLoginHistoryEntity> LoginHistories { get; set; }
        #endregion


        /// <summary>
        /// 用户 DTO 对象
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public UserDTOEntity DTO => new()
        {
            Name = Name,
            Email = Email,
            Avatar = Avatar,
            RegisterTime = RegisterTime,
            State = State,
            Permission = Permission,
            IsApiAvailable = IsApiAvailable,
            MaxUploadNums = MaxUploadNums,
            MaxUploadCapacity = MaxUploadCapacity,
            SingleMaxUploadNums = SingleMaxUploadNums,
            SingleMaxUploadSize = SingleMaxUploadSize
        };


        /// <summary>
        /// 用户 Claims 认证对象
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public ClaimsIdentity ClaimsIdentify => new(new Claim[]
        {
            new Claim(ClaimTypes.Role, Role),
            new Claim(ClaimTypes.SerialNumber, Id.ToString()),
            new Claim("LastModifyTime", LastModifyTime.ToString("G"))
        }, CookieAuthenticationDefaults.AuthenticationScheme);


        /// <summary>
        /// 序列化用户对象
        /// </summary>
        /// <returns></returns>
        public override string ToString() => JsonSerializer.Serialize(this, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
    }


    /// <summary>
    /// 用户 DTO 实体
    /// </summary>
    public class UserDTOEntity : Entity
    {
        #region 账户信息（用户名、注册邮箱、头像、注册时间）
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// 注册邮箱
        /// </summary>
        public string Email { get; init; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; init; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegisterTime { get; init; }
        #endregion

        #region 权限信息（账户状态、角色、API访问控制）
        /// <summary>
        /// 账户状态
        /// </summary>
        public UserState State { get; init; }

        /// <summary>
        /// 用户权限
        /// </summary>
        public UserPermission Permission { get; init; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public string Role => Permission switch
        {
            UserPermission.User => "User",
            UserPermission.System => "System",
            _ => "UnKnown"
        };

        /// <summary>
        /// 能否通过API访问
        /// </summary>
        public bool IsApiAvailable { get; set; } = true;
        #endregion

        #region 图片上传限制
        /// <summary>
        /// 最大上传数量（张）
        /// </summary>
        public int MaxUploadNums { get; init; }

        /// <summary>
        /// 最大存储容量（MB）
        /// </summary>
        public double MaxUploadCapacity { get; init; }

        /// <summary>
        /// 最大存储容量（Byte）
        /// </summary>
        public double MaxUploadCapacityByte => MaxUploadCapacity * 1024 * 1024;

        /// <summary>
        /// 总上传数量（张）
        /// </summary>
        public int TotalUploadedNums { get; init; }

        /// <summary>
        /// 总上传容量（MB）
        /// </summary>
        public double TotalUploadedCapacity { get; init; }

        /// <summary>
        /// 总上传容量（Byte）
        /// </summary>
        public double TotalUploadedCapacityByte => TotalUploadedCapacity * 1024 * 1024;

        /// <summary>
        /// 单次最大上传数量（张）
        /// </summary>
        public int SingleMaxUploadNums { get; init; }

        /// <summary>
        /// 单张图片最大尺寸（MB）
        /// </summary>
        public double SingleMaxUploadSize { get; init; }

        /// <summary>
        /// 单张图片最大尺寸（Byte）
        /// </summary>
        public double SingleMaxUploadSizeByte => SingleMaxUploadSize * 1024 * 1024;

        /// <summary>
        /// 图片剩余上传数量（张）
        /// </summary>
        public int UplaodRemainedNums => MaxUploadNums - TotalUploadedNums;

        /// <summary>
        /// 图片剩余上传容量（MB）
        /// </summary>
        public double UploadRemainedCapacity => MaxUploadCapacity - TotalUploadedCapacity;

        /// <summary>
        /// 图片剩余上传容量（Byte）
        /// </summary>
        public double UploadRemainedCapacityByte => UploadRemainedCapacity * 1024 * 1024;
        #endregion
    }


    /// <summary>
    /// 用户权限
    /// </summary>
    public enum UserPermission
    {
        User = 0,           // 用户
        System              // 系统
    }


    /// <summary>
    /// 账户状态
    /// </summary>
    public enum UserState
    {
        Checking = 0,       // 待验证
        Normal,             // 正常
        Forbidden           // 封禁中
    }
}
