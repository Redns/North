using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using static ImageBed.Common.UnitNameGenerator;

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
            GlobalValues.Logger.Info("Uploading images...");

            List<string> imageUrls = new();
            List<ImageEntity> images = new();
            using (var context = new OurDbContext())
            {
                // 获取图片统一存储路径
                string imageDirPath = GlobalValues.appSetting?.Data?.Resources?.Images?.Path ?? "Data/Resources/Images";
                if (!Directory.Exists(imageDirPath))
                {
                    Directory.CreateDirectory(imageDirPath);
                }

                var sqlImageData = new SQLImageData(context);
                FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
                foreach (IFormFile fileReader in fileCollection)
                {
                    try
                    {
                        var extName = GetFileExtension(fileReader.FileName);
                        if (extName == "export")
                        {
                            GlobalValues.Logger.Info("Importing images...");

                            // export 为图片导出文件后缀
                            // 创建文件夹用于存储 export 解压文件
                            string importImagePath = $"{imageDirPath}/Import";
                            if (!Directory.Exists(importImagePath))
                            {
                                Directory.CreateDirectory(importImagePath);
                            }

                            // 保存并解压export文件
                            await SaveFile(fileReader.OpenReadStream(), $"{importImagePath}/ImportImages.export");
                            FileOperator.DeCompressMulti($"{importImagePath}/ImportImages.export", $"{importImagePath}/");

                            // 保存图片信息至数据库
                            GlobalValues.Logger.Info("Putting images into database...");

                            IEnumerable<string> imagePaths = Directory.GetFiles(importImagePath);
                            foreach (string imagePath in imagePaths)
                            {
                                var imageName = imagePath.Split('\\').Last();
                                if (GetFileExtension(imageName) != "export")
                                {
                                    var image = await SaveImage(new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), imageName, imageDirPath);
                                    imageUrls.Add($"{image.Url}");
                                    images.Add(image);
                                }
                            }
                            Directory.Delete(importImagePath, true);

                            GlobalValues.Logger.Info("Import finished");
                        }
                        else
                        {
                            var image = await SaveImage(fileReader.OpenReadStream(), fileReader.FileName, imageDirPath);
                            imageUrls.Add($"{image.Url}");
                            images.Add(image);
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
            return new ApiResult<object>(200, "Upload finished", imageUrls);
        }


        /// <summary>
        /// 保存图片到本地
        /// </summary>
        /// <param name="fileReader">图片输入流</param>
        /// <param name="filename">图片名称</param>
        /// <param name="imageDirPath">图片存储文件夹</param>
        /// <returns></returns>
        private static async Task<ImageEntity> SaveImage(Stream fileReader, string filename, string imageDirPath)
        {
            // 格式化文件名
            GlobalValues.Logger.Info($"Rename image... Current rename format is {GlobalValues.appSetting?.Data?.Resources?.Images?.RenameFormat}");
            RenameFormat renameFormat = GlobalValues.appSetting?.Data?.Resources?.Images?.RenameFormat ?? RenameFormat.MD5;
            string unitFileName = RenameFile(imageDirPath, filename, renameFormat);
            string unitFilePath = $"{imageDirPath}/{unitFileName}";

            // 检查是否命名冲突

            if ((renameFormat == RenameFormat.NONE) && System.IO.File.Exists(unitFilePath))
            {
                System.IO.File.Delete(unitFilePath);
            }

            // 保存图片
            using (FileStream fileWriter = System.IO.File.Create(unitFilePath))
            {
                await fileReader.CopyToAsync(fileWriter);
                await fileWriter.FlushAsync();
            }
            await fileReader.FlushAsync();
            fileReader.Dispose();

            // 录入数据库
            var fileInfo = new FileInfo(unitFilePath);
            var imageInfo = NetVips.Image.NewFromFile(unitFilePath);
            ImageEntity image = new()
            {
                Id = EncryptAndDecrypt.Encrypt_MD5(unitFileName),
                Name = unitFileName,
                Url = $"api/image/{unitFileName}",
                Dpi = $"{imageInfo.Width}*{imageInfo.Height}",
                Size = UnitNameGenerator.RebuildFileSize(fileInfo.Length),
                UploadTime = fileInfo.LastAccessTime.ToString(),
                Owner = "Admin"
            };
            imageInfo.Close();
            imageInfo.Dispose();
            
            return image;
        }


        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="fileReader">文件输入流</param>
        /// <param name="dstDirPath">文件存储路径</param>
        /// <returns></returns>
        private static async Task SaveFile(Stream fileReader, string dstDirPath)
        {
            if (System.IO.File.Exists(dstDirPath))
            {
                System.IO.File.Delete(dstDirPath);
            }
            using (FileStream fileWriter = System.IO.File.Create(dstDirPath))
            {
                await fileReader.CopyToAsync(fileWriter);
            }
            await fileReader.FlushAsync();
            fileReader.Dispose();
        }


        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="filename">图片名称</param>
        /// <returns></returns>
        [HttpGet("{filename}")]
        public async Task<IActionResult> Get(string filename)
        {
            GlobalValues.Logger.Info($"Get image {filename}");

            string imageDir = GlobalValues.appSetting?.Data?.Resources?.Images?.Path ?? "Data/Resources/Images";
            string imagePath = $"{imageDir}/{filename}";

            if (!System.IO.File.Exists(imagePath))
            {
                imagePath = $"{imageDir}/imageNotFound.jpg";
            }

            // 修改请求次数
            using (var context = new OurDbContext())
            {
                var sqlImageData = new SQLImageData(context);
                var image = await sqlImageData.GetByName(filename);
                if (image != null)
                {
                    image.RequestNum++;
                    _ = sqlImageData.Update(image);
                }
            }
            return File(System.IO.File.ReadAllBytes(imagePath), $"image/{GetFileExtension(filename)}");
        }


        /// <summary>
        /// 删除指定图片
        /// </summary>
        /// <param name="token">用户令牌</param>
        /// <param name="filename">待删除的图片名称</param>
        /// <returns></returns>
        [HttpDelete("{filename}")]
        public ApiResult<object> Delete(string filename)
        {
            GlobalValues.Logger.Info($"Del image {filename}");

            string? imagePath = $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path ?? "Data/Resources/Images"}/{filename}";
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            return new ApiResult<object>(200, "Delete image success", null);
        }
    }
}
