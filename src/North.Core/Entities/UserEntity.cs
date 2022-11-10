using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;

namespace North.Core.Entities
{
    /// <summary>
    /// 用户实体
    /// </summary>
    public class UserEntity : Entity
    {
        #region 账户信息
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(32)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 加密后的密码
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 头像
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Avatar { get; set; } = string.Empty;

        /// <summary>
        /// 注册时间
        /// </summary>
        [Required]
        public DateTime RegisterTime { get; set; } = DateTime.Now;
        #endregion

        #region 权限信息
        /// <summary>
        /// 账户状态
        /// </summary>
        [Required]
        public UserState State { get; set; } = UserState.Checking;

        /// <summary>
        /// 用户权限
        /// </summary>
        [Required]
        public UserPermission Permission { get; set; } = UserPermission.User;

        /// <summary>
        /// 能否通过API访问
        /// </summary>
        [Required]
        public bool IsApiAvailable { get; set; } = true;

        /// <summary>
        /// 用户令牌
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string Token { get; set; } = string.Empty;
        #endregion

        #region 图片上传限制
        /// <summary>
        /// 最大上传数量（张）
        /// </summary>
        public long MaxUploadNums { get; set; } = 0;

        /// <summary>
        /// 最大存储容量（MB）
        /// </summary>
        public double MaxUploadCapacity { get; set; } = 0;

        /// <summary>
        /// 单次最大上传数量（张）
        /// </summary>
        public long SingleMaxUploadNums { get; set; } = 0L;

        /// <summary>
        /// 单次最大上传容量（MB）
        /// </summary>
        public double SingleMaxUploadCapacity { get; set; } = 0;
        #endregion

        #region 导航属性
        /// <summary>
        /// 用户存储的图片
        /// </summary>
        public ICollection<ImageEntity> Images { get; set; }
        #endregion

        #region 构造函数
        public UserEntity() : base(new Guid()) { }
        public UserEntity(Guid id) : base(id) { }
        #endregion


        /// <summary>
        /// 是否持有有效令牌
        /// 用户信息更改或账户状态变化均会导致之前生成的令牌清空（失效）
        /// </summary>
        /// <returns></returns>
        public bool HasValidToken => !string.IsNullOrWhiteSpace(Token) && State is UserState.Normal;


        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <param name="tokens">已生成的所有令牌</param>
        /// <returns></returns>
        public string GenerateToken()
        {
            return Token = Guid.NewGuid().ToString().ToUpper().Replace("-", string.Empty);
        }


        #region 相关对象生成
        /// <summary>
        /// 生成 DTO 对象
        /// </summary>
        /// <returns></returns>
        public UserDTOEntity DTO => new(Id, 
                                        Name, 
                                        Email, 
                                        Avatar, 
                                        State, 
                                        Permission,
                                        IsApiAvailable, 
                                        MaxUploadNums, 
                                        MaxUploadCapacity,
                                        SingleMaxUploadNums, 
                                        SingleMaxUploadCapacity, 
                                        RegisterTime);


        /// <summary>
        /// 生成 Claims 认证对象
        /// </summary>
        /// <returns></returns>
        public ClaimsIdentity ClaimsIdentify => new(new Claim[]
        {
            new Claim("Token", Token),
            new Claim(ClaimTypes.Email, Email),
            new Claim(ClaimTypes.Role, Permission.ToString("G"))
        }, CookieAuthenticationDefaults.AuthenticationScheme);
        #endregion


        public override string ToString() => JsonSerializer.Serialize(this, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
    }


    /// <summary>
    /// 用户 DTO 实体
    /// </summary>
    public class UserDTOEntity
    {
        public Guid Id { get; set; }

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
        public UserState State { get; set; }

        /// <summary>
        /// 用户权限
        /// </summary>
        public UserPermission Permission { get; set; }

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
        public DateTime RegisterTime { get; set; }

        public UserDTOEntity(Guid id, string name, string email, string avatar, UserState state, UserPermission permission, bool isApiAvailable, long maxUploadNums, double maxUploadCapacity, long singleMaxUploadNums, double singleMaxUploadCapacity, DateTime registerTime)
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
            RegisterTime = registerTime;
        }
    }


    /// <summary>
    /// 用户权限
    /// </summary>
    public enum UserPermission
    {
        User = 0,           // 用户
        Administrator,      // 管理员
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


    /// <summary>
    /// 用户查询辅助类
    /// </summary>
    public class SqlUserData
    {
        private NorthDbContext _context;
        public SqlUserData(NorthDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<UserEntity> Get(Func<UserEntity, bool>? predicate = null)
        {
            if (_context.Users is not null)
            {
                if (predicate is null)
                {
                    return _context.Users.AsNoTracking();
                }
                return _context.Users.AsNoTracking().Where(predicate);
            }
            return Enumerable.Empty<UserEntity>();
        }


        /// <summary>
        /// 获取用户的异步版本
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async ValueTask<IEnumerable<UserEntity>> GetAsync(Func<UserEntity, bool>? predicate = null)
        {
            if (_context.Users is not null)
            {
                if (predicate is not null)
                {
                    return _context.Users.AsNoTracking().Where(predicate);
                }
                return await _context.Users.AsNoTracking().ToArrayAsync();
            }
            return Enumerable.Empty<UserEntity>();
        }


        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Any(Expression<Func<UserEntity, bool>> predicate)
        {
            return _context.Users?.Any(predicate) ?? false;
        }


        /// <summary>
        /// 查找用户
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public UserEntity? Find(Expression<Func<UserEntity, bool>> predicate)
        {
            return _context.Users?.AsNoTracking().FirstOrDefault(predicate);
        }


        /// <summary>
        /// 查找用户的异步版本
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async ValueTask<UserEntity?> FindAsync(Expression<Func<UserEntity, bool>> predicate)
        {
            if (_context.Users is not null)
            {
                return await _context.Users.AsNoTracking().FirstOrDefaultAsync(predicate);
            }
            return null;
        }


        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Add(UserEntity user)
        {
            if (_context.Users is not null)
            {
                _context.Users.Add(user);
                return _context.SaveChanges() > 0;
            }
            return false;
        }


        /// <summary>
        /// 添加用户的异步版本
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async ValueTask<bool> AddAsync(UserEntity user)
        {
            if (_context.Users is not null)
            {
                await _context.Users.AddAsync(user);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }


        /// <summary>
        /// 添加多个用户
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public bool AddRange(IEnumerable<UserEntity> users)
        {
            if (_context.Users is not null)
            {
                _context.Users.AddRange(users);
                return _context.SaveChanges() > 0;
            }
            return false;
        }


        /// <summary>
        /// 添加多个用户的异步版本
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public async ValueTask<bool> AddRangeAsync(IEnumerable<UserEntity> users)
        {
            if (_context.Users is not null)
            {
                await _context.Users.AddRangeAsync(users);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }


        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Remove(UserEntity user)
        {
            if (_context.Users is not null)
            {
                _context.Users.Remove(user);
                return _context.SaveChanges() > 0;
            }
            return true;
        }


        /// <summary>
        /// 删除用户的异步版本
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async ValueTask<bool> RemoveAsync(UserEntity user)
        {
            if (_context.Users is not null)
            {
                _context.Users.Remove(user);
                return await _context.SaveChangesAsync() > 0;
            }
            return true;
        }


        /// <summary>
        /// 移除多个用户
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public bool RemoveRange(IEnumerable<UserEntity> users)
        {
            if (_context.Users is not null)
            {
                _context.Users.RemoveRange(users);
                return _context.SaveChanges() > 0;
            }
            return true;
        }


        /// <summary>
        /// 移除多个用户的异步版本
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public async ValueTask<bool> RemoveRangeAsync(IEnumerable<UserEntity> users)
        {
            if (_context.Users is not null)
            {
                _context.Users.RemoveRange(users);
                return await _context.SaveChangesAsync() > 0;
            }
            return true;
        }


        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Update(UserEntity user)
        {
            if (_context.Users is not null)
            {
                _context.Users.Update(user);
                return _context.SaveChanges() > 0;
            }
            return false;
        }


        /// <summary>
        /// 更新用户的异步版本
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async ValueTask<bool> UpdateAsync(UserEntity user)
        {
            if (_context.Users is not null)
            {
                _context.Users.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}
