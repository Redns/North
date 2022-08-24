using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Timer = System.Timers.Timer;

namespace North.Core.Helper
{
    /// <summary>
    /// 应用监测辅助类，收集应用程序 CPU、内存、磁盘等资源占用
    /// </summary>
    public static class AppMonitorHelper
    {
        private static Timer? _dataRefreshTimer;
        private static Process? _process;
        private static DateTime _recordTime = DateTime.UtcNow;
        private static TimeSpan _totalProcessorTime = TimeSpan.Zero;

        /// <summary>
        /// 进程 CPU 占用百分比
        /// </summary>
        private static double _cpuUsage;
        public static double CpuUsage
        {
            get
            {
                if(_dataRefreshTimer is null)
                {
                    DataRefreshTimerInit();
                }
                return _cpuUsage * 100;
            }
        }

        /// <summary>
        /// 初始化数据刷新定时器
        /// </summary>
        /// <param name="refreshInterval">数据更新间隔（ms）</param>
        private static void DataRefreshTimerInit(int refreshInterval = 10)
        {
            _dataRefreshTimer = new Timer(refreshInterval)
            {
                AutoReset = true,
                Enabled = true,
            };
            _dataRefreshTimer.Elapsed += (sender, args) =>
            {
                _process ??= Process.GetCurrentProcess();
                _cpuUsage = (_process.TotalProcessorTime - _totalProcessorTime) / (Environment.ProcessorCount * (DateTime.UtcNow - _recordTime));
                _recordTime = DateTime.UtcNow;
                _totalProcessorTime = _process.TotalProcessorTime;
            };
            _dataRefreshTimer.Start();
        }
    }
}
