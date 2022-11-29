using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using ILogger = North.Core.Services.Logger.ILogger;

namespace North.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ISqlSugarClient _db;

        public ImageController(ISqlSugarClient db, ILogger logger)
        {
            _db = db;
            _logger = logger;
        }


        [HttpGet]
        public async ValueTask<IActionResult> Get([FromQuery] string storager)
        {
            return await ValueTask.FromResult(new OkResult());
        }
    }
}