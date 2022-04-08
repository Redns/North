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
        public DbSet<RecordEntity> Records { get; set; }


        /// <summary>
        /// 连接本地SQLite数据库
        /// </summary>
        /// <param name="opt"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder opt)
        {
            if (string.IsNullOrEmpty(GlobalValues.appSetting?.Data?.Resources?.Database?.Path))
            {
                GlobalValues.appSetting.Data.Resources.Database.Path = SQLiteHelper.CreateSQLiteDatabase("Data/Database/imagebed.sqlite");
                AppSetting.Save(GlobalValues.appSetting, "appsettings.json");
            }
            opt.UseSqlite(GlobalValues.appSetting.Data.Resources.Database.Path);
        }
    }


    /// <summary>
    /// 图片数据处理
    /// </summary>
    public class SQLImageData : IDisposable
    {
        public OurDbContext? _context { get; set; }
        public SQLImageData(OurDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 添加图片信息至数据库
        /// </summary>
        /// <param name="image">待添加的图片</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public async Task<bool> Add(ImageEntity image)
        {
            if((_context != null) && (_context.Images != null) && (image != null))
            {
                await _context.Images.AddAsync(image);
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        public async Task<bool> Update(ImageEntity image)
        {
            if ((_context != null) && (_context.Images != null))
            {
                try
                {
                    _context.Images.Update(image);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception) { }
            }
            return false;
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
        /// 根据图片名称查找
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ImageEntity?> GetByName(string name)
        {
            if ((_context != null) && (_context.Images != null))
            {
                try
                {
                    return await _context.Images.FirstAsync(x => x.Name == name);
                }
                catch { }
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
                        string imagePath = $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path}/{image.Name}";
                        if (File.Exists(imagePath))
                        {
                            File.Delete(imagePath);
                        }
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


    public class SQLRecordData : IDisposable
    {
        private OurDbContext _context { get; set; }
        public SQLRecordData(OurDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 获取所有记录数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<RecordEntity>> Get()
        {
            if((_context != null) && (_context.Records != null))
            {
                return await _context.Records.ToListAsync();
            }
            return new List<RecordEntity>();
        }


        /// <summary>
        /// 更新记录信息
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task<bool> Update(RecordEntity newRecord)
        {
            if((_context != null) && (_context.Records != null))
            {
                try
                {
                    RecordEntity? oldRecord = _context.Records.FirstOrDefault(r => r.Date == newRecord.Date);
                    if(oldRecord != null)
                    {
                        _context.Records.Update(newRecord);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                }
                catch (Exception) { }
            }
            return false;
        }


        public void Dispose()
        {
            if(_context != null)
            {
                _context.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
