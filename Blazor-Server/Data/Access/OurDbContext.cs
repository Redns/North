using ImageBed.Common;
using ImageBed.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace ImageBed.Data.Access
{
    public class OurDbContext : DbContext
    {
        public OurDbContext() { }
        public OurDbContext(DbContextOptions<OurDbContext> options) : base(options) { }

        public DbSet<ImageEntity>? Images { get; set; }
        public DbSet<RecordEntity>? Records { get; set; }
        public DbSet<UserEntity>? Users { get; set; }


        /// <summary>
        /// 连接本地SQLite数据库
        /// </summary>
        /// <param name="opt"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder opt)
        {
            opt.UseSqlite(GlobalValues.CONNSTR_DATABASE);
        }
    }
}
