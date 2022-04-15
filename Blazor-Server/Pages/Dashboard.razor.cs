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
    partial class Dashboard : IDisposable
    {
        bool spinning;                                  // 页面加载标志

        Record? recordConfig;                           // 仪表盘设置
        
        Process? process;                               // 当前进程
        System.Timers.Timer? t;                         // 资源刷新定时器

        List<object>? SysRecords { get; set; }          // 统计数据
        List<ImageEntity>? Images { get; set; }         // 图片信息
        int HostImageNums { get; set; }                 // 托管图片总数
        string DiskOccupancy { get; set; }              // 磁盘存储占用
        string RunningMemUsage { get; set; }            // 运行内存占用
        string SysRunningTime { get; set; }             // 系统运行时间

        
        /// <summary>
        /// 初始化界面
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            spinning = true;

            HostImageNums = 0;
            DiskOccupancy = "MB";
            RunningMemUsage = "MB";
            SysRunningTime = "00:00:00";

            SysRecords = new();
            Images = new();

            process = Process.GetCurrentProcess();
            recordConfig = GlobalValues.appSetting.Record;
            if (recordConfig.RefreshRealTime)
            {
                InitTimer();
            }

            // 刷新界面
            RefreshChart();
            RefreshDashboard(null, null);

            await base.OnInitializedAsync();
        }


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
                await InvokeAsync(() => { StateHasChanged(); });
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

            await InvokeAsync(new Action(() => { StateHasChanged(); }));
        }


        /// <summary>
        /// 刷新资源统计表格
        /// </summary>
        async void RefreshChart()
        {
            SysRecords.Clear();
            using(var context = new OurDbContext())
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

            recordConfig = null;
            process = null;
            SysRecords = null;
            Images = null;


            GC.Collect();
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
