using ImageBed.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ImageBed.Data.Access
{
    public class SqlUserData
    {
        private OurDbContext _context { get; set; }
        public SqlUserData(OurDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 获取符合条件的全部用户
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetAll(Func<UserEntity, bool> predicate)
        {
            if ((_context != null) && (_context.Users != null))
            {
                return _context.Users.Where(predicate);
            }
            return Array.Empty<UserEntity>();
        }


        /// <summary>
        /// 获取符合条件的第一个用户
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<UserEntity?> GetFirstAsync(Expression<Func<UserEntity, bool>> predicate)
        {
            if ((_context != null) && (_context.Users != null))
            {
                return await _context?.Users?.FirstOrDefaultAsync(predicate);
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
            if ((_context != null) && (_context.Users != null))
            {
                _context.Users.Add(user);
                if (_context.SaveChanges() > 0)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(UserEntity user)
        {
            if ((_context != null) && (_context.Users != null))
            {
                await _context.Users.AddAsync(user);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Update(UserEntity user)
        {
            if ((_context != null) && (_context.Users != null))
            {
                _context.Users.Update(user);
                if (_context.SaveChanges() > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
