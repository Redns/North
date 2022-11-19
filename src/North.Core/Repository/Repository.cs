using SqlSugar;
using System.Linq.Expressions;

namespace North.Core.Repository
{
    public class Repository<T> where T : class, new()
    {
        private readonly SqlSugarClient _client;

        public Repository(SqlSugarClient client)
        {
            _client = client;
        }

        public T First(Expression<Func<T, bool>> expression)
        {
            return _client.Queryable<T>().First(expression);
        }

        public async ValueTask<T> FirstAsync(Expression<Func<T, bool>> expression)
        {
            return await _client.Queryable<T>().FirstAsync(expression);
        }

        public T Single(Expression<Func<T, bool>> expression)
        {
            return _client.Queryable<T>().Single(expression);
        }

        public async ValueTask<T> SingleAsync(Expression<Func<T, bool>> expression)
        {
            return await _client.Queryable<T>().SingleAsync(expression);
        }

        public List<T> GetList(Expression<Func<T, bool>> expression)
        {
            return _client.Queryable<T>().Where(expression).ToList();
        }

        public async ValueTask<List<T>> GetListAsync(Expression<Func<T, bool>> expression)
        {
            return await _client.Queryable<T>().Where(expression).ToListAsync();
        }

        public bool Any(Expression<Func<T, bool>> expression)
        {
            return _client.Queryable<T>().Any(expression);
        }

        public async ValueTask<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _client.Queryable<T>().AnyAsync(expression);
        }

        public int Add(T entity)
        {
            return _client.Insertable(entity).ExecuteCommand();
        }

        public async ValueTask<int> AddAsync(T entity)
        {
            return await _client.Insertable(entity).ExecuteCommandAsync();
        }

        public int AddList(List<T> entities)
        {
            return _client.Insertable(entities).ExecuteCommand();
        }

        public async ValueTask<int> AddListAsync(List<T> entities)
        {
            return await _client.Insertable(entities).ExecuteCommandAsync();
        }

        public int Delete(T entity)
        {
            return _client.Deleteable(entity).ExecuteCommand();
        }

        public async ValueTask<int> DeleteAsync(T entity)
        {
            return await _client.Deleteable(entity).ExecuteCommandAsync();
        }

        public int Update(T entity)
        {
            return _client.Updateable(entity).ExecuteCommand();
        }

        public async ValueTask<int> UpdateAsync(T entity)
        {
            return await _client.Updateable(entity).ExecuteCommandAsync();
        }
    }
}
