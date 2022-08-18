using MudBlazor;
using North.Common;

namespace North.Pages.Settings
{
    partial class Auth
    {
        private bool SaveRunning { get; set; } = false;
        private bool RestoreRunning { get; set; } = false;
        private AuthSetting AuthSetting { get; set; } = GlobalValues.AppSettings.Auth.Clone();


        /// <summary>
        /// 保存设置
        /// </summary>
        /// <returns></returns>
        private async Task SaveSettings()
        {
            try
            {
                SaveRunning = true;
                await Task.Delay(500);
                GlobalValues.AppSettings.Auth = AuthSetting.Clone();
                GlobalValues.AppSettings.Save();
                _snackbar.Add("保存成功", Severity.Success);
            }
            catch (Exception e)
            {
                AuthSetting = GlobalValues.AppSettings.Auth.Clone();
                _snackbar.Add("保存失败，已还原设置", Severity.Error);
                _logger.Error("Failed to save auth settings", e);
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
        /// <returns></returns>
        private async Task RestoreSettings()
        {
            try
            {
                RestoreRunning = true;
                await Task.Delay(500);
                AuthSetting = GlobalValues.AppSettings.Auth.Clone();
                _snackbar.Add("已还原设置", Severity.Success);
            }
            catch (Exception e)
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
