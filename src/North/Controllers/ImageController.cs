using Microsoft.AspNetCore.Mvc;
using North.Data.Access;

namespace North.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly OurDbContext _context;
        private readonly Core.Services.Logger.ILogger _logger;

        public ImageController(OurDbContext context, Core.Services.Logger.ILogger logger)
        {
            _context = context;
            _logger = logger;
        }
    }
}