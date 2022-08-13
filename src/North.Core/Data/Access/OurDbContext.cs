using Microsoft.EntityFrameworkCore;
using North.Core.Data.Entities;

namespace North.Core.Data.Access
{
    public class OurDbContext : DbContext, IDisposable
    {
        private readonly string _connStr;
        public DbSet<UserEntity>? Users { get; set; }
        public DbSet<VerifyEmailEntity>? VerifyEmails { get; set; }

        public OurDbContext(string connStr) 
        { 
            _connStr = connStr;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder opt)
        {
            opt.UseSqlite(_connStr);
        }
    }
}
