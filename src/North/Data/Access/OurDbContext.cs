using Microsoft.EntityFrameworkCore;
using North.Data.Entities;

namespace North.Data.Access
{
    public class OurDbContext : DbContext, IDisposable
    {
        public DbSet<UserEntity>? Users { get; set; }

        public OurDbContext() { }
        public OurDbContext(DbContextOptions<OurDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder opt)
        {
            opt.UseSqlite("Data Source=Data/Databases/North.db;");
        }
    }
}
