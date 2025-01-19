using System.Reflection;

namespace North.Pages.Settings
{
    partial class About
    {
        /// <summary>
        /// 应用版本号
        /// </summary>
        private string? _appVersion;
        public string AppVersion
        {
            get
            {
                return _appVersion ??= Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString() ?? "获取版本号失败";
            }
        }


        private string? _buildTime;
        public string BuildTime
        {
            get
            {
                if(!string.IsNullOrEmpty(_buildTime))
                {
                    return _buildTime;
                }

                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                if (assemblyLocation != null)
                {
                    return _buildTime = $"编译于 {File.GetLastWriteTime(assemblyLocation)}";
                }

                return _buildTime = string.Empty;
            }
        }
    }
}
