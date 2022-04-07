using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using Microsoft.AspNetCore.Mvc;

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

            using var context = new OurDbContext();
            using var sqlImageData = new SQLImageData(context);

            // 加载设置文件
            string imageDirPath = GlobalValues.appSetting?.Data?.Resources?.Images?.Path ?? "Data/Resources/Images";
            if (!Directory.Exists(imageDirPath))
            {
                Directory.CreateDirectory(imageDirPath);
            }

            FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
            foreach (IFormFile fileReader in fileCollection)
            {
                try
                {
                    var extName = UnitNameGenerator.GetFileExtension(fileReader.FileName);
                    if(extName == "export")
                    {
                        // 创建解压文件夹
                        string importImagePath = $"{imageDirPath}/Import";
                        if (!Directory.Exists(importImagePath))
                        {
                            Directory.CreateDirectory(importImagePath);
                        }

                        // 保存并解压export文件
                        await SaveFile(fileReader.OpenReadStream(), $"{importImagePath}/ImportImages.export");
                        FileOperator.DeCompressMulti($"{importImagePath}/ImportImages.export", $"{importImagePath}/");

                        // 保存图片信息至数据库
                        IEnumerable<string> imagePaths = Directory.GetFiles(importImagePath);
                        foreach(string imagePath in imagePaths)
                        {
                            var imageName = imagePath.Split('\\').Last();
                            if(UnitNameGenerator.GetFileExtension(imageName) != "export")
                            {
                                var image = await SaveImage(new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), imageName, imageDirPath);
                                imageUrls.Add($"{GetHost()}{image.Url}");
                                await sqlImageData.Add(image);
                            }
                        }

                        // 删除文件夹
                        Directory.Delete(importImagePath, true);
                    }
                    else
                    {
                        var image = await SaveImage(fileReader.OpenReadStream(), fileReader.FileName, imageDirPath);
                        imageUrls.Add($"{GetHost()}{image.Url}");
                        await sqlImageData.Add(image);
                    }
                }
                catch (Exception)
                {
                    imageUrls.Add(string.Empty);
                }
            }
            return new ApiResult<object>(200, "Upload finished", imageUrls);
        }


        /// <summary>
        /// 保存图片到本地
        /// </summary>
        /// <param name="fileReader">图片输入流</param>
        /// <param name="filename">图片名称</param>
        /// <param name="imageDirPath">图片存储文件夹</param>
        /// <returns></returns>
        private async Task<ImageEntity> SaveImage(Stream fileReader, string filename, string imageDirPath)
        {
            // 格式化文件名
            string unitFileName = UnitNameGenerator.RenameFile(imageDirPath, filename, GlobalValues.appSetting.Data.Resources.Images.RenameFormat);
            string unitFilePath = $"{imageDirPath}/{unitFileName}";

            // 检查是否命名冲突
            if ((GlobalValues.appSetting.Data.Resources.Images.RenameFormat == UnitNameGenerator.RenameFormat.NONE) && System.IO.File.Exists(unitFilePath))
            {
                System.IO.File.Delete(unitFilePath);
            }

            // 保存图片
            using FileStream fileWriter = System.IO.File.Create(unitFilePath);
            await fileReader.CopyToAsync(fileWriter);
            fileWriter.Flush();
            fileWriter.Close();
            fileReader.Flush();
            fileReader.Close();

            // 录入数据库
            var fileInfo = new FileInfo(unitFilePath);
            var imageInfo = SixLabors.ImageSharp.Image.Load(unitFilePath);
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
            return image;
        }


        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="fileReader">文件输入流</param>
        /// <param name="dstDirPath">文件存储路径</param>
        /// <returns></returns>
        private async Task SaveFile(Stream fileReader, string dstDirPath)
        {
            if (System.IO.File.Exists(dstDirPath))
            {
                System.IO.File.Delete(dstDirPath);
            }
            using FileStream fileWriter = System.IO.File.Create(dstDirPath);
            await fileReader.CopyToAsync(fileWriter);
            fileWriter.Flush();
            fileWriter.Close();
            fileReader.Flush();
            fileReader.Close();
        }


        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="filename">图片名称</param>
        /// <returns></returns>
        [HttpGet("{filename}")]
        public async Task<IActionResult> Get(string filename)
        {
            string imageDir = GlobalValues.appSetting?.Data?.Resources?.Images?.Path ?? "Data/Resources/Images";
            string imagePath = $"{imageDir}/{filename}";

            if (!System.IO.File.Exists(imagePath))
            {
                imagePath = $"{imageDir}/imageNotFound.jpg";
            }

            // 修改请求次数
            using var context = new OurDbContext();
            using var sqlImageData = new SQLImageData(context);
            
            var image = await sqlImageData.GetByName(filename);
            if(image != null)
            {
                image.RequestNum++;
                await sqlImageData.Update(image);
            }

            return File(System.IO.File.ReadAllBytes(imagePath), $"image/{UnitNameGenerator.GetFileExtension(filename)}");
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
            string? imagePath = $"{GlobalValues.appSetting?.Data?.Resources?.Images?.Path ?? "Data/Resources/Images"}/{filename}";
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            return new ApiResult<object>(200, "Delete image success", null);
        }


        /// <summary>
        /// 获取完整host地址
        /// </summary>
        /// <returns></returns>
        private string GetHost()
        {
            string? host = string.Empty;
            if (Request.IsHttps)
            {
                host += $"https://{Request.Host}";
            }
            else
            {
                host += $"http://{Request.Host}";
            }
            return host;
        }
    }
}
