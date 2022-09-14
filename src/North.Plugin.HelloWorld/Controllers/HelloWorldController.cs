using Microsoft.AspNetCore.Mvc;

namespace North.Plugin.HelloWorld.Controllers
{
    [Route("api/hello_world")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Hello, World";
        }
    }
}