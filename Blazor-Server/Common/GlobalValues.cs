using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using System.Diagnostics;
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
            // 检测时间是否为设置的时间段
            DateTime today = DateTime.Now;
            int refreshStartTime = appSetting.Record.RefreshStartTime;
            if((today.Hour >= refreshStartTime) && (today.Hour <= refreshStartTime + 6))
            {
                Logger.Info("Refreshing sys record");
                using (var context = new OurDbContext())
                {
                    int hostImageNumTotal = 0;
                    int hostImageRequestTotal = 0;
                    double hostImageDiskOccupyTotal = 0;


                    // 收集数据库中所有图片的数量、磁盘占用(MB)、请求次数
                    var sqlImageData = new SQLImageData(context);
                    List<ImageEntity> images = await sqlImageData.GetAsync();

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

                    string dateYestoday = today.Subtract(new TimeSpan(TimeSpan.TicksPerDay))
                                                .ToShortDateString()
                                                .Split(" ")[0]
                                                .Replace("-", "/");

                    List<RecordEntity> records = await sqlRecordData.GetAsync();
                    foreach (var record in records)
                    {
                        if(record.Date != dateYestoday)
                        {
                            hostImageNumRecord += record.UploadImageNum;
                            hostImageRequestRecord += record.RequestNum;
                            hostImageDiskOccupyRecord += record.UploadImageSize;
                        }
                    }

                    _ = sqlRecordData.AddAsync(new RecordEntity
                    {
                        Date = dateYestoday,
                        UploadImageNum = hostImageNumTotal - hostImageNumRecord,
                        UploadImageSize = (int)hostImageDiskOccupyTotal - hostImageDiskOccupyRecord,
                        RequestNum = hostImageRequestTotal - hostImageRequestRecord
                    });


                    /// <summary>
                    /// 检查是否需要发送邮件
                    /// </summary>
                    Email emailConfig = appSetting.Notify.Email;
                    Condition emailSendCondition = appSetting.Notify.Condition;
                    if (emailSendCondition.SysRefresh)
                    {
                        _ = MailHelper.PostEmails(new MailEntity
                        {
                            FromPerson = emailConfig.From,
                            RecipientArry = emailConfig.To.Split(","),
                            MailTitle = "ImageBed 每日汇报",
                            MailBody = $"[系统总体数据如下]\n" + 
                                       $"托管图片总数 ------- {hostImageNumTotal} 张\n" +
                                       $"图片请求总数 ------- {hostImageRequestTotal} 次" +
                                       $"磁盘存储占用 ------- {hostImageDiskOccupyTotal} MB\n\n" +
                                       $"[系统昨日数据如下]\n" +
                                       $"上传图片数量 ------- {hostImageNumTotal - hostImageNumRecord} 张\n" +
                                       $"磁盘存储占用 ------- {hostImageDiskOccupyTotal - hostImageDiskOccupyRecord} MB\n" +
                                       $"图片请求次数 ------- {hostImageRequestTotal - hostImageRequestRecord} 次\n\n" +
                                       $"感谢您使用 ImageBed, 祝您生活愉快🎉",
                            Code = emailConfig.Code,
                            IsBodyHtml = false
                        });
                    }
                    if((emailSendCondition.OverDiskOccupy > 0) && (hostImageDiskOccupyTotal > emailSendCondition.OverDiskOccupy))
                    {
                        _ = MailHelper.PostEmails(new MailEntity
                        {
                            FromPerson = emailConfig.From,
                            RecipientArry = emailConfig.To.Split(","),
                            MailTitle = "⚠ ImageBed 磁盘占用超出限制",
                            MailBody = $"磁盘使用限制为 {emailSendCondition.OverDiskOccupy} MB, 当前已使用 {hostImageDiskOccupyTotal} MB\n\n" +
                                       $"感谢您使用 ImageBed, 祝您生活愉快🎉",
                            Code = emailConfig.Code,
                            IsBodyHtml = false
                        });
                    }

                    long memoryUsage = Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024);
                    if ((emailSendCondition.OverMemory > 0) && (memoryUsage > emailSendCondition.OverMemory))
                    {
                        _ = MailHelper.PostEmails(new MailEntity
                        {
                            FromPerson = emailConfig.From,
                            RecipientArry = emailConfig.To.Split(","),
                            MailTitle = "⚠ ImageBed 内存占用超出限制",
                            MailBody = $"程序运行内存限制为 {emailSendCondition.OverMemory} MB, 当前已占用 {memoryUsage} MB\n\n" +
                                       $"感谢您使用 ImageBed, 祝您生活愉快🎉",
                            Code = emailConfig.Code,
                            IsBodyHtml = false
                        });
                    }

                    if((emailSendCondition.OverNum > 0) && (hostImageNumTotal > emailSendCondition.OverNum))
                    {
                        _ = MailHelper.PostEmails(new MailEntity
                        {
                            FromPerson = emailConfig.From,
                            RecipientArry = emailConfig.To.Split(","),
                            MailTitle = "⚠ ImageBed 托管图片总数超出限制",
                            MailBody = $"托管图片总数限制为 {emailSendCondition.OverNum} 张, 当前已托管 {hostImageNumTotal} 张\n\n" +
                                       $"感谢您使用 ImageBed, 祝您生活愉快🎉",
                            Code = emailConfig.Code,
                            IsBodyHtml = false
                        });
                    }
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
