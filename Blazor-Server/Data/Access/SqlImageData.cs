using ImageBed.Common;
using ImageBed.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ImageBed.Data.Access
{
    public class SqlImageData
    {
        public OurDbContext? _context { get; set; }

        public SqlImageData(OurDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 获取数据库中符合条件的所有图片信息
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<ImageEntity> GetAll(Func<ImageEntity, bool> predicate)
        {
            if ((_context != null) && (_context.Images != null))
            {
                return _context.Images.Where(predicate);
            }
            return Array.Empty<ImageEntity>();
        }


        /// <summary>
        /// 获取符合条件的第一张图片的信息
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ImageEntity? GetFirst(Expression<Func<ImageEntity, bool>> predicate)
        {
            if ((_context != null) && (_context.Images != null))
            {
                return _context.Images.FirstOrDefault(predicate);
            }
            return null;
        }


        /// <summary>
        /// 获取符合条件的第一张图片的信息
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<ImageEntity?> GetFirstAsync(Expression<Func<ImageEntity, bool>> predicate)
        {
            if ((_context != null) && (_context.Images != null))
            {
                return await _context.Images.FirstOrDefaultAsync(predicate);
            }
            return null;
        }


        /// <summary>
        /// 添加图片信息至数据库
        /// </summary>
        /// <param name="image"></param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public async Task<bool> AddAsync(ImageEntity image)
        {
            if ((_context != null) && (_context.Images != null) && (image != null))
            {
                try
                {
                    await _context.Images.AddAsync(image);
                    if(await _context.SaveChangesAsync() > 0)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    GlobalValues.Logger.Error($"Add {image.Name} to database failed, {ex.Message}");
                }
            }
            return false;
        }


        /// <summary>
        /// 添加多张图片至数据库
        /// </summary>
        /// <param name="images"></param>
        /// <returns>添加成功返回true， 否则返回false</returns>
        public async Task<bool> AddRangeAsync(IEnumerable<ImageEntity> images)
        {
            if ((_context != null) && (_context.Images != null) && (images != null))
            {
                try
                {
                    await _context.Images.AddRangeAsync(images);
                    if(await _context.SaveChangesAsync() == images.Count())
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    GlobalValues.Logger.Error($"Add pictures to database failed, {ex.Message}");
                }
            }
            return false;
        }


        /// <summary>
        /// 更新图片信息
        /// </summary>
        /// <param name="image"></param>
        /// <returns>更新成功返回true, 否则返回false</returns>
        public async Task<bool> UpdateAsync(ImageEntity image)
        {
            if ((_context != null) && (_context.Images != null))
            {
                try
                {
                    _context.Images.Update(image);
                    if(await _context.SaveChangesAsync() > 0)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    GlobalValues.Logger.Error($"Update {image.Name} failed, {ex.Message}");
                }
            }
            return false;
        }


        /// <summary>
        /// 移除图片(和磁盘文件)
        /// </summary>
        /// <param name="id"></param>
        /// <returns>移除成功返回true, 否则返回false</returns>
        public async Task<bool> RemoveAsync(string id)
        {
            ImageEntity? image = null;
            if ((_context != null) && (_context.Images != null) && ((image = await GetFirstAsync(i => i.Id == id)) != null))
            {
                try
                {
                    _context.Images.Remove(image);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        // 删除磁盘上的文件
                        File.Delete($"{GlobalValues.AppSetting?.Data.Image.RootPath}/{image.Name}");
                        File.Delete($"{GlobalValues.AppSetting?.Data.Image.RootPath}/thumbnails_{image.Name}");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    GlobalValues.Logger.Error($"Remove {image.Name} failed, {ex.Message}");
                }
            }
            return false;
        }


        /// <summary>
        /// 移除图片信息(和磁盘文件)
        /// </summary>
        /// <param name="image"></param>
        /// <returns>移除成功返回true， 否则返回false</returns>
        public async Task<bool> RemoveAsync(ImageEntity image)
        {
            if ((_context != null) && (_context.Images != null))
            {
                try
                {
                    _context.Images.Remove(image);
                    if(await _context.SaveChangesAsync() > 0)
                    {
                        File.Delete($"{GlobalValues.AppSetting?.Data.Image.RootPath}/{image.Name}");
                        File.Delete($"{GlobalValues.AppSetting?.Data.Image.RootPath}/thumbnails_{image.Name}");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    GlobalValues.Logger.Error($"Remove {image.Name} failed, {ex.Message}");
                }
            }
            return false;
        }


        /// <summary>
        /// 移除图片信息
        /// </summary>
        /// <param name="image"></param>
        /// <returns>移除成功返回true， 否则返回false</returns>
        public bool Remove(ImageEntity image)
        {
            if ((_context != null) && (_context.Images != null))
            {
                try
                {
                    _context.Images.Remove(image);
                    if(_context.SaveChanges() > 0)
                    {
                        File.Delete($"{GlobalValues.AppSetting?.Data.Image.RootPath}/{image.Name}");
                        File.Delete($"{GlobalValues.AppSetting?.Data.Image.RootPath}/thumbnails_{image.Name}");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    GlobalValues.Logger.Error($"Remove {image.Name} failed, {ex.Message}");
                }
            }
            return false;
        }


        /// <summary>
        /// 移除多个图片信息
        /// </summary>
        /// <param name="images"></param>
        /// <returns>移除成功返回true， 否则返回false</returns>
        public async Task<bool> RemoveRangeAsync(IEnumerable<ImageEntity> images)
        {
            if ((_context != null) && (_context.Images != null))
            {
                try
                {
                    _context.Images.RemoveRange(images);
                    if(await _context.SaveChangesAsync() == images.Count())
                    {
                        foreach (var image in images)
                        {
                            File.Delete($"{GlobalValues.AppSetting.Data.Image.RootPath}/{image.Name}");
                            File.Delete($"{GlobalValues.AppSetting.Data.Image.RootPath}/thumbnails_{image.Name}");
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    GlobalValues.Logger.Error($"Remove images failed, {ex.Message}");
                }
            }
            return false;
        }
    }
}
