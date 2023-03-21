using Microsoft.AspNetCore.Authentication;
using Microsoft.JSInterop;
using North.Core.Common;
using North.Core.Entities;
using North.Core.Repository;
using System.Security.Claims;

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
        public static string[] WithoutAuthenticationPages { get; } = new string[] { "login", "register", "signin", "verify", "install" };


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
        /// 应用程序是否安装完成
        /// </summary>
        /// TODO 根据机密文件是否存储有管理员账号密码判断
        private static bool _isApplicationInstalled = false;
        public static bool IsApplicationInstalled
        {
            get { return _isApplicationInstalled; }
        }
    }
}
