using AntDesign;
using System.Timers;
using ImageBed.Common;
using Microsoft.AspNetCore.Components;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using System.Diagnostics;
using AntDesign.Charts;
using Microsoft.JSInterop;

namespace ImageBed.Pages
{
    partial class Dashboard : IDisposable
    {
        bool spinning = true;                                   // 页面加载标志
        bool showChart = false;

        Record? recordConfig = GlobalValues.appSetting.Record;  // 仪表盘设置
        
        Process process = Process.GetCurrentProcess();          // 当前进程
        System.Timers.Timer? t;                                 // 资源刷新定时器

        List<object>? SysRecords { get; set; } = new();         // 统计数据
        List<ImageEntity>? Images { get; set; } = new();        // 图片信息
        int HostImageNums { get; set; } = 0;                    // 托管图片总数
        string DiskOccupancy { get; set; } = "MB";              // 磁盘存储占用
        string RunningMemUsage { get; set; } = "MB";            // 运行内存占用
        string SysRunningTime { get; set; } = "00:00:00";       // 系统运行时间


        /// <summary>
        /// 资源统计表格相关属性
        /// </summary>
        LineConfig? SysRecordConfig = new()
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

        // 卡片视图时相关参数
        ListGridType? grid = new()
        {
            Gutter = 16,    // 栅格间距
            Xs = 1,         // < 576px 展示的列数
            Lg = 2,         // ≥ 992px 展示的列数
            Xl = 4,         // ≥ 1200px 展示的列数
            Xxl = 4,        // ≥ 1600px 展示的列数 
        };
        SysRunningInfoCard[]? sysRunningInfoCards = new[]
        {
            new SysRunningInfoCard("托管图片总数", "picture", "0 张"),
            new SysRunningInfoCard("磁盘存储占用", "database", "0 MB"),
            new SysRunningInfoCard("运行内存占用", "fund-projection-screen", "0 MB"),
            new SysRunningInfoCard("系统运行时长", "dashboard", "00:00:00")
        };


        /// <summary>
        /// 结束页面渲染
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                spinning = false;

                if ((recordConfig != null) && recordConfig.RefreshRealTime)
                {
                    InitTimer();
                }

                // 刷新界面
                if(await JS.InvokeAsync<double>("GetScreenWidth") > 500)
                {
                    showChart = true;
                    RefreshChart();
                }
                RefreshDashboard(null, null);

                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
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
        /// 刷新仪表盘
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        async void RefreshDashboard(object? source, ElapsedEventArgs? e)
        {
            using(var context = new OurDbContext())
            {
                Images.Clear();
                Images = await new SQLImageData(context).GetAsync();

                // 获取托管图片总数
                HostImageNums = Images.Count;

                // 获取托管图片所占磁盘存储总量
                double diskOccupancyKB = 0;
                Images.ForEach(image =>
                {
                    diskOccupancyKB += UnitNameGenerator.ParseFileSize(image.Size ?? "0B");
                });
                DiskOccupancy = $"{diskOccupancyKB / 1024.0:f3} MB";
            }

            // 获取该进程占用的运行内存
            process.Refresh();
            RunningMemUsage = $"{process.WorkingSet64 / (1024 * 1024)} MB";

            // 获取应用运行时长
            TimeSpan SysRunningTimeSpan = DateTime.Now - process.StartTime;
            SysRunningTime = $"{(int)SysRunningTimeSpan.TotalHours}:{((int)SysRunningTimeSpan.TotalMinutes) % 60}:{((int)SysRunningTimeSpan.TotalSeconds) % 60}";

            sysRunningInfoCards[0].Description = HostImageNums.ToString();
            sysRunningInfoCards[1].Description = DiskOccupancy;
            sysRunningInfoCards[2].Description = RunningMemUsage;
            sysRunningInfoCards[3].Description = SysRunningTime;

            await InvokeAsync(new Action(() => { StateHasChanged(); }));
        }


        /// <summary>
        /// 刷新资源统计表格
        /// </summary>
        async void RefreshChart()
        {
            SysRecords.Clear();
            using (var context = new OurDbContext())
            {
                foreach (var record in await new SQLRecordData(context).GetAsync())
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
                t = null;
            }

            SysRecords.Clear();
            SysRecords = null;

            Images.Clear();
            Images = null;

            grid = null;

            GC.SuppressFinalize(this);
        }
    }

    public class SysRunningInfoCard
    {
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public SysRunningInfoCard(string title, string icon, string description)
        {
            Title = title;
            Icon = icon;
            Description = description;
        }
    }
}
