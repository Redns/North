using System.Reflection;

namespace North.Pages.Settings
{
    partial class About
    {
        public string AppVersion { get; } = Assembly.GetEntryAssembly()?
                                                    .GetName()?
                                                    .Version?
                                                    .ToString() ?? "获取版本号失败";
    }
}
