using North.Core.Entities;
using SqlSugar;

namespace North.Core.Repository
{
    public class UserRepository : Repository<UserEntity>
    {
        public UserRepository(SqlSugarClient client) : base(client)
        {
        }
    }
}
