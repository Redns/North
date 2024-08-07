﻿using Microsoft.AspNetCore.Mvc;
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
        /// <summary>
        /// 全局锁
        /// </summary>
        private readonly object _lock = new();

        /// <summary>
        /// 插件程序集上下文
        /// </summary>
        private readonly List<PluginAssemblyLoadContext> _pluginContexts = [];

        /// <summary>
        /// forget what it is
        /// </summary>
        private readonly ApplicationPartManager _applicationPartManager;

        /// <summary>
        /// forget what it is
        /// </summary>
        private readonly IActionDescriptorChangeProvider _actionDescriptorChangeProvider;

        /// <summary>
        /// Razor Pages 程序集集合
        /// 注意程序集中不一定包含 Razor Pages
        /// </summary>>
        public readonly List<Assembly> _razorPageAssembies = [];
        public List<Assembly> RazorPageAssembies
        {
            get
            {
                return _razorPageAssembies;
            }
        }


        #region 事件函数

        /// <summary>
        /// 中间件
        /// </summary>
        public Func<HttpContext, RequestDelegate, Task> Middleware { get; private set; } = (context, next) => { return next(context); };

        /// <summary>
        /// Razor 页面更新
        /// </summary>
        public Action<IList<Assembly>> OnRefreshRazorPages { get; init; } = (assemblies) => { };

        /// <summary>
        /// 控制器更新
        /// </summary>
        public Action<IList<ApplicationPart>> OnRefreshControllers { get; init; } = (applicationParts) => { };

        /// <summary>
        /// 图片上传
        /// </summary>
        public Func<List<ImageUploadModel>, UserDTOEntity?, Task> OnUpload { get; private set; } = (images, user) => Task.CompletedTask;

        /// <summary>
        /// 图片下载
        /// </summary>
        public Func<HttpRequest, UserDTOEntity?, ValueTask<FileContentResult>> OnDownload { get; private set; } = (request, user) => ValueTask.FromResult(new FileContentResult(File.ReadAllBytes("Resources/Images/ImageNotFound.jpg"), "image/jpg"));

        #endregion

        public PluginsContext(ApplicationPartManager applicationPartManager, IActionDescriptorChangeProvider actionDescriptorChangeProvider)
        {
            _applicationPartManager = applicationPartManager;
            _actionDescriptorChangeProvider = actionDescriptorChangeProvider;
        }

        public void EnableRazorPages(PluginEntity plugin)
        {
            throw new NotImplementedException();
        }

        public void DisableRazorPages(PluginEntity plugin)
        {
            throw new NotImplementedException();
        }
        

        #region 加载 / 卸载插件

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


        #region 安装 / 卸载插件

        /// <summary>
        /// 安装插件
        /// </summary>
        /// <param name="plugin">待安装的插件</param>
        /// <param name="setting">插件设置</param>
        public void Install(PluginEntity plugin, PluginSetting setting)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 移除插件
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="setting"></param>
        public void UnInstall(PluginEntity plugin, PluginSetting setting)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    /// <summary>
    /// 插件加载上下文
    /// </summary>
    public class PluginAssemblyLoadContext : AssemblyLoadContext
    {
        public PluginEntity Plugin { get; init; }
        public Assembly Assembly { get; init; }
        public AssemblyPart AssemblyPart { get; init; }


        #region Modules (图片上传/下载模块集合)
        public Dictionary<PluginCategory, Dictionary<string, Type>> Modules { get; init; } = [];
        public Dictionary<string, IAuth> Authers { get; init; } = [];
        public Dictionary<string, IParse> Parsers { get; init; } = [];
        public Dictionary<string, IStorage> Storagers { get; init; } = [];
        public Dictionary<string, IUploadNode> UploadNodes { get; init; } = [];
        public Dictionary<string, IDownloadNode> DownloadNodes { get; init; } = [];
        #endregion

        #region Middlewares

        /// <summary>
        /// 中间件
        /// </summary>
        public Dictionary<string, IMiddleware> Middlewares { get; init; } = [];

        #endregion

        public PluginAssemblyLoadContext(PluginEntity plugin) : base(isCollectible: true)
        {
            Plugin = plugin;
            Assembly = LoadFromAssemblyPath(plugin.InstallDir);
            AssemblyPart = new AssemblyPart(Assembly);

            /**
             * 解析插件子模块
             * // TODO 采用反射实现
             */
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
                else if(typeof(IMiddleware).IsAssignableFrom(type))
                {
                    var middleware = Activator.CreateInstance(type) as IMiddleware;
                    if (middleware is not null)
                    {
                        Middlewares.Add(type.FullName ?? throw new ArgumentNullException(type.Name), middleware);
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

    /// <summary>
    /// 插件类别
    /// </summary>
    public enum PluginCategory
    {
        Auther = 0,
        Parser,
        Storager,
        UploadNode,
        DownloadNode,
        Middleware
    }
}
