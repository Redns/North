using MudBlazor;
using North.Common;
using North.Core.Common;

namespace North.Pages.Settings
{
    partial class Appearance
    {
        private bool SaveRunning { get; set; } = false;
        private bool RestoreRunning { get; set; } = false;
        private AppearanceSetting AppearanceSettings { get; set; } = GlobalValues.AppSettings.Appearance.Clone();


        /// <summary>
        /// 保存设置
        /// </summary>
        private async void SaveSettings()
        {
            try
            {
                SaveRunning = true;
                await Task.Delay(400);
                GlobalValues.AppSettings.Appearance = AppearanceSettings.Clone();
                GlobalValues.AppSettings.Save();
                _snackbar.Add("保存成功", Severity.Success);
                _navigationManager.NavigateTo(_navigationManager.Uri, true);
            }
            catch(Exception e)
            {
                AppearanceSettings = GlobalValues.AppSettings.Appearance.Clone();
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
                AppearanceSettings = GlobalValues.AppSettings.Appearance.Clone();
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
