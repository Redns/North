using Microsoft.AspNetCore.Mvc;
using North.NSFW.Common;
using NsfwSpyNS;

namespace North.NSFW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassifyController : ControllerBase
    {
        private readonly INsfwSpy _nsfwSpy;

        public ClassifyController(INsfwSpy nsfwSpy)
        {
            _nsfwSpy = nsfwSpy;
        }


        [HttpPost]
        public async ValueTask<ApiResult<object>> Classify([FromForm] IFormCollection formCollection)
        {
            var images = (FormFileCollection)formCollection.Files;
            var imageClassifyResult = new List<object>();

            foreach(var image in images)
            {
                var buffer = new byte[image.Length];
                using var imageReadStream = image.OpenReadStream();
                if (await imageReadStream.ReadAsync(buffer) == buffer.Length)
                {
                    imageClassifyResult.Add(new
                    {
                        Name = image.FileName,
                        Nsfw = _nsfwSpy.ClassifyImage(buffer)
                    });
                }
            }

            return new ApiResult<object>(200, "Classify success", imageClassifyResult);
        }
    }
}
