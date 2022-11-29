using SqlSugar;
using System.Linq.Expressions;

namespace North.Core.Repository
{
    public class Repository<T> where T : class, new()
    {
        private readonly SqlSugarProvider _db;

        public Repository(ISqlSugarClient client, dynamic dbConfigId)
        {
            _db = client.AsTenant().GetConnection(dbConfigId);

            // TODO 后期仅在新增数据库源时初始化数据库表
            _db.DbMaintenance.CreateDatabase();
            _db.CodeFirst.InitTables<T>();
        }


        /// <summary>
        /// 获取符合表达式的第一个实体
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public T? First(Expression<Func<T, bool>> expression)
        {
            return _db.Queryable<T>().First(expression);
        }


        /// <summary>
        /// 获取符合表达式的第一个实体
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async ValueTask<T?> FirstAsync(Expression<Func<T, bool>> expression)
        {
            return await _db.Queryable<T>().FirstAsync(expression);
        }

        public T? Single(Expression<Func<T, bool>> expression)
        {
            return _db.Queryable<T>().Single(expression);
        }

        public async ValueTask<T?> SingleAsync(Expression<Func<T, bool>> expression)
        {
            return await _db.Queryable<T>().SingleAsync(expression);
        }

        public List<T> GetList(Expression<Func<T, bool>> expression)
        {
            return _db.Queryable<T>().Where(expression).ToList();
        }

        public async ValueTask<List<T>> GetListAsync(Expression<Func<T, bool>> expression)
        {
            return await _db.Queryable<T>().Where(expression).ToListAsync();
        }

        public bool Any(Expression<Func<T, bool>> expression)
        {
            return _db.Queryable<T>().Any(expression);
        }

        public async ValueTask<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _db.Queryable<T>().AnyAsync(expression);
        }

        public int Add(T entity)
        {
            return _db.Insertable(entity).ExecuteCommand();
        }

        public async ValueTask<int> AddAsync(T entity)
        {
            return await _db.Insertable(entity).ExecuteCommandAsync();
        }

        public int AddList(List<T> entities)
        {
            return _db.Insertable(entities).ExecuteCommand();
        }

        public async ValueTask<int> AddListAsync(List<T> entities)
        {
            return await _db.Insertable(entities).ExecuteCommandAsync();
        }

        public int Delete(T entity)
        {
            return _db.Deleteable(entity).ExecuteCommand();
        }

        public async ValueTask<int> DeleteAsync(T entity)
        {
            return await _db.Deleteable(entity).ExecuteCommandAsync();
        }

        public int Update(T entity)
        {
            return _db.Updateable(entity).ExecuteCommand();
        }

        public async ValueTask<int> UpdateAsync(T entity)
        {
            return await _db.Updateable(entity).ExecuteCommandAsync();
        }
    }
}
