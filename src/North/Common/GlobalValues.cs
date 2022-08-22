using Krins.Nuget;

namespace North.Common
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public static class GlobalValues
    {
        public static readonly string AvatarDir = "Data/Images/Avatars";
        public static readonly string[] WithoutAuthenticationPages = new string[] { "login", "register", "signin", "verify" };

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
    }
}
