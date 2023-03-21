using System.Reflection;

namespace North.Pages.Settings
{
    partial class About
    {
        public string AppVersion { get; } = Assembly.GetEntryAssembly()?
                                                         .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                                         .InformationalVersion ?? "获取版本号失败";
    }
}
