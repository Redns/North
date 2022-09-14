using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using MudBlazor.Utilities;
using North.Common;
using North.Core.Common;
using System.Reflection;

namespace North.Controllers
{
    [Route("api/plugin")]
    [ApiController]
    public class PluginTestController : ControllerBase
    {
        private readonly ApplicationPartManager _partManager;

        public PluginTestController(ApplicationPartManager partManager)
        {
            _partManager = partManager;
        }

        [HttpGet("enable")]
        public string EnablePlugin()
        {
            var pluginAssembly = Assembly.LoadFile("E:\\Project\\North\\src\\North.Plugin.HelloWorld\\bin\\Debug\\net6.0\\North.Plugin.HelloWorld.dll");
            var pluginAssemblyPart = new AssemblyPart(pluginAssembly);

            // 加载 Razor Page
            GlobalValues.PluginAssemblies.Add(pluginAssembly);
            // 加载控制器
            _partManager.ApplicationParts.Add(pluginAssemblyPart);
            NorthActionDescriptorChangeProvider.Instance.HasChanged = true;
            NorthActionDescriptorChangeProvider.Instance.TokenSource.Cancel();

            return "Enabled";
        }
    }
}
