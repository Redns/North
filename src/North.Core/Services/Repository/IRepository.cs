using Microsoft.EntityFrameworkCore;
using North.Core.Entities;
using System.Linq.Expressions;

namespace North.Core.Services.Repository
{
    public interface IRepository<TEntity, TContext> where TEntity : Entity where TContext : DbContext
    {
        #region Add
        bool Add(TEntity entity);
        ValueTask<bool> AddAsync(TEntity entity);
        bool AddRange(IEnumerable<TEntity> entities);
        ValueTask<bool> AddRangeAsync(IEnumerable<TEntity> entities);
        #endregion

        #region Remove
        bool Remove(TEntity entity);
        ValueTask<bool> RemoveAsync(TEntity entity);
        bool RemoveRange(IEnumerable<TEntity> entities);
        ValueTask<bool> RemoveRangeAsync(IEnumerable<TEntity> entities);
        #endregion

        #region Get
        bool Any(Expression<Func<UserEntity, bool>>? predicate = null);
        ValueTask<bool> AnyAsync(Expression<Func<UserEntity, bool>>? predicate = null);
        TEntity? Get(Func<TEntity, bool>? predicate = null);
        ValueTask<TEntity?> GetAsync(Func<TEntity, bool>? predicate = null);
        IQueryable<TEntity> GetAll(Func<TEntity, bool>? predicate = null);
        ValueTask<IQueryable<TEntity>> GetAllAsync(Func<TEntity, bool>? predicate = null);
        #endregion

        #region Update
        bool Update(TEntity entity);
        ValueTask<bool> UpdateAsync(TEntity entity);
        bool UpdateRange(IEnumerable<TEntity> entities);
        ValueTask<bool> UpdateRangeAsync(IEnumerable<TEntity> entities);
        #endregion
    }


    public class Entity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
