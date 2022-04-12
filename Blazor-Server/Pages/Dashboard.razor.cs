using AntDesign;
using System.Timers;
using ImageBed.Common;
using Microsoft.AspNetCore.Components;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using System.Diagnostics;
using AntDesign.Charts;

namespace ImageBed.Pages
{
    partial class Dashboard
    {
        private static System.Timers.Timer? t;

        int HostImageNums { get; set; } = 0;
        string DiskOccupancy { get; set; } = "MB";
        string RunningMemUsage { get; set; } = "MB";
        string SysRunningTime { get; set; } = "00:00:00";

        List<object> SysRecords = new();


        /// <summary>
        /// 初始化界面
        /// </summary>
        protected override void OnInitialized()
        {
            if (GlobalValues.appSetting?.Record?.RefreshRealTime ?? true)
            {
                InitTimer();
            }
            RefreshChart();
            RefreshDashboard(null, null);
        }


        /// <summary>
        /// 初始化定时器
        /// </summary>
        void InitTimer()
        {
            t = new System.Timers.Timer(1000);
            t.Elapsed += RefreshDashboard;
            t.AutoReset = true;
            t.Enabled = true;
        }


        /// <summary>
        /// 每隔 1s 刷新仪表盘
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        async void RefreshDashboard(object? source, ElapsedEventArgs? e)
        {
            using (var context = new OurDbContext())
            {
                var sqlImageData = new SQLImageData(context);
                List<ImageEntity> images = await sqlImageData.GetAsync();

                // 获取托管图片总数
                HostImageNums = images.Count;

                // 获取托管图片所占磁盘存储总量
                double diskOccupancyKB = 0;
                images.ForEach(image =>
                {
                    diskOccupancyKB += UnitNameGenerator.ParseFileSize(image.Size ?? "0B");
                });
                DiskOccupancy = $"{diskOccupancyKB / 1024.0:f3} MB";

                // 获取该进程占用的运行内存
                Process mainProcess = Process.GetCurrentProcess();
                mainProcess.Refresh();
                RunningMemUsage = $"{mainProcess.WorkingSet64 / (1024 * 1024)} MB";

                // 获取应用运行时长
                TimeSpan SysRunningTimeSpan = DateTime.Now - mainProcess.StartTime;
                SysRunningTime = $"{(int)SysRunningTimeSpan.TotalHours}:{((int)SysRunningTimeSpan.TotalMinutes) % 60}:{((int)SysRunningTimeSpan.TotalSeconds) % 60}";

                await InvokeAsync(new Action(() => { StateHasChanged(); }));
            }
        }


        /// <summary>
        /// 刷新资源统计表格
        /// </summary>
        async void RefreshChart()
        {
            using (var context = new OurDbContext())
            {
                var sqlRecordData = new SQLRecordData(context);
                SysRecords.Clear();
                foreach (var record in await sqlRecordData.GetAsync())
                {
                    SysRecords.Add(new
                    {
                        date = record.Date,
                        type = "图片上传数量",
                        value = record.UploadImageNum
                    });

                    SysRecords.Add(new
                    {
                        date = record.Date,
                        type = "图片磁盘占用(MB)",
                        value = record.UploadImageSize
                    });

                    SysRecords.Add(new
                    {
                        date = record.Date,
                        type = "图片请求次数",
                        value = record.RequestNum
                    });
                }
            }
        }


        /// <summary>
        /// 释放页面资源
        /// </summary>
        public void Dispose()
        {
            if(t != null)
            {
                t?.Stop();
                t?.Dispose();
            }
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// 资源统计表格相关属性
        /// </summary>
        LineConfig SysRecordConfig = new()
        {
            Padding = "auto",
            XField = "date",
            YField = "value",
            YAxis = new ValueAxis
            {
                Label = new BaseAxisLabel()
            },
            Legend = new Legend
            {
                Position = "right-top"
            },
            SeriesField = "type",
            Color = new string[] { "#1979C9", "#D62A0D", "#FAA219" },
            Responsive = true,
            Smooth = true
        };
    }
}
