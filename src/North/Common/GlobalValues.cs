using North.Core.Common;

namespace North.Common
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public static class GlobalValues
    {
        /// <summary>
        /// 以下页面无需授权即可访问（防止App.razor路由陷入死循环）
        /// </summary>
        public static string[] WithoutAuthenticationPages { get; } = new string[] { "login", "register", "signin", "verify", "install" };

        /// <summary>
        /// 应用设置
        /// </summary>
        private static AppSetting? _appSetting;
        public static AppSetting AppSettings
        {
            get { return _appSetting ??= AppSetting.Load(); }
            set { _appSetting = value; }
        }
    }
}
