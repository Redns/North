using North.Core.Entities;
using SqlSugar;

namespace North.Core.Repository
{
    public class ImageRepository : Repository<ImageEntity>
    {
        public ImageRepository(ISqlSugarClient client, string dbConfigId) : base(client, dbConfigId)
        {
        }
    }
}
