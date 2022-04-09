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
        public async Task<bool> AddAsync(ImageEntity image)
        {
            GlobalValues.Logger.Info("Putting image into database");
            if ((_context != null) && (_context.Images != null) && (image != null))
            {
                try
                {
                    await _context.Images.AddAsync(image);
                    _ = _context.SaveChangesAsync();
                    return true;
                }
                catch(Exception ex) 
                {
                    GlobalValues.Logger.Error($"Putting image failed, {ex.Message}");
                }
            }
            GlobalValues.Logger.Error("Putting image failed, context or dbset or param is null");
            return false;
        }


        /// <summary>
        /// 添加多张图片至数据库
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public async Task AddRangeAsync(IEnumerable<ImageEntity> images)
        {
            GlobalValues.Logger.Info("Putting images into database");
            if ((_context != null) && (_context.Images != null) && (images != null))
            {
                try
                {
                    await _context.Images.AddRangeAsync(images);
                    _ = _context.SaveChangesAsync();
                }
                catch(Exception ex) 
                {
                    GlobalValues.Logger.Error($"Putting images failed, {ex.Message}");
                }
            }
            GlobalValues.Logger.Info("Putting images finished");
        }


        /// <summary>
        /// 更新图片信息
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
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
                catch (Exception ex) 
                {
                    GlobalValues.Logger.Error($"Update image failed, {ex.Message}");
                }
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
                catch(Exception ex) 
                {
                    GlobalValues.Logger.Error($"Get image failed, {ex.Message}");
                }
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
                catch (Exception ex)
                {
                    GlobalValues.Logger.Error($"Remove image failed, {ex.Message}");
                }
            }
            return false;
        }


        /// <summary>
        /// 移除多个图片信息
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public async Task<bool> RemoveRangeAsync(List<ImageEntity> images)
        {
            if ((_context != null) && (_context.Images != null))
            {
                try
                {
                    images.ForEach(image =>
                    {
                        string imagePath = $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path}/{image.Name}";
                        if (File.Exists(imagePath))
                        {
                            File.Delete(imagePath);
                        }
                    });
                    _context.Images.RemoveRange(images);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    GlobalValues.Logger.Error($"Remove images failed, {ex.Message}");
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
            catch (Exception ex) 
            {
                GlobalValues.Logger.Error($"Dispose SqlImageData failed, {ex.Message}");
            }
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
        /// 获取指定日期的数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<RecordEntity?> Get(string date)
        {
            if ((_context != null) && (_context.Records != null))
            {
                return await _context.Records.FirstAsync(r => r.Date == date);
            }
            return null;
        }


        public RecordEntity? GetByDate(string date)
        {
            if ((_context != null) && (_context.Records != null))
            {
                try
                {
                    return _context.Records.First(r => r.Date == date);
                }
                catch (Exception ex) 
                {
                    GlobalValues.Logger.Error($"Get record of {date} failed, {ex.Message}");
                }
            }
            return null;
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
                catch (Exception ex) 
                {
                    GlobalValues.Logger.Error($"Update record of {newRecord.Date} failed, {ex.Message}");
                }
            }
            return false;
        }


        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        public async Task<bool> Add(RecordEntity newRecord)
        {
            if((newRecord != null) && (_context.Records != null))
            {
                try
                {
                    await _context.Records.AddAsync(newRecord);
                    _context.SaveChanges();
                    return true;
                }
                catch (Exception ex) 
                {
                    GlobalValues.Logger.Error($"Add record of {newRecord.Date} failed, {ex.Message}");
                }
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
