using ImageBed.Common;
using Microsoft.AspNetCore.Mvc;

namespace ImageBed.Controllers
{
    [Consumes("application/json", "multipart/form-data")]
    [Route("api/image")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        /// POST api/image
        [HttpPost]
        public async Task<ActionResult<object>> Post([FromForm] IFormCollection formcollection)
        {
            try
            {
                // 规范图片名称
                var file = formcollection.Files[0];
                string unitImageName = ((long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString() + Path.GetExtension(file.FileName);

                // 复制图片文件
                using var imageReader = file.OpenReadStream();
                using var imageWriter = new FileStream($"Assets/Images/" + unitImageName, FileMode.OpenOrCreate, FileAccess.Write);
                await imageReader.CopyToAsync(imageWriter);
                return new
                {
                    Status_Code = 200,
                    Message = "upload success",
                    Url = $"{AppSettings.Get("imageBed:url")}/api/image/{unitImageName}"
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Status_Code = 400,
                    Message = $"upload failed,{ex.Message}"
                };
            }
        }


        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="image_name">图片名称</param>
        /// <returns></returns>
        /// GET api/image/{image_name}
        [HttpGet("{image_name}")]
        public async Task<IActionResult?> Get(string image_name)
        {
            try
            {
                using var imageReader = new FileStream($"Assets/Images/{image_name}", FileMode.Open);
                byte[] buffer = new byte[imageReader.Length];
                await imageReader.ReadAsync(buffer);
                return new FileContentResult(buffer, "image/jpeg");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
