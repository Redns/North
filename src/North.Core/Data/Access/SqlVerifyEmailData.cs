using Microsoft.EntityFrameworkCore;
using North.Core.Data.Entities;
using System.Linq.Expressions;

namespace North.Core.Data.Access
{
    public class SqlVerifyEmailData
    {
        private OurDbContext _context;
        public SqlVerifyEmailData(OurDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 获取验证邮件
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<VerifyEmailEntity> Get(Func<VerifyEmailEntity, bool>? predicate = null)
        {
            if(_context.VerifyEmails is not null)
            {
                if(predicate is null)
                {
                    return _context.VerifyEmails.AsNoTracking();
                }
                return _context.VerifyEmails.AsNoTracking().Where(predicate);
            }
            return Enumerable.Empty<VerifyEmailEntity>();
        }


        /// <summary>
        /// 获取验证邮件的异步版本
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async ValueTask<IEnumerable<VerifyEmailEntity>> GetAsync(Func<VerifyEmailEntity, bool>? predicate = null)
        {
            if(_context.VerifyEmails is not null)
            {
                if(predicate is not null)
                {
                    return _context.VerifyEmails.AsNoTracking().Where(predicate);
                }
                return await _context.VerifyEmails.AsNoTracking().ToArrayAsync();
            }
            return Enumerable.Empty<VerifyEmailEntity>();
        }


        /// <summary>
        /// 查找验证邮件
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public VerifyEmailEntity? Find(Func<VerifyEmailEntity, bool> predicate)
        {
            return _context.VerifyEmails?.AsNoTracking().FirstOrDefault(predicate);
        }


        /// <summary>
        /// 查找验证邮件的异步版本
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async ValueTask<VerifyEmailEntity?> FindAsync(Expression<Func<VerifyEmailEntity, bool>> predicate)
        {
            if(_context.VerifyEmails is not null)
            {
                return await _context.VerifyEmails.AsNoTracking().FirstOrDefaultAsync(predicate);
            }
            return null;
        }


        /// <summary>
        /// 新增验证邮件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool Add(VerifyEmailEntity email)
        {
            if(_context.VerifyEmails is not null)
            {
                _context.VerifyEmails.Add(email);
                return _context.SaveChanges() > 0;
            }
            return false;
        }


        /// <summary>
        /// 新增验证邮件的异步版本
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async ValueTask<bool> AddAsync(VerifyEmailEntity email)
        {
            if(_context.VerifyEmails is not null)
            {
                await _context.VerifyEmails.AddAsync(email);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }


        /// <summary>
        /// 删除验证邮件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool Remove(VerifyEmailEntity email)
        {
            if(_context.VerifyEmails is not null)
            {
                _context.VerifyEmails.Remove(email);
                return _context.SaveChanges() > 0;
            }
            return false;
        }


        /// <summary>
        /// 删除验证邮件的异步版本
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async ValueTask<bool> RemoveAsync(VerifyEmailEntity email)
        {
            if (_context.VerifyEmails is not null)
            {
                _context.VerifyEmails.Remove(email);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}
