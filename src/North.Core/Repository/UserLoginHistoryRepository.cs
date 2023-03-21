using North.Core.Entities;
using SqlSugar;

namespace North.Core.Repository
{
    public class UserLoginHistoryRepository : Repository<UserLoginHistoryEntity>
    {
        public UserLoginHistoryRepository(ISqlSugarClient client, string dbConfigId) : base(client, dbConfigId)
        {
        }
    }
}
