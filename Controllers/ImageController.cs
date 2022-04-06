using ImageBed.Common;
using ImageBed.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Net;
using System.Net.Http.Headers;

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

            AppSetting? appSetting = AppSetting.Parse();
            FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
            foreach (IFormFile fileReader in fileCollection)
            {
                try
                {
                    string imageDirPath = appSetting?.Data?.Resources?.Images?.Path ?? "Data/Resources/Images";
                    if (!Directory.Exists(imageDirPath))
                    {
                        Directory.CreateDirectory(imageDirPath);
                    }

                    // 格式化文件名
                    string unitFileName = $"{UnitNameGenerator.GetTimeStamp()}.{UnitNameGenerator.GetFileExtension(fileReader.FileName)}";
                    string unitFilePath = $"{imageDirPath}/{unitFileName}";
                    if (System.IO.File.Exists(unitFilePath))
                    {
                        System.IO.File.Delete(unitFilePath);
                    }

                    // 读取图片
                    using FileStream fileWriter = System.IO.File.Create(unitFilePath);
                    await fileReader.CopyToAsync(fileWriter);
                    fileWriter.Flush();
                    imageUrls.Add($"{GetHost()}/api/image/{unitFileName}");
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
            AppSetting? appSetting = AppSetting.Parse();

            string imageDir = appSetting?.Data?.Resources?.Images?.Path ?? "Data/Resources/Images";
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
            AppSetting? appSetting = AppSetting.Parse();

            string? imagePath = $"{appSetting?.Data?.Resources?.Images?.Path ?? "Data/Resources/Images"}/{filename}";
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
