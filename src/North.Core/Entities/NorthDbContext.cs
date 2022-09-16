using Microsoft.EntityFrameworkCore;

namespace North.Core.Entities
{
    /// <summary>
    /// North 数据库上下文
    /// </summary>
    public class NorthDbContext : DbContext, IDisposable
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<ImageEntity> Images { get; set; }
        public DbSet<EmailEntity> Emails { get; set; }
        public DbSet<PluginEntity> Plugins { get; set; }
        public DbSet<PluginModuleEntity> PluginsModules { get; set; }

        // TODO 避免使用常量值建立查询
        public NorthDbContext(DbContextOptions options) : base(options) 
        {
            if (Database.IsSqlite())
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Database.GetConnectionString()?.Split('=').Last()) ?? string.Empty);
            }
            Database.EnsureCreated();
        }


        /// <summary>
        /// 迁移数据库
        /// </summary>
        /// <param name="context">新数据库的上下文</param>
        /// <returns>迁移完成返回 true，否则返回 false</returns>
        public async ValueTask<bool> MigrateDatabaseAsync(NorthDbContext context)
        {
            await context.Users.AddRangeAsync(Users);
            await context.Images.AddRangeAsync(Images);
            await context.Emails.AddRangeAsync(Emails);
            return await context.SaveChangesAsync() > 0;
        }
    }
}