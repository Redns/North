using Masuit.Tools.Html;
using MudBlazor;
using North.Common;
using North.Core.Common;

namespace North.Pages.Settings
{
    partial class Appearance
    {
        private bool SaveRunning { get; set; } = false;
        private bool RestoreRunning { get; set; } = false;
        private AppearanceSetting AppearanceSettings { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            AppearanceSettings = _appSetting.Appearance.Clone();
        }


        /// <summary>
        /// 保存设置
        /// </summary>
        private async void SaveSettings()
        {
            try
            {
                SaveRunning = true;
                await Task.Delay(400);

                // 防 XSS 处理
                AppearanceSettings.Footer = AppearanceSettings.Footer.HtmlSantinizerStandard();
                
                // 修改全局设置并保存至本地
                _appSetting.Appearance = AppearanceSettings.Clone();
                _appSetting.Save();
                _snackbar.Add("保存成功", Severity.Success);

                // 强制重新刷新该页面以更新网站样式
                _navigationManager.NavigateTo(_navigationManager.Uri, true);
            }
            catch(Exception e)
            {
                AppearanceSettings = _appSetting.Appearance.Clone();
                _snackbar.Add("保存失败，已还原设置", Severity.Error);
                _logger.Error("Failed to save appearance settings", e);
            }
            finally
            {
                await InvokeAsync(() =>
                {
                    SaveRunning = false;
                    StateHasChanged();
                });
            }
        }


        /// <summary>
        /// 还原设置
        /// </summary>
        private async void RestoreSettings()
        {
            try
            {
                RestoreRunning = true;
                await Task.Delay(500);
                AppearanceSettings = _appSetting.Appearance.Clone();
                _snackbar.Add("设置还原成功", Severity.Success);
            }
            catch(Exception e)
            {
                _snackbar.Add("设置还原失败", Severity.Error);
                _logger.Error("Failed to restore settings", e);
            }
            finally
            {
                await InvokeAsync(() =>
                {
                    RestoreRunning = false;
                    StateHasChanged();
                });
            }
        }
    }
}
