using MudBlazor;
using North.Common;
using North.Core.Common;

namespace North.Pages
{
    partial class Plugins
    {
        private string SearchPluginName { get; set; } = string.Empty;
        private bool IsSearching { get; set; } = true;
        private PluginSetting PluginSetting { get; set; } = GlobalValues.AppSettings.Plugin;
        private List<Plugin> ShowPlugins { get; set; } = new List<Plugin>(1024);


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                GlobalValues.AppSettings.Plugin.Plugins.ForEach(plugin =>
                {
                    ShowPlugins.Add(plugin);
                });

                await InvokeAsync(() =>
                {
                    IsSearching = false;
                    StateHasChanged();
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }


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
                    IsSearching = true;
                    StateHasChanged();
                });

                ShowPlugins.Clear();
                if (string.IsNullOrEmpty(SearchPluginName))
                {
                    PluginSetting.Plugins.ForEach(plugin =>
                    {
                        ShowPlugins.Add(plugin);
                    });
                }
                else
                {
                    var localSearchedPlugins = PluginSetting.Plugins.Where(plugin => plugin.Id.Contains(SearchPluginName));
                    foreach(var metadata in await GlobalValues.NugetEngine.GetPackagesAsync(SearchPluginName))
                    {
                        ShowPlugins.Add(localSearchedPlugins.FirstOrDefault(plugin => plugin.Id == metadata.Identity.Id) ?? new Plugin(metadata, PluginState.UnInstall));
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to search plugin {SearchPluginName}", e);
                _snackbar.Add("搜索失败，服务器内部错误", Severity.Error);
            }
            finally
            {
                IsSearching = false;
            }
        }


        /// <summary>
        /// 安装插件
        /// </summary>
        /// <param name="plugin">待安装的插件</param>
        private async Task InstallPlugin(Plugin plugin)
        {
            try
            {
                // 下载插件包
                var pluginInstallDir = Path.Combine(PluginSetting.InstallDir, plugin.Id);
                var pluginPackage = await GlobalValues.NugetEngine.GetPackageAsync(plugin.Id, plugin.Version.ToNormalizedString());
                if(pluginPackage is not null)
                {
                    await GlobalValues.NugetEngine.InstallAsync(pluginPackage, pluginInstallDir, (packageId, extractDir) =>
                    {
                        var directories = Directory.GetDirectories(Path.Combine(extractDir, "lib"));
                        foreach (var directory in directories)
                        {
                            if (directory.Contains("net6.0") || directory.Contains("netstandard"))
                            {
                                var targetDllPath = Path.Combine(directory, $"{packageId}.dll");
                                if (File.Exists(targetDllPath))
                                {
                                    File.Copy(targetDllPath, Path.Combine(pluginInstallDir, $"{packageId}.dll")); return;
                                }
                            }
                        }
                    }, f => (f.Framework == ".NETCoreApp") && (f.Version.Major == 6));
                }
                plugin.State = PluginState.Enable;

                // TODO 写入插件类别 PluginSetting.Categories

                // 更新设置
                PluginSetting.Plugins.Add(plugin);
                GlobalValues.AppSettings.Save();

                _snackbar.Add($"{plugin.Id} 安装成功", Severity.Success);
            }
            catch(Exception e)
            {
                _logger.Error($"Failed to install plugin {plugin.Id}", e);
                _snackbar.Add("插件安装失败，服务器内部错误", Severity.Error);
            }
        }


        /// <summary>
        /// 更新插件
        /// </summary>
        /// <param name="plugin">待更新的插件</param>
        private async Task UpdatePlugin(Plugin plugin)
        {
            try
            {
                var package = await GlobalValues.NugetEngine.GetPackageAsync(plugin.Id, plugin.Version.ToNormalizedString());
                var latestVersion = (await package.GetVersionsAsync()).LastOrDefault();
                if (plugin.Version < latestVersion?.Version)
                {
                    var updatePlugin = await _dialog.ShowMessageBox("插件更新",
                                                                    $"检测到新版本 {latestVersion.Version.ToNormalizedString()}，现在更新?",
                                                                    "确 定",
                                                                    "取 消");
                    if(updatePlugin is true)
                    {
                        // TODO 更新插件
                    }
                }
                else
                {
                    _snackbar.Add("当前已是最新版本", Severity.Success);
                }

            }
            catch(Exception e)
            {
                _logger.Error($"Failed to check update {plugin.Id}", e);
                _snackbar.Add("检查更新失败，服务器内部错误", Severity.Error);
            }
        }


        /// <summary>
        /// 启用插件
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        private void EnablePlugin(Plugin plugin)
        {
            try
            {
                // 更改插件状态
                plugin.State = PluginState.Enable;

                // 保存设置
                PluginSetting.Plugins.First(p => p.Equals(plugin)).State = PluginState.Enable;
                GlobalValues.AppSettings.Save();

                _snackbar.Add($"已启用插件 {plugin.Id}", Severity.Success);
            }
            catch(Exception e)
            {
                _logger.Error($"Failed to enable plugin {plugin.Id}", e);
                _snackbar.Add("插件启动失败，服务器内部错误", Severity.Error);
            }
        }


        /// <summary>
        /// 禁用插件
        /// </summary>
        /// <returns></returns>
        private void DisablePlugin(Plugin plugin)
        {
            try
            {
                // 更改插件状态
                plugin.State = PluginState.Disable;

                // 保存设置
                PluginSetting.Plugins.First(p => p.Equals(plugin)).State = PluginState.Disable;
                GlobalValues.AppSettings.Save();

                _snackbar.Add($"已禁用插件 {plugin.Id}", Severity.Success);
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to disable plugin {plugin.Id}", e);
                _snackbar.Add("插件禁用失败，服务器内部错误", Severity.Error);
            }
        }


        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <param name="plugin">待卸载的插件</param>
        private void UnInstallPlugin(Plugin plugin)
        {
            try
            {
                plugin.State = PluginState.UnInstall;

                // 删除本地文件
                var pluginInstallDir = Path.Combine(PluginSetting.InstallDir, plugin.Id);
                if (Directory.Exists(pluginInstallDir))
                {
                    Directory.Delete(pluginInstallDir, true);
                }

                // 保存设置
                PluginSetting.Plugins.Remove(plugin);
                GlobalValues.AppSettings.Save();

                _snackbar.Add("卸载成功", Severity.Success);
            }
            catch(Exception e)
            {
                _logger.Error($"Failed to uninstall plugin {plugin.Id}", e);
                _snackbar.Add("插件卸载失败，服务器内部错误", Severity.Error);
            }
        }
    }
}