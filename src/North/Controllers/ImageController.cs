using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using North.Core.Entities;

namespace North.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly NorthDbContext _context;
        private readonly Core.Services.Logger.ILogger _logger;

        public ImageController(NorthDbContext context, Core.Services.Logger.ILogger logger)
        {
            _context = context;
            _logger = logger;
        }
    }
}