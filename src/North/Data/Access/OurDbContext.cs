using Microsoft.EntityFrameworkCore;
using North.Common;
using North.Core.Entities;

namespace North.Data.Access
{
    public class OurDbContext : DbContext, IDisposable
    {
        public DbSet<UserEntity>? Users { get; set; }
        public DbSet<VerifyEmailEntity>? VerifyEmails { get; set; }

        public OurDbContext() { }
        public OurDbContext(DbContextOptions<OurDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder opt)
        {
            opt.UseSqlite(GlobalValues.AppSettings.General.DataBase.ConnStr);
        }
    }
}
