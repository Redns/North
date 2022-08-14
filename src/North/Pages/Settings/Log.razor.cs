using Microsoft.JSInterop;
using MudBlazor;
using North.Common;
using static MudBlazor.CategoryTypes;

namespace North.Pages.Settings
{
    partial class Log
    {
        public AppSetting AppSetting { get; set; }
        public bool PageLoading { get; set; } = true;
        public bool SaveRunning { get; set; } = false;
        public bool CencelSaveRunning { get; set; } = false;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                AppSetting = AppSetting.Load();
                await InvokeAsync(() =>
                {
                    PageLoading = false;
                    StateHasChanged();
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        /// <summary>
        /// 下载日志文件
        /// </summary>
        /// <returns></returns>
        public async Task DownloadLogfile()
        {
            try
            {
                using var logReadStream = File.OpenRead(AppSetting.Log.Output);
                using var logReadStreamRef = new DotNetStreamReference(logReadStream);

                var logDownloadUrl = await JS.InvokeAsync<string>("upload", logReadStreamRef, "text/plain");
                await JS.InvokeVoidAsync("download", "North.log", logDownloadUrl);
                await JS.InvokeVoidAsync("destroy", logDownloadUrl);
            }
            catch(Exception e)
            {
                _logger.Error("Failed to doanload log file", e);
                _snackbar.Add("日志下载失败，服务器内部错误", Severity.Error);
            }
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

                if(AppSetting.Log.Level.Min > AppSetting.Log.Level.Max)
                {
                    _snackbar.Add("日志输出等级不正确", Severity.Error);
                }
                else
                {
                    await Task.Delay(500);

                    // 保存设置
                    AppSetting.Save();
                    GlobalValues.AppSettings = AppSetting.Clone();

                    _logger.ConfigLoggers(AppSetting.Log);
                    _snackbar.Add("保存成功", Severity.Success);
                }
            }
            catch (Exception e)
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
