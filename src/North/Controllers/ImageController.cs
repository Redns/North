using Microsoft.AspNetCore.Mvc;
using North.Common;

namespace North.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        /// <summary>
        /// 获取头像
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("avatar/{path}")]
        public IActionResult GetAvatar(string path)
        {
            var avatar = $"{GlobalValues.AvatarDir}/{path}";
            try
            {
                return File(System.IO.File.ReadAllBytes(avatar), $"image/{path.Split('.').Last()}");
            }
            catch (Exception)
            {
                return File(System.IO.File.ReadAllBytes($"{GlobalValues.AvatarDir}/default.png"), "image/png");
            }
        }
    }
}