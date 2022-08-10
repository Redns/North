using Microsoft.AspNetCore.Mvc;
using North.Common;
using North.Data.Access;

namespace North.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly OurDbContext _context;
        private readonly North.Services.Logger.ILogger _logger;

        public ImageController(OurDbContext context, Services.Logger.ILogger logger)
        {
            _context = context;
            _logger = logger;
        }


        /// <summary>
        /// 获取头像
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("avatar/{path}")]
        public IActionResult GetAvatar(string path)
        {
            try
            {
                return File(System.IO.File.ReadAllBytes($"{GlobalValues.AvatarDir}/{path}"), $"image/{path.Split('.').Last()}");
            }
            catch (Exception e)
            {
                _logger.Error($"Get avatar {path} failed", e);
                return File(System.IO.File.ReadAllBytes($"{GlobalValues.AvatarDir}/default.png"), "image/png");
            }
        }
    }
}