using ImageBed.Common;
using ImageBed.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace ImageBed.Data.Access
{
    public class OurDbContext : DbContext
    {
        public OurDbContext() { }
        public OurDbContext(DbContextOptions<OurDbContext> options) : base(options) { }

        public DbSet<ImageEntity> Images { get; set; }

        /// <summary>
        /// 连接本地SQLite数据库
        /// </summary>
        /// <param name="opt"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder opt)
        {
            AppSetting? appSetting = AppSetting.Parse(File.ReadAllText("appsettings.json"));
            if ((appSetting != null) && (appSetting.Data != null) && (appSetting.Data.Resources != null) && (appSetting.Data.Resources.Database != null))
            {
                if (string.IsNullOrEmpty(appSetting.Data.Resources.Database.ConnStr))
                {
                    appSetting.Data.Resources.Database.ConnStr = SQLiteHelper.CreateSQLiteDatabase("Data/Database");

                    // 保存设置
                    AppSetting.Save(appSetting, "appsettings.json");
                }
                opt.UseSqlite(appSetting.Data.Resources.Database.ConnStr);
            }
        }
    }


    public class SQLImageData : IDisposable
    {
        public OurDbContext? _context { get; set; }
        public SQLImageData(OurDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取数据库中的所有图片信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<ImageEntity>> Get()
        {
            if((_context != null) && (_context.Images != null))
            {
                return await _context.Images.ToListAsync();
            }
            return new List<ImageEntity>();
        }

        /// <summary>
        /// 获取数据库中指定ID的图片信息
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <returns></returns>
        public async Task<ImageEntity?> Get(string id)
        {
            if ((_context != null) && (_context.Images != null))
            {
                return await _context.Images.FirstAsync(x => x.Id == id);
            }
            return null;
        }

        /// <summary>
        /// 移除数据库中指定图片的信息
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <returns></returns>
        public async Task<bool> Remove(string id)
        {
            if ((_context != null) && (_context.Images != null))
            {
                try
                {
                    ImageEntity? image = await Get(id);
                    if(image != null)
                    {
                        _context.Images.Remove(image);
                        _context.SaveChanges();
                    }
                    return true;
                }
                catch (Exception)
                {

                }
            }
            return false;
        }

        public void Dispose()
        {
            try
            {
                if(_context != null)
                {
                    _context?.Dispose();
                }
                GC.SuppressFinalize(this);
            }
            catch (Exception) { }
        }
    }
}
