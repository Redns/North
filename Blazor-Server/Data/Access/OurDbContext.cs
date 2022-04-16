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
    /// 处理数据库中的图片信息
    /// </summary>
    public class SQLImageData
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
                    await _context.SaveChangesAsync();
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
        /// <returns>添加成功返回true， 否则返回false</returns>
        public async Task AddRangeAsync(IEnumerable<ImageEntity> images)
        {
            GlobalValues.Logger.Info("Putting images into database");
            if ((_context != null) && (_context.Images != null) && (images != null))
            {
                try
                {
                    await _context.Images.AddRangeAsync(images);
                    await _context.SaveChangesAsync();
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
        /// <param name="image">更新后的图片实体</param>
        /// <returns>更新成功返回true, 否则返回false</returns>
        public async Task<bool> UpdateAsync(ImageEntity image)
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
        /// 图片过滤方式
        /// </summary>
        public enum ImageFilter
        {
            ID,             // 根据ID过滤
            NAME,           // 根据名称过滤
            UploadTime,     // 上传时间 xxxx/xx/xx xx:xx:xx
            UpoadDate       // 上传日期 xxxx/xx/xx
        }


        /// <summary>
        /// 获取数据库中的所有图片信息
        /// </summary>
        /// <returns>获取到的全部信息</returns>
        public async Task<List<ImageEntity>> GetAsync()
        {
            if((_context != null) && (_context.Images != null))
            {
                return await _context.Images.ToListAsync();
            }
            return new List<ImageEntity>();
        }


        /// <summary>
        /// 获取数据库中的所有图片信息
        /// </summary>
        /// <returns>获取到的全部信息</returns>
        public async Task<ImageEntity[]> GetArrayAsync()
        {
            if ((_context != null) && (_context.Images != null))
            {
                return await _context.Images.ToArrayAsync();
            }
            return new ImageEntity[1];
        }


        /// <summary>
        /// 获取图片信息
        /// </summary>
        /// <param name="filter">过滤器类型</param>
        /// <param name="param">过滤器参数</param>
        /// <returns>获取到的图片信息</returns>
        public async Task<ImageEntity?> GetAsync(ImageFilter filter, object param)
        {
            if((_context != null) && (_context.Images != null))
            {
                return filter switch
                {
                    ImageFilter.ID => await _context.Images.FirstOrDefaultAsync(i => i.Id == param.ToString()),
                    ImageFilter.NAME => await _context.Images.FirstOrDefaultAsync(i => i.Name == param.ToString()),
                    ImageFilter.UploadTime => await _context.Images.FirstOrDefaultAsync(i => i.UploadTime == param.ToString()),
                    ImageFilter.UpoadDate => await _context.Images.FirstOrDefaultAsync(i => i.UploadTime.Contains(param.ToString() ?? "xxxx/xx/xx")),
                    _ => null
                };
            }
            return null;
        }


        /// <summary>
        /// 移除图片信息
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <returns>移除成功返回true, 否则返回false</returns>
        public async Task<bool> RemoveAsync(string id)
        {
            if ((_context != null) && (_context.Images != null))
            {
                try
                {
                    ImageEntity? image = await GetAsync(ImageFilter.ID, id);
                    if(image != null)
                    {
                        _context.Images.Remove(image);
                        await _context.SaveChangesAsync();
                    }

                    // 删除磁盘上的文件
                    string? imageFullPath = $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path}/{image.Name}";
                    string? imageThumbnailsFullPath = $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path}/thumbnails_{image.Name}";
                    if (File.Exists(imageFullPath))
                    {
                        File.Delete(imageFullPath);
                    }
                    if (File.Exists(imageThumbnailsFullPath))
                    {
                        File.Delete(imageThumbnailsFullPath);
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
        /// 移除图片信息
        /// </summary>
        /// <param name="image">图片实体</param>
        /// <returns>移除成功返回true， 否则返回false</returns>
        public async Task<bool> RemoveAsync(ImageEntity image)
        {
            if((_context != null) && (_context.Images != null))
            {
                try
                {
                    // 删除磁盘上的文件
                    // 删除磁盘上的文件
                    string? imageFullPath = $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path}/{image.Name}";
                    string? imageThumbnailsFullPath = $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path}/thumbnails_{image.Name}";
                    if (File.Exists(imageFullPath))
                    {
                        File.Delete(imageFullPath);
                    }
                    if (File.Exists(imageThumbnailsFullPath))
                    {
                        File.Delete(imageThumbnailsFullPath);
                    }

                    _context.Images.Remove(image);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch(Exception ex)
                {
                    GlobalValues.Logger.Error($"Remove image failed, {ex.Message}");
                }
            }
            return false;
        }


        /// <summary>
        /// 移除图片信息
        /// </summary>
        /// <param name="image">图片实体</param>
        /// <returns>移除成功返回true， 否则返回false</returns>
        public bool Remove(ImageEntity image)
        {
            if ((_context != null) && (_context.Images != null))
            {
                try
                {
                    // 删除磁盘上的文件
                    // 删除磁盘上的文件
                    string? imageFullPath = $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path}/{image.Name}";
                    string? imageThumbnailsFullPath = $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path}/thumbnails_{image.Name}";
                    if (File.Exists(imageFullPath))
                    {
                        File.Delete(imageFullPath);
                    }
                    if (File.Exists(imageThumbnailsFullPath))
                    {
                        File.Delete(imageThumbnailsFullPath);
                    }

                    _context.Images.Remove(image);
                    _context.SaveChanges();
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
        /// <param name="images">待移除的图片列表</param>
        /// <returns>移除成功返回true， 否则返回false</returns>
        public async Task<bool> RemoveRangeAsync(IEnumerable<ImageEntity> images)
        {
            if ((_context != null) && (_context.Images != null))
            {
                try
                {
                    string imageDir = $"{GlobalValues.appSetting.Data.Resources.Images.Path}";
                    foreach(var image in images)
                    {
                        // 删除磁盘上的文件
                        string? imageFullPath = $"{imageDir}/{image.Name}";
                        string? imageThumbnailsFullPath = $"{imageDir}/thumbnails_{image.Name}";
                        if (File.Exists(imageFullPath)) { File.Delete(imageFullPath); }
                        if (File.Exists(imageThumbnailsFullPath)) { File.Delete(imageThumbnailsFullPath); }
                    }

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
    }


    /// <summary>
    /// 处理数据库中的资源记录信息
    /// </summary>
    public class SQLRecordData
    {
        private OurDbContext _context { get; set; }
        public SQLRecordData(OurDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 资源记录筛选器
        /// </summary>
        public enum RecordFilter
        {
            NONE = 0,               // 不筛选
            DATE,                   // 根据日期筛选
            UploadNum,              // 根据上传数量筛选 [min, max]
            UploadSize,             // 根据上传尺寸筛选 [min, max]
            RequestNum              // 根据请求次数筛选 [min, max]
        }


        /// <summary>
        /// 获取所有记录数据
        /// </summary>
        /// <returns>获取到的所有记录信息</returns>
        public async Task<List<RecordEntity>> GetAsync()
        {
            if((_context != null) && (_context.Records != null))
            {
                return await _context.Records.ToListAsync();
            }
            return new List<RecordEntity>();
        }


        /// <summary>
        /// 获取指定的记录数据
        /// </summary>
        /// <param name="filter">过滤器类型</param>
        /// <param name="param">过滤器参数</param>
        /// <returns></returns>
        public async Task<RecordEntity?> GetAsync(RecordFilter filter, object param)
        {
            if ((_context != null) && (_context.Records != null))
            {
                return filter switch
                {
                    RecordFilter.DATE => await _context.Records.FirstOrDefaultAsync(r => r.Date == param.ToString()),
                    RecordFilter.UploadNum => await _context.Records.FirstOrDefaultAsync(r => r.UploadImageNum == (int)param),
                    RecordFilter.UploadSize => await _context.Records.FirstOrDefaultAsync(r => r.UploadImageSize == (int)param),
                    RecordFilter.RequestNum => await _context.Records.FirstOrDefaultAsync(r => r.RequestNum == (int) param),
                    _ => null
                };
            }
            return null;
        }


        /// <summary>
        /// 更新记录信息
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(RecordEntity newRecord)
        {
            if((_context != null) && (_context.Records != null))
            {
                try
                {
                    RecordEntity? record = await GetAsync(RecordFilter.DATE, newRecord.Date);
                    if(record != null)
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
        public async Task<bool> AddAsync(RecordEntity newRecord)
        {
            if((newRecord != null) && (_context.Records != null))
            {
                try
                {
                    await _context.Records.AddAsync(newRecord);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex) 
                {
                    GlobalValues.Logger.Error($"Add record of {newRecord.Date} failed, {ex.Message}");
                }
            }
            return false;
        }
    }
}
