using MudBlazor;
using North.Common;
using North.Core.Common;

namespace North.Pages.Settings
{
    partial class Register
    {
        public bool SaveRunning { get; set; } = false;
        public bool RestoreRunning { get; set; } = false;
        public RegisterSetting RegisterSetting { get; set; } = GlobalValues.AppSettings.Register.Clone();


        /// <summary>
        /// 保存设置
        /// </summary>
        /// <returns></returns>
        public async Task SaveSettings()
        {
            try
            {
                SaveRunning = true;
                await Task.Delay(500);
                GlobalValues.AppSettings.Register = RegisterSetting.Clone();
                GlobalValues.AppSettings.Save();
                _snackbar.Add("保存成功", Severity.Success);
            }
            catch(Exception e)
            {
                RegisterSetting = GlobalValues.AppSettings.Register.Clone();
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
        /// <returns></returns>
        public async Task RestoreSettings()
        {
            try
            {
                RestoreRunning = true;
                await Task.Delay(500);
                RegisterSetting = GlobalValues.AppSettings.Register.Clone();
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
