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
                    // 格式化文件名(原文件名为fileReader.FileName)
                    string unitFileName = UnitNameGenerator.RenameFile(imageDirPath, fileReader.FileName, GlobalValues.appSetting.Data.Resources.Images.RenameFormat);
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
                    imageUrls.Add($"{GetHost()}/api/image/{unitFileName}");

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

                    await sqlImageData.Add(image);
                }
                catch (Exception)
                {
                    imageUrls.Add(string.Empty);
                }
            }
            return new ApiResult<object>(200, "Upload finished", imageUrls);
        }


        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="filename">图片名称</param>
        /// <returns></returns>
        [HttpGet("{filename}")]
        public IActionResult Get(string filename)
        {
            string imageDir = GlobalValues.appSetting?.Data?.Resources?.Images?.Path ?? "Data/Resources/Images";
            string imagePath = $"{imageDir}/{filename}";

            if (!System.IO.File.Exists(imagePath))
            {
                imagePath = $"{imageDir}/imageNotFound.jpg";
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
