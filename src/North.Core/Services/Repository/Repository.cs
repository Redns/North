using Microsoft.EntityFrameworkCore;
using North.Core.Entities;
using System.Linq;
using System.Linq.Expressions;

// TODO 增加邮件生成工厂
namespace North.Core.Services.Repository
{
    //public class Repository<TEntity> : IRepository<TEntity, NorthDbContext> where TEntity : Entity
    //{
    //    private DbSet<TEntity> _dbSet;
    //    private NorthDbContext _context;

    //    public Repository(NorthDbContext context)
    //    {
    //        _context = context;
    //        _dbSet = context.Set<TEntity>();
    //    }

    //    #region Add
    //    public bool Add(TEntity entity)
    //    {
    //        _dbSet.Add(entity);
    //        return _context.SaveChanges() > 0;
    //    }

    //    public async ValueTask<bool> AddAsync(TEntity entity)
    //    {
    //        await _dbSet.AddAsync(entity);
    //        return await _context.SaveChangesAsync() > 0;
    //    }

    //    public bool AddRange(IEnumerable<TEntity> entities)
    //    {
    //        _dbSet.AddRange(entities);
    //        return _context.SaveChanges() > 0;
    //    }

    //    public async ValueTask<bool> AddRangeAsync(IEnumerable<TEntity> entities)
    //    {
    //        await _dbSet.AddRangeAsync(entities);
    //        return await _context.SaveChangesAsync() > 0;
    //    }
    //    #endregion

    //    public bool Any(Expression<Func<TEntity, bool>> predicate)
    //    {
    //        return _dbSet.AsNoTracking().Any(predicate);
    //    }

    //    public async ValueTask<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    //    {
    //        return await _dbSet.AsNoTracking().AnyAsync(predicate);
    //    }

    //    public TEntity? Get(Func<TEntity, bool>? predicate = null)
    //    {
    //        return _dbSet.First(predicate ?? (u => true));
    //    }

    //    public IQueryable<TEntity> GetAll(Func<TEntity, bool>? predicate = null)
    //    {
    //        return predicate is not null ? _dbSet.Where(predicate).AsQueryable() : _dbSet.AsQueryable();
    //    }

    //    public ValueTask<IQueryable<TEntity>> GetAllAsync(Func<TEntity, bool>? predicate = null)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ValueTask<TEntity?> GetAsync(Func<TEntity, bool>? predicate = null)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool Remove(TEntity entity)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ValueTask<bool> RemoveAsync(TEntity entity)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool RemoveRange(IEnumerable<TEntity> entities)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ValueTask<bool> RemoveRangeAsync(IEnumerable<TEntity> entities)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool Update(TEntity entity)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ValueTask<bool> UpdateAsync(TEntity entity)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool UpdateRange(IEnumerable<TEntity> entities)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ValueTask<bool> UpdateRangeAsync(IEnumerable<TEntity> entities)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}