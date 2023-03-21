using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using North.Core.Common;
using North.Core.Entities;
using North.Core.Models;
using North.Plugin;
using System.Reflection;
using System.Runtime.Loader;

namespace North.Common
{
    public class PluginsContext
    {
        #region 插件页面、控制器更新回调函数
        public Action<IList<Assembly>> OnRefreshRazorPages { get; init; } = (assemblies) => { };
        public Action<IList<ApplicationPart>> OnRefreshControllers { get; init; } = (applicationParts) => { };
        #endregion

        #region 图片上传/下载委托
        public Func<List<ImageUploadModel>, UserDTOEntity?, Task> OnImageUpload { get; private set; } = (images, user) => Task.CompletedTask;
        public Func<HttpRequest, UserDTOEntity?, ValueTask<FileContentResult>> OnImageDownload { get; private set; } = (request, user) => ValueTask.FromResult(new FileContentResult(File.ReadAllBytes("Resources/Images/ImageNotFound.jpg"), "image/jpg"));
        #endregion

        #region 中间件
        // TODO 中间件测试
        private static Func<HttpContext, RequestDelegate, Task> _middlewares = (context, next) =>
        {
            var ipAddress = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            if (ipAddress == "0.0.0.1")
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            }
            else
            {
                return next(context);
            }
        };
        public static Func<HttpContext, RequestDelegate, Task> Middlewares
        {
            get
            {
                return _middlewares;
            }
        }
        #endregion

        private readonly object _lock = new();
        private readonly ApplicationPartManager _applicationPartManager;
        private readonly IActionDescriptorChangeProvider _actionDescriptorChangeProvider;
        private readonly List<PluginAssemblyLoadContext> _pluginContexts = new();

        #region Razor Pages 程序集
        private List<Assembly>? _razorPageAssemblies = null;

        public PluginsContext(ApplicationPartManager applicationPartManager, IActionDescriptorChangeProvider actionDescriptorChangeProvider)
        {
            _applicationPartManager = applicationPartManager;
            _actionDescriptorChangeProvider = actionDescriptorChangeProvider;
        }

        /// <summary>
        /// Razor Pages 程序集集合（程序集中不一定包含 Razor Pages）
        /// </summary>
        public List<Assembly> RazorPageAssembies
        {
            get
            {
                return _razorPageAssemblies ??= _pluginContexts.ConvertAll(context => context.Assembly);
            }
        }
        #endregion


        public void EnableRazorPages(PluginEntity plugin)
        {
            //var pluginContext = _pluginContexts.FirstOrDefault(context => context.Plugin.Id == plugin.Id);
            //if(pluginContext is not null)
            //{
            //    _razorPageAssemblies.FirstOrDefault(assembly => assembly.)
            //}
        }


        public void DisableRazorPages(PluginEntity plugin)
        {

        }
        

        #region 加载插件（磁盘 ---> 内存）
        /// <summary>
        /// 加载插件
        /// </summary>
        /// <param name="plugin">待加载的插件</param>
        public void Load(PluginEntity plugin)
        {
            _pluginContexts.Add(new PluginAssemblyLoadContext(plugin));
        }


        /// <summary>
        /// 加载多个插件
        /// </summary>
        /// <param name="plugins">待加载的插件集合</param>
        public void Load(IEnumerable<PluginEntity> plugins)
        {
            foreach(var plugin in plugins)
            {
                Load(plugin);
            }
        }
        #endregion

        #region 卸载插件（从内存中清除）
        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <param name="plugin">待卸载的插件</param>
        public void Unload(PluginEntity plugin)
        {
            var pluginContext = _pluginContexts.FirstOrDefault(context => context.Plugin.Id == plugin.Id);
            if(pluginContext is not null)
            {
                pluginContext.Unload();
                _pluginContexts.Remove(pluginContext);
            }
        }


        /// <summary>
        /// 卸载多个插件
        /// </summary>
        /// <param name="plugins">待卸载的插件</param>
        public void Unload(IEnumerable<PluginEntity> plugins)
        {
            foreach(var plugin in plugins)
            {
                Unload(plugin);
            }
        }
        #endregion


        /// <summary>
        /// 安装插件
        /// </summary>
        /// <param name="plugin">待安装的插件</param>
        /// <param name="setting">插件设置</param>
        public void Install(PluginEntity plugin, PluginSetting setting)
        {

        }


        /// <summary>
        /// 移除插件
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="setting"></param>
        public void UnInstall(PluginEntity plugin, PluginSetting setting)
        {
            
        }
    }


    /// <summary>
    /// 插件加载上下文
    /// </summary>
    public class PluginAssemblyLoadContext : AssemblyLoadContext
    {
        public PluginEntity Plugin { get; init; }
        public Assembly Assembly { get; init; }
        public AssemblyPart AssemblyPart { get; init; }

        #region 图片上传/下载模块集合
        public Dictionary<string, IAuth> Authers { get; init; } = new();
        public Dictionary<string, IParse> Parsers { get; init; } = new();
        public Dictionary<string, IStorage> Storagers { get; init; } = new();
        public Dictionary<string, IUploadNode> UploadNodes { get; init; } = new();
        public Dictionary<string, IDownloadNode> DownloadNodes { get; init; } = new();
        #endregion

        public PluginAssemblyLoadContext(PluginEntity plugin) : base(isCollectible: true)
        {
            Plugin = plugin;
            Assembly = LoadFromAssemblyPath(plugin.InstallDir);
            AssemblyPart = new AssemblyPart(Assembly);

            // 解析插件中的图片上传/下载模块
            foreach (var type in Assembly.GetTypes())
            {
                if (typeof(IAuth).IsAssignableFrom(type))
                {
                    var auther = Activator.CreateInstance(type) as IAuth;
                    if (auther is not null)
                    {
                        Authers.Add(type.FullName ?? throw new ArgumentNullException(type.FullName), auther);
                    }
                }
                else if (typeof(IParse).IsAssignableFrom(type))
                {
                    var parser = Activator.CreateInstance(type) as IParse;
                    if (parser is not null)
                    {
                        Parsers.Add(type.FullName ?? throw new ArgumentNullException(type.FullName), parser);
                    }
                }
                else if (typeof(IStorage).IsAssignableFrom(type))
                {
                    var storager = Activator.CreateInstance(type) as IStorage;
                    if (storager is not null)
                    {
                        Storagers.Add(type.FullName ?? throw new ArgumentNullException(type.FullName), storager);
                    }
                }
                else if (typeof(IUploadNode).IsAssignableFrom(type))
                {
                    var uploadNode = Activator.CreateInstance(type) as IUploadNode;
                    if (uploadNode is not null)
                    {
                        UploadNodes.Add(type.FullName ?? throw new ArgumentNullException(type.FullName), uploadNode);
                    }
                }
                else if (typeof(IDownloadNode).IsAssignableFrom(type))
                {
                    var downloadNode = Activator.CreateInstance(type) as IDownloadNode;
                    if (downloadNode is not null)
                    {
                        DownloadNodes.Add(type.FullName ?? throw new ArgumentNullException(type.FullName), downloadNode);
                    }
                }
            }
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }
    }


    /// <summary>
    /// 控制器更新 Provider
    /// </summary>
    public class NorthActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        public static NorthActionDescriptorChangeProvider Instance { get; } = new();

        public CancellationTokenSource TokenSource { get; private set; } = new();

        public bool HasChanged { get; set; }

        public IChangeToken GetChangeToken()
        {
            TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(TokenSource.Token);
        }
    }
}
