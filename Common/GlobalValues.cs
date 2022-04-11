using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using System.Timers;

namespace ImageBed.Common
{
    public class GlobalValues
    {
        // 全局设置
        public static AppSetting? appSetting { get; set; }

        // 资源数据记录刷新定时器(6h)
        public static System.Timers.Timer? SysRecordTimer { get; set; }

        // 日志记录器
        public static LoggerHelper Logger { get; set; } = new LoggerHelper();


        /// <summary>
        /// 初始化资源刷新定时器
        /// </summary>
        public static void InitSysRecordTimer()
        {
            Logger.Info("Start init SysRecordTimer");

            SysRecordTimer = new(6 * 60 * 60 * 1000);
            SysRecordTimer.Elapsed += RefreshSysRecord;
            SysRecordTimer.AutoReset = true;
            SysRecordTimer.Enabled = true;

            Logger.Info("Init SysRecordTimer finished");
        }


        /// <summary>
        /// 资源刷新事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static async void RefreshSysRecord(Object? source, ElapsedEventArgs? e)
        {
            // 检测时间是否为 00:00 ~ 06:00
            DateTime today = DateTime.Now;
            if((today.Hour >= 0) && (today.Hour <= 6))
            {
                Logger.Info("Refreshing sys record");
                using (var context = new OurDbContext())
                {
                    int hostImageNumTotal = 0;
                    int hostImageRequestTotal = 0;
                    double hostImageDiskOccupyTotal = 0;

                    // 收集数据库中所有图片的数量、磁盘占用(MB)、请求次数
                    var sqlImageData = new SQLImageData(context);
                    List<ImageEntity> images = await sqlImageData.Get();

                    hostImageNumTotal = images.Count;

                    foreach (var image in images)
                    {
                        hostImageRequestTotal += image.RequestNum;
                        hostImageDiskOccupyTotal += UnitNameGenerator.ParseFileSize(image.Size ?? "0MB") / 1024.0;
                    }

                    /// <summary>
                    /// 与前日数据对比，计算昨日数据
                    /// </summary>
                    var sqlRecordData = new SQLRecordData(context);
                    int hostImageNumRecord = 0;
                    int hostImageRequestRecord = 0;
                    int hostImageDiskOccupyRecord = 0;

                    List<RecordEntity> records = await sqlRecordData.Get();
                    foreach(var record in records)
                    {
                        hostImageNumRecord += record.UploadImageNum;
                        hostImageRequestRecord += record.RequestNum;
                        hostImageDiskOccupyRecord += record.UploadImageSize;
                    }

                    string dateYestoday = today.Subtract(new TimeSpan(TimeSpan.TicksPerDay))
                                                .ToShortDateString()
                                                .Split(" ")[0]
                                                .Replace("-", "/");
                    _ = sqlRecordData.Add(new RecordEntity
                    {
                        Date = dateYestoday,
                        UploadImageNum = hostImageNumTotal - hostImageNumRecord,
                        UploadImageSize = (int)hostImageDiskOccupyTotal - hostImageDiskOccupyRecord,
                        RequestNum = hostImageRequestTotal - hostImageRequestRecord
                    });
                }
                Logger.Info("Refresh finished");
            }
        }


        /// <summary>
        /// 停止资源记录定时器
        /// </summary>
        public static void StopSysRecordTimer()
        {
            SysRecordTimer?.Stop();
            SysRecordTimer?.Dispose();
            Logger.Info("The SysRecordTimer is stoped");
        }
    }
}
