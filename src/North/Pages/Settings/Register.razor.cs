using MudBlazor;
using North.Common;

namespace North.Pages.Settings
{
    partial class Register
    {
        public bool SaveRunning { get; set; } = false;
        public bool CencelSaveRunning { get; set; } = false;
        public bool PageLoading { get; set; } = true;
        public AppSetting AppSetting { get; set; }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                AppSetting = GlobalValues.AppSettings.Clone();
                await InvokeAsync(() =>
                {
                    PageLoading = false;
                    StateHasChanged();
                });
            }
            await base.OnAfterRenderAsync(firstRender);
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

                AppSetting.Save();
                GlobalValues.AppSettings = AppSetting.Clone();

                _snackbar.Add("保存成功", Severity.Success);
            }
            catch(Exception e)
            {
                GlobalValues.AppSettings = AppSetting.Load();
                AppSetting = GlobalValues.AppSettings.Clone();

                _snackbar.Add("保存失败", Severity.Error);
                _logger.Error("Failed to save settings", e);
            }
            finally
            {
                SaveRunning = false;
            }
        }


        /// <summary>
        /// 还原设置
        /// </summary>
        /// <returns></returns>
        public async Task CancelSaveSettings()
        {
            try
            {
                CencelSaveRunning = true;
                await Task.Delay(500);

                AppSetting = GlobalValues.AppSettings.Clone();

                _snackbar.Add("已还原设置", Severity.Success);
            }
            catch (Exception e)
            {
                _snackbar.Add("设置还原失败", Severity.Error);
                _logger.Error("Failed to restore settings", e);
            }
            finally
            {
                CencelSaveRunning = false;
            }
        }
    }
}
