using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using North.Core.Common;
using North.Core.Entities;
using North.Plugin;
using System.Reflection;
using System.Runtime.Loader;

namespace North.Common
{
    public class PluginContext
    {
        #region 插件页面、控制器更新委托
        public Action<IList<Assembly>> OnRefreshRazorPages { get; init; } = (assemblies) => { };
        public Action<IList<ApplicationPart>> OnRefreshControllers { get; init; } = (applicationParts) => { };
        #endregion

        /// <summary>
        /// 插件所在父文件夹
        /// </summary>
        private string _pluginDir;

        private readonly ApplicationPartManager _applicationPartManager;

        #region 插件程序集
        private readonly List<Assembly> _assemblies = new();
        private readonly List<PluginEntity> _plugins = new();
        private readonly List<PluginAssemblyLoadContext> _contextes = new();
        #endregion

        #region 图片上传模块集合
        private readonly List<IStorage> _storages = new();
        private readonly Dictionary<string, List<IDownloadNode>> _nodes = new()
        {
            { "BeforeAuth", new() },
            { "BeforeStorage", new() },
            { "BeforeRecord", new() },
            { "AfterRecord", new() }
        };
        #endregion

        public List<Assembly> Assemblies => _assemblies;


        public PluginContext(string pluginDir, ApplicationPartManager applicationPartManager)
        {
            _pluginDir = pluginDir;
            _applicationPartManager = applicationPartManager;
        }


        #region 加载插件
        /// <summary>
        /// 加载插件
        /// </summary>
        /// <param name="plugin">待加载的插件</param>
        public void Load(PluginEntity plugin)
        {
            // 加载插件
            LoadWithoutRefresh(plugin);

            // 更新 Razor Pages 和 Controllers
            OnRefreshRazorPages(_assemblies);
            OnRefreshControllers(_applicationPartManager.ApplicationParts);
        }


        /// <summary>
        /// 加载插件
        /// </summary>
        /// <param name="plugins">待加载的插件集合</param>
        public void Load(IEnumerable<PluginEntity> plugins)
        {
            // 加载插件
            foreach(var plugin in plugins)
            {
                LoadWithoutRefresh(plugin);
            }

            // 更新 Razor Pages 和 Controllers
            OnRefreshRazorPages(_assemblies);
            OnRefreshControllers(_applicationPartManager.ApplicationParts);
        }


        /// <summary>
        /// 加载插件（不更新 Razor Pages 和 Controllers）
        /// </summary>
        /// <param name="plugin">待加载的插件</param>
        private void LoadWithoutRefresh(PluginEntity plugin)
        {
            // 创建程序集上下文
            // 直接使用程序集无法动态更新、卸载插件
            var pluginContext = new PluginAssemblyLoadContext(plugin.Id, Path.Combine(_pluginDir, plugin.Name));

            // 加载 Razor Pages
            _assemblies.Add(pluginContext.Assembly);

            // 加载 Controllers
            _applicationPartManager.ApplicationParts.Add(new AssemblyPart(pluginContext.Assembly));

            // 加载图片上传模块
            foreach (var module in plugin.Modules)
            {
                if (module.Category is "Storage")
                {
                    _storages.Add(module.Type as IStorage ?? throw new Exception($"Unable to convert module {module.Name}-{module.Id} to IStorage"));
                }
                else
                {
                    _nodes[module.Category].Add(module.Type as IDownloadNode ?? throw new Exception($"Unable to convert module {module.Name}-{module.Id} to INode"));
                }
            }

            _plugins.Add(plugin);
            _contextes.Add(pluginContext);
        }
        #endregion


        #region 卸载插件
        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <param name="plugin">待卸载的插件</param>
        public void Unload(PluginEntity plugin)
        {

            UnloadWithoutRefresh(plugin);

            OnRefreshRazorPages(_assemblies);
            OnRefreshControllers(_applicationPartManager.ApplicationParts);
        }


        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <param name="plugins">待卸载的插件集合</param>
        public void Unload(IEnumerable<PluginEntity> plugins)
        {
            foreach(var plugin in plugins)
            {
                UnloadWithoutRefresh(plugin);
            }

            OnRefreshRazorPages(_assemblies);
            OnRefreshControllers(_applicationPartManager.ApplicationParts);
        }


        /// <summary>
        /// 卸载插件（不更新 Razor Pages 和 Controllers）
        /// </summary>
        /// <param name="plugin">待卸载的插件</param>
        private void UnloadWithoutRefresh(PluginEntity plugin)
        {
            var context = _contextes.FirstOrDefault(c => c.Id == plugin.Id);
            if (context is not null)
            {
                context.Unload();

                _plugins.Remove(plugin);
                _contextes.Remove(context);
                _assemblies.Remove(context.Assembly);
            }
        }
        #endregion


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


    public class PluginAssemblyLoadContext : AssemblyLoadContext
    {
        public Guid Id { get; init; }
        public Assembly Assembly { get; init; }

        public PluginAssemblyLoadContext(Guid id, string assemblyPath) : base(isCollectible: true)
        {
            Id = id;
            Assembly = LoadFromAssemblyPath(assemblyPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }
    }


    public class NorthActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        public static NorthActionDescriptorChangeProvider Instance { get; } = new();

        public CancellationTokenSource TokenSource { get; private set; }

        public bool HasChanged { get; set; }

        public IChangeToken GetChangeToken()
        {
            TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(TokenSource.Token);
        }
    }
}
