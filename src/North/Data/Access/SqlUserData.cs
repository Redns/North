using Microsoft.EntityFrameworkCore;
using North.Data.Entities;
using System.Linq.Expressions;

namespace North.Data.Access
{
    public class SqlUserData
    {
        private OurDbContext _context;
        public SqlUserData(OurDbContext context)
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
            if(_context.Users is not null)
            {
                if(predicate is null)
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
            if(_context.Users is not null)
            {
                if(predicate is not null)
                {
                    return _context.Users.AsNoTracking().Where(predicate);
                }
                return await _context.Users.AsNoTracking().ToArrayAsync();
            }
            return Enumerable.Empty<UserEntity>();
        }


        /// <summary>
        /// 查找用户
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public UserEntity? Find(Func<UserEntity, bool> predicate)
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
            if(_context.Users is not null)
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
            if(_context.Users is not null)
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
            if(_context.Users is not null)
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
            if(_context.Users is not null)
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
            if(_context.Users is not null)
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
            if(_context.Users is not null)
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
            if(_context.Users is not null)
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
            if(_context.Users is not null)
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
