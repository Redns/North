using North.Core.Entities;
using SqlSugar;

namespace North.Core.Repository
{
    public class ImageRepository : Repository<ImageEntity>
    {
        public ImageRepository(SqlSugarClient client) : base(client)
        {
        }
    }
}
