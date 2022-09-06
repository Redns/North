using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace North.Core.Entities
{
    /// <summary>
    /// 邮箱验证类
    /// </summary>
    public class EmailEntity
    {
        /// <summary>
        /// 验证链接为 verify?type={nameof(VerifyType)}&id={Id}
        /// </summary>
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 待验证的邮箱地址
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 链接到期时间
        /// </summary>
        [Required]
        public DateTime ExpireTime { get; set; } = DateTime.Now.AddDays(1);

        /// <summary>
        /// 验证类型
        /// </summary>
        [Required]
        public VerifyType VerifyType { get; set; }
    }


    /// <summary>
    /// 验证类型
    /// </summary>
    public enum VerifyType
    {
        Register = 0
    }


    public class SqlVerifyEmailData
    {
        private NorthDbContext _context;
        public SqlVerifyEmailData(NorthDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 获取验证邮件
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<EmailEntity> Get(Func<EmailEntity, bool>? predicate = null)
        {
            if (_context.Emails is not null)
            {
                if (predicate is null)
                {
                    return _context.Emails.AsNoTracking();
                }
                return _context.Emails.AsNoTracking().Where(predicate);
            }
            return Enumerable.Empty<EmailEntity>();
        }


        /// <summary>
        /// 获取验证邮件的异步版本
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async ValueTask<IEnumerable<EmailEntity>> GetAsync(Func<EmailEntity, bool>? predicate = null)
        {
            if (_context.Emails is not null)
            {
                if (predicate is not null)
                {
                    return _context.Emails.AsNoTracking().Where(predicate);
                }
                return await _context.Emails.AsNoTracking().ToArrayAsync();
            }
            return Enumerable.Empty<EmailEntity>();
        }


        /// <summary>
        /// 查找验证邮件
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public EmailEntity? Find(Func<EmailEntity, bool> predicate)
        {
            return _context.Emails?.AsNoTracking().FirstOrDefault(predicate);
        }


        /// <summary>
        /// 查找验证邮件的异步版本
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async ValueTask<EmailEntity?> FindAsync(Expression<Func<EmailEntity, bool>> predicate)
        {
            if (_context.Emails is not null)
            {
                return await _context.Emails.AsNoTracking().FirstOrDefaultAsync(predicate);
            }
            return null;
        }


        /// <summary>
        /// 新增验证邮件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool Add(EmailEntity email)
        {
            if (_context.Emails is not null)
            {
                _context.Emails.Add(email);
                return _context.SaveChanges() > 0;
            }
            return false;
        }


        /// <summary>
        /// 新增验证邮件的异步版本
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async ValueTask<bool> AddAsync(EmailEntity email)
        {
            if (_context.Emails is not null)
            {
                await _context.Emails.AddAsync(email);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }


        /// <summary>
        /// 添加多个验证邮件
        /// </summary>
        /// <param name="emails"></param>
        /// <returns></returns>
        public bool AddRange(IEnumerable<EmailEntity> emails)
        {
            if (_context.Emails is not null)
            {
                _context.Emails.AddRange(emails);
                return _context.SaveChanges() > 0;
            }
            return false;
        }


        /// <summary>
        /// 添加多个验证邮件的异步版本
        /// </summary>
        /// <param name="emails"></param>
        /// <returns></returns>
        public async ValueTask<bool> AddRangeAsync(IEnumerable<EmailEntity> emails)
        {
            if (_context.Emails is not null)
            {
                await _context.Emails.AddRangeAsync(emails);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }


        /// <summary>
        /// 删除验证邮件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool Remove(EmailEntity email)
        {
            if (_context.Emails is not null)
            {
                _context.Emails.Remove(email);
                return _context.SaveChanges() > 0;
            }
            return false;
        }


        /// <summary>
        /// 删除验证邮件的异步版本
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async ValueTask<bool> RemoveAsync(EmailEntity email)
        {
            if (_context.Emails is not null)
            {
                _context.Emails.Remove(email);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }


        /// <summary>
        /// 移除多个验证邮件
        /// </summary>
        /// <param name="emails"></param>
        /// <returns></returns>
        public bool RemoveRange(IEnumerable<EmailEntity> emails)
        {
            if (_context.Emails is not null)
            {
                _context.Emails.RemoveRange(emails);
                return _context.SaveChanges() > 0;
            }
            return true;
        }


        /// <summary>
        /// 移除多个用户的异步版本
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public async ValueTask<bool> RemoveRangeAsync(IEnumerable<EmailEntity> emails)
        {
            if (_context.Emails is not null)
            {
                _context.Emails.RemoveRange(emails);
                return await _context.SaveChangesAsync() > 0;
            }
            return true;
        }
    }
}
