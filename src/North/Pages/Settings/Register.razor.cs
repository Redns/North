using MudBlazor;
using North.Common;
using North.Core.Common;

namespace North.Pages.Settings
{
    partial class Register
    {
        public bool SaveRunning { get; set; } = false;
        public bool RestoreRunning { get; set; } = false;
        public RegisterSetting RegisterSetting { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            RegisterSetting = _appSetting.Register.Clone();
        }


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
                _appSetting.Register = RegisterSetting.Clone();
                _appSetting.Save();
                _snackbar.Add("保存成功", Severity.Success);
            }
            catch(Exception e)
            {
                RegisterSetting = _appSetting.Register.Clone();
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
                RegisterSetting = _appSetting.Register.Clone();
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
