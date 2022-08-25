using MudBlazor;
using North.Common;
using NuGet.Protocol.Core.Types;

namespace North.Pages
{
    partial class Plugins
    {
        private string PackageName { get; set; } = string.Empty;
        private bool PackageSearching { get; set; } = false;
        private PluginSetting PluginSetting { get; set; } = GlobalValues.AppSettings.Plugin;
        private IPackageSearchMetadata[] Packages { get; set; } = GlobalValues.AppSettings.Plugin.Plugins;


        /// <summary>
        /// 搜索插件
        /// </summary>
        /// <returns></returns>
        private async Task SearchPluginsAsync()
        {
            try
            {
                await InvokeAsync(() =>
                {
                    PackageSearching = true;
                    StateHasChanged();
                });
                Packages = string.IsNullOrEmpty(PackageName) ? PluginSetting.Plugins : (await GlobalValues.NugetEngine.GetPackagesAsync(PackageName)).ToArray();
            }
            catch (Exception e)
            {
                _logger.Error("Search package error", e);
                _snackbar.Add("搜索失败，服务器内部错误", Severity.Error);
            }
            finally
            {
                PackageSearching = false;
            }
        }


        private void InstallPlugin(IPackageSearchMetadata plugin)
        {
            PluginSetting.Plugins = PluginSetting.Plugins.Append(plugin).ToArray();
        }


        private void UnInstallPlugin(IPackageSearchMetadata plugin)
        {
            PluginSetting.Plugins = PluginSetting.Plugins.Where(p => p.Identity.Id != plugin.Identity.Id).ToArray();
        }
    }
}
