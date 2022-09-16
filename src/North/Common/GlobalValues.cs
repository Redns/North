using Krins.Nuget;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using North.Core.Common;
using North.Core.Entities;
using North.Core.Helpers;
using North.PluginBase;
using System.Reflection;

namespace North.Common
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public static class GlobalValues
    {
        /// <summary>
        /// 以下页面无需授权即可访问（防止 MainLayout 认证陷入死循环）
        /// </summary>
        public static string[] WithoutAuthenticationPages { get; } = new string[] { "login", "register", "signin", "verify" };


        /// <summary>
        /// 身份认证属性
        /// </summary>
        public static AuthenticationProperties AuthenticationProperties { get; } = new AuthenticationProperties()
        {
            IsPersistent = true,
            ExpiresUtc = DateTime.Now.AddSeconds(AppSettings.Auth.CookieValidTime)
        };

        /// <summary>
        /// 应用设置
        /// </summary>
        private static AppSetting? _appSetting;
        public static AppSetting AppSettings
        {
            get { return _appSetting ??= AppSetting.Load(); }
            set { _appSetting = value; }
        }

        /// <summary>
        /// Nuget 引擎
        /// </summary>
        private static NugetEngine? _nugetEngine;
        public static NugetEngine NugetEngine
        {
            get
            {
                return _nugetEngine ??= new NugetEngine();
            }
            set { _nugetEngine = value; }
        }

        
        public static Dictionary<string, List<INode>> PluginNodes { get; set; } = new()
        {
            { "BeforeAuth", new() },
            { "BeforeStorage", new() },
            { "BeforeRecord", new() },
            { "AfterRecord", new() }
        };
        public static List<IStorage> PluginStorages { get; set; } = new();
        public static List<Assembly> PluginAssemblies { get; set; } = new();
        public static List<CollectibleAssemblyLoadContext> PluginContexts { get; set; } = new();


        public static void Initialize(PluginEntity[] plugins)
        {
            // 加载插件
            // InitPlugins(plugins, AppSettings.Plugin);
        }


        private static void InitPlugin(PluginEntity[] plugins, PluginSetting setting, ApplicationPartManager applicationPartManager)
        {
            foreach(var plugin in plugins)
            {
                // 加载程序集上下文
                var context = new CollectibleAssemblyLoadContext(Path.Combine(setting.InstallDir, plugin.Name));

                // 加载 Razor 页面
                PluginAssemblies.Add(context.Assembly);

                // 加载控制器
                applicationPartManager.ApplicationParts.Add(new AssemblyPart(context.Assembly));
                
                // 加载图片上传处理模块
                foreach(var module in plugin.Modules)
                {
                    PluginNodes[module.Category].Add(module.Type as INode);
                }
                PluginContexts.Add(context);
            }
        }
    }
}
