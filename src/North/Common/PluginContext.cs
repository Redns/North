using Microsoft.AspNetCore.Mvc.ApplicationParts;
using North.Core.Common;
using North.Core.Entities;
using North.PluginBase;
using System.Reflection;
using ILogger = North.Core.Services.Logger.ILogger;

namespace North.Common
{
    public class PluginContext
    {
        #region 插件页面、控制器更新委托
        public Action<IList<Assembly>> OnReloadRazorPages { get; set; } = (assemblies) => { };
        public Action<IList<ApplicationPart>> OnReloadControllers { get; set; } = (applicationParts) => { };
        #endregion

        public ILogger? Logger { get; set; }

        private string _pluginDir;
        private ApplicationPartManager _applicationPartManager;

        #region 插件程序集
        private readonly List<Assembly> _assemblies = new();
        private readonly List<PluginEntity> _plugins = new();
        private readonly List<CollectibleAssemblyLoadContext> _contextes = new();
        #endregion

        #region 图片上传模块集合
        private readonly List<IStorage> _storages = new();
        private readonly Dictionary<string, List<INode>> _nodes = new()
        {
            { "BeforeAuth", new() },
            { "BeforeStorage", new() },
            { "BeforeRecord", new() },
            { "AfterRecord", new() }
        };
        #endregion

        public List<Assembly> Assemblies => _assemblies;


        public PluginContext(List<PluginEntity> plugins, string pluginDir, ApplicationPartManager applicationPartManager)
        {
            _plugins = plugins;
            _pluginDir = pluginDir;
            _applicationPartManager = applicationPartManager;
        }


        public void Load()
        {
            foreach (var plugin in _plugins)
            {
                try
                {
                    // 创建程序集上下文
                    // 直接使用程序集无法动态更新、卸载插件
                    var pluginContext = new CollectibleAssemblyLoadContext(Path.Combine(_pluginDir, plugin.Name));

                    // 加载 Razor Pages
                    _assemblies.Add(pluginContext.Assembly);

                    // 加载 Controllers
                    _applicationPartManager.ApplicationParts.Add(new AssemblyPart(pluginContext.Assembly));

                    // 加载图片上传模块
                    foreach(var module in plugin.Modules)
                    {
                        if(module.Category is "Storage")
                        {
                            _storages.Add(module.Type as IStorage ?? throw new Exception($"Unable to convert module {module.Name}-{module.Id} to IStorage"));
                        }
                        else
                        {
                            _nodes[module.Category].Add(module.Type as INode ?? throw new Exception($"Unable to convert module {module.Name}-{module.Id} to INode"));
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger?.Error($"Load plugin {plugin.Name}({plugin.Id}) failed", e);
                }
            }

            // 更新 Razor Pages 和 Controllers
            OnReloadRazorPages(_assemblies);
            OnReloadControllers(_applicationPartManager.ApplicationParts);

            // 对图片上传模块按照执行顺序排序
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void UnLoad(CollectibleAssemblyLoadContext context)
        {
            context.Unload();
        }


        /// <summary>
        /// 安装插件
        /// </summary>
        /// <param name="plugin">待安装的插件</param>
        /// <param name="setting">插件设置</param>
        public static void Install(PluginEntity plugin, PluginSetting setting)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="setting"></param>
        public static void UnInstall(PluginEntity plugin, PluginSetting setting)
        {
            Directory.Delete(Path.Combine(setting.InstallDir, plugin.Name), true);
        }
    }
}
