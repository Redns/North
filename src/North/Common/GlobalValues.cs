using Krins.Nuget;
using Microsoft.AspNetCore.Authentication;
using North.Core.Common;
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

        // TODO 在 App.razor 中引用以动态加载 Razor 页面
        public static List<Assembly> PluginAssemblies { get; set; } = new();
    }
}
