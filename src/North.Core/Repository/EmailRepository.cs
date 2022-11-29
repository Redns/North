using North.Core.Entities;
using SqlSugar;

namespace North.Core.Repository
{
    public class EmailRepository : Repository<EmailEntity>
    {
        public EmailRepository(ISqlSugarClient client, string dbConfigId) : base(client, dbConfigId)
        {
        }
    }
}
