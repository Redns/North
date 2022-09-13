using Microsoft.AspNetCore.Mvc;

namespace North.Plugin.Demo.Controllers
{
    [Route("api/demo")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Hello, World";
        }
    }
}