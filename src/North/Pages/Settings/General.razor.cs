using MudBlazor;
using North.Common;

namespace North.Pages.Settings
{
    partial class General
    {
        private bool SaveRunning { get; set; } = false;
        private bool RestoreRunning { get; set; } = false;
        private GeneralSetting GeneralSetting { get; set; } = GlobalValues.AppSettings.General.Clone();


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
                GlobalValues.AppSettings.General = GeneralSetting.Clone();
                GlobalValues.AppSettings.Save();
                _snackbar.Add("保存成功", Severity.Success);
            }
            catch (Exception e)
            {
                GeneralSetting = GlobalValues.AppSettings.General.Clone();
                _snackbar.Add("保存失败，已还原设置", Severity.Error);
                _logger.Error("Failed to save general settings", e);
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
                GeneralSetting = GlobalValues.AppSettings.General.Clone();
                _snackbar.Add("已还原设置", Severity.Success);
            }
            catch (Exception e)
            {
                _snackbar.Add("设置还原失败", Severity.Error);
                _logger.Error("Failed to restore settings", e);
            }
            finally
            {
                RestoreRunning = false;
            }
        }
    }
}
