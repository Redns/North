using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using static ImageBed.Common.UnitNameGenerator;
using static ImageBed.Data.Access.SQLImageData;

namespace ImageBed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<object>> Post([FromForm] IFormCollection formCollection)
        {
            var imageConfig = GlobalValues.appSetting.Data.Resources.Images;

            List<string> imageUrls = new();             // 图片url(相对路径)
            List<ImageEntity> images = new();           // 图片信息

            GlobalValues.Logger.Info("Uploading images...");

            try
            {
                using (var context = new OurDbContext())
                {
                    // 获取图片存储路径
                    string imageDir = $"{imageConfig.Path}";
                    Directory.CreateDirectory(imageDir);

                    var sqlImageData = new SQLImageData(context);
                    var uploadImages = (FormFileCollection)formCollection.Files;
                    
                    // 检查上传图片是否满足数量限制
                    if((imageConfig.MaxNum > 0) && (uploadImages.Count > imageConfig.MaxNum))
                    {
                        int indexStart = imageConfig.MaxNum;
                        int indexCount = uploadImages.Count - imageConfig.MaxNum;
                        uploadImages.RemoveRange(indexStart, indexCount);
                    }

                    foreach (IFormFile fileReader in uploadImages)
                    {
                        try
                        {
                            if (GetFileType(GetFileExtension(fileReader.FileName) ?? "") == FileType.COMPRESS)
                            {
                                // 上传文件为压缩包
                                GlobalValues.Logger.Info("Importing images...");

                                string importFullPath = $"{imageDir}/Import.zip";
                                using(var fileReadStream = fileReader.OpenReadStream())
                                {
                                    await FileOperator.SaveFile(fileReadStream, importFullPath);
                                    _ = fileReadStream.FlushAsync();
                                }
                                
                                foreach(var image in await FileOperator.ImportImages(importFullPath, imageDir))
                                {
                                    images.Add(image);
                                    imageUrls.Add($"{image.Url}");
                                } 
                            }
                            else
                            {
                                if((imageConfig.MaxSize <= 0) || fileReader.Length <= imageConfig.MaxSize * 1024*1024)
                                {
                                    ImageEntity image;
                                    using(var imageReadStream = fileReader.OpenReadStream())
                                    {
                                        image = await FileOperator.SaveImage(imageReadStream, fileReader.FileName, imageDir);
                                        await imageReadStream.FlushAsync();
                                    }
                                    images.Add(image);
                                    imageUrls.Add($"{image.Url}");
                                }
                                else
                                {
                                    imageUrls.Add(string.Empty);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            imageUrls.Add(string.Empty);
                            GlobalValues.Logger.Error($"Upload failed, imageName: {fileReader.FileName}");
                        }
                    }
                    await sqlImageData.AddRangeAsync(images);
                }
                GlobalValues.Logger.Info("Upload finished");
            }
            catch (Exception ex)
            {
                GlobalValues.Logger.Error($"Upload image failed, {ex.Message}");
            }
            return new ApiResult<object>(200, "Upload finished", imageUrls);
        }


        /// <summary>
        /// 下载图片(或文件)
        /// </summary>
        /// <param name="imageName">图片(或文件)名称</param>
        /// <returns></returns>
        [HttpGet("{imageName}")]
        public async Task<IActionResult> Get(string imageName)
        {
            GlobalValues.Logger.Info($"Get image {imageName}");

            // 构造图片路径
            // 图片存储路径为 Data/Resources/Images
            // 当图片不存在时返回 "imageNotFound.jpg"
            var imageConfig = GlobalValues.appSetting.Data.Resources.Images;
            string imageFullPath = $"{imageConfig.Path}/{imageName}";
            string imageExtension = GetFileExtension(imageName) ?? string.Empty;
            if (!System.IO.File.Exists(imageFullPath))
            {
                return File(System.IO.File.ReadAllBytes($"{imageConfig.Path}/imageNotFound.jpg"), $"image/{imageExtension}");
            }
            else
            {
                // 为了实现代码公用, 这里的图片接口也会返回其他格式的文件
                if (GetFileType(imageExtension) == FileType.COMPRESS)
                {
                    return File(System.IO.File.ReadAllBytes(imageFullPath), "application/octet-stream");
                }
                else
                {
                    // 修改图片请求次数
                    // 更新图片的过程可以放在子线程中完成, 不需要等待
                    using (var context = new OurDbContext())
                    {
                        var sqlImageData = new SQLImageData(context);
                        var image = await sqlImageData.GetAsync(ImageFilter.NAME, imageName);
                        if (image != null)
                        {
                            image.RequestNum++;
                            await sqlImageData.UpdateAsync(image);
                        }
                    }
                    return File(System.IO.File.ReadAllBytes(imageFullPath), $"image/{imageExtension}");
                }
            }
        }


        /// <summary>
        /// 删除指定图片(文件)
        /// </summary>
        /// <param name="token">用户令牌</param>
        /// <param name="imageName">待删除的图片名称</param>
        /// <returns></returns>
        [HttpDelete("{imageName}")]
        public async Task<ApiResult<object>> Delete(string imageName)
        {
            GlobalValues.Logger.Info($"Del image {imageName}");

            // 删除磁盘上的文件
            var imageConfig = GlobalValues.appSetting.Data.Resources.Images;
            string? imageFullPath = $"{imageConfig.Path}/{imageName}";
            string? imageThumbnailsFullPath = $"{imageConfig.Path}/thumbnails_{imageName}";

            try
            {
                // 删除磁盘文件
                if (System.IO.File.Exists(imageFullPath)) { System.IO.File.Delete(imageFullPath); }
                if (System.IO.File.Exists(imageThumbnailsFullPath)) { System.IO.File.Delete(imageThumbnailsFullPath); }

                // 删除数据库信息
                using (var context = new OurDbContext())
                {
                    var sqlImageData = new SQLImageData(context);
                    var image = await sqlImageData.GetAsync(ImageFilter.NAME, imageName);
                    if (image != null)
                    {
                        await sqlImageData.RemoveAsync(image);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalValues.Logger.Error($"Del image failed, {ex.Message}");
            }
            
            return new ApiResult<object>(200, "Delete image success", null);
        }
    }
}
