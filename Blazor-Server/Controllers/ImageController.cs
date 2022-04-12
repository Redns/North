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
        /// <param name="token">用户令牌</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<object>> Post([FromForm] IFormCollection formCollection)
        {
            List<string> imageUrls = new();
            List<ImageEntity> images = new();

            try
            {
                GlobalValues.Logger.Info("Uploading images...");
                using (var context = new OurDbContext())
                {
                    // 获取图片存储路径
                    // 若文件夹不存在则创建
                    string imageDir = $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path}";
                    if (!Directory.Exists(imageDir))
                    {
                        Directory.CreateDirectory(imageDir);
                    }

                    var sqlImageData = new SQLImageData(context);
                    var uploadImages = (FormFileCollection)formCollection.Files;
                    var imageMaxNumLimit = GlobalValues.appSetting?.Data?.Resources?.Images?.MaxNum ?? 0;
                    if((imageMaxNumLimit > 0) && (uploadImages.Count > imageMaxNumLimit))
                    {
                        int indexStart = imageMaxNumLimit;
                        int indexCount = uploadImages.Count - imageMaxNumLimit;
                        uploadImages.RemoveRange(indexStart, indexCount);
                    }

                    foreach (IFormFile fileReader in uploadImages)
                    {
                        try
                        {
                            if (GetFileType(GetFileExtension(fileReader.FileName) ?? "") == FileType.COMPRESS)
                            {
                                GlobalValues.Logger.Info("Importing images...");

                                string importFullPath = $"{imageDir}/Import.zip";
                                await FileOperator.SaveFile(fileReader.OpenReadStream(), importFullPath);
                                foreach(var image in await FileOperator.ImportImages(importFullPath, imageDir))
                                {
                                    images.Add(image);
                                    imageUrls.Add($"{image.Url}");
                                } 
                            }
                            else
                            {
                                int imageMaxSizeLimit = GlobalValues.appSetting.Data.Resources.Images.MaxSize;
                                if((imageMaxSizeLimit <= 0) || fileReader.Length <= imageMaxSizeLimit*1024*1024)
                                {
                                    var image = await FileOperator.SaveImage(fileReader.OpenReadStream(), fileReader.FileName, imageDir);
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
                            GlobalValues.Logger.Error($"Upload failed, imageName: {fileReader.FileName}");
                            imageUrls.Add(string.Empty);
                        }
                    }
                    _ = sqlImageData.AddRangeAsync(images);
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
            string imageDir = GlobalValues.appSetting?.Data?.Resources?.Images?.Path ?? string.Empty;
            string imageFullPath = $"{imageDir}/{imageName}";
            string imageExtension = GetFileExtension(imageName) ?? string.Empty;
            if (!System.IO.File.Exists(imageFullPath))
            {
                return File(System.IO.File.ReadAllBytes($"{imageDir}/imageNotFound.jpg"), $"image/{imageExtension}");
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
                            _ = sqlImageData.UpdateAsync(image);
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
            string? imageFullPath = $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path}/{imageName}";
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }

            // 删除数据库信息
            using (var context = new OurDbContext())
            {
                var sqlImageData = new SQLImageData(context);
                var image = await sqlImageData.GetAsync(ImageFilter.NAME, imageName);
                if(image != null)
                {
                    _ = sqlImageData.RemoveAsync(image);
                }
            }
            return new ApiResult<object>(200, "Delete image success", null);
        }
    }
}
