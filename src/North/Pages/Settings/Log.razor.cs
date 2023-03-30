using Microsoft.JSInterop;
using MudBlazor;
using North.Common;
using North.Core.Services.Logger;

namespace North.Pages.Settings
{
    partial class Log
    {
        public bool SaveRunning { get; set; } = false;
        public bool RestoreRunning { get; set; } = false;
        public LogSetting LogSetting { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            LogSetting = _appSetting.Log.Clone();
        }


        /// <summary>
        /// 下载日志文件
        /// </summary>
        /// <returns></returns>
        public async Task DownloadLogfile()
        {
            try
            {
                using var logReadStream = File.OpenRead(LogSetting.Output);
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
                if(LogSetting.Levels.Min > LogSetting.Levels.Max)
                {
                    _snackbar.Add("日志输出等级不正确", Severity.Error);
                }
                else
                {
                    await Task.Delay(500);
                    _appSetting.Log = LogSetting.Clone();
                    _appSetting.Save();
                    _logger.ConfigLoggers(LogSetting);
                    _snackbar.Add("保存成功", Severity.Success);
                }
            }
            catch (Exception e)
            {
                LogSetting = _appSetting.Log.Clone();
                _snackbar.Add("保存失败，已还原设置", Severity.Error);
                _logger.Error("Failed to save log settings", e);
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
                LogSetting = _appSetting.Log.Clone();
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
