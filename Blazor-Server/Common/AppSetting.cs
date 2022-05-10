using ImageBed.Pages;
using Newtonsoft.Json;
using static ImageBed.Common.FileHelper;

namespace ImageBed.Common
{
    public class AppSetting
    {
        public Views Views { get; set; }        // 页面视图
        public Data Data { get; set; }          // 数据存储
        public Notify Notify { get; set; }      // 通知
        public Update Update { get; set; }      // 应用更新

        public AppSetting(Views views, Data data, Notify notify, Update update)
        {
            Views = views;
            Data = data;
            Notify = notify;
            Update = update;
        }


        /// <summary>
        /// 解析配置文件
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <returns></returns>
        public static AppSetting? Parse(string path = "appsettings.json")
        {
            try
            {
                return JsonConvert.DeserializeObject<AppSetting>(File.ReadAllText(path));
            }
            catch(Exception ex)
            {
                GlobalValues.Logger.Error($"Cannot load settings file, {ex.Message}");
            }
            return null;
        }


        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <returns></returns>
        public bool Save(string path = "appsettings.json")
        {
            try
            {
                File.WriteAllText(path, ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 序列化设置
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Views
    {
        public Dashboard Dashboard { get; set; }        // 仪表盘界面
        public Pics Pics { get; set; }                  // 图库界面
        public Footer Footer { get; set; }              // 页脚

        public Views(Dashboard dashboard, Pics pics, Footer footer)
        {
            Dashboard = dashboard;
            Pics = pics;
            Footer = footer;
        }
    }

    public class Dashboard
    {
        public Refresh Refresh { get; set; }            // 仪表盘刷新设置

        public Dashboard(Refresh refresh)
        {
            Refresh = refresh;
        }
    }

    public class Refresh
    {
        public bool RealTime { get; set; }              // 是否实时刷新

        public Refresh(bool realTime)
        {
            RealTime = realTime;
        }
    }

    public class Pics
    {
        public ViewFormat ViewFormat { get; set; }      // 页面视图(表格、卡片)

        public Pics(ViewFormat viewFormat)
        {
            ViewFormat = viewFormat;
        }
    }

    public class Footer
    {
        public string Content { get; set; }             // 页脚内容
        public string RegisterNumber { get; set; }      // 公安联网备案号

        public Footer(string content, string registerNumber)
        {
            Content = content;
            RegisterNumber = registerNumber;
        }
    }

    public class Data
    {
        public Image Image { get; set; }                // 图片相关设置

        public Data(Image image)
        {
            Image = image;
        }
    }

    public class Notify
    {
        public Email Email { get; set; }                // 通知邮箱设置
        public Condition Condition { get; set; }        // 通知触发条件

        public Notify(Email email, Condition condition)
        {
            Email = email;
            Condition = condition;
        }
    }


    public class Email
    {
        public string From { get; set; }                // 发送账号
        public string To { get; set; }                  // 接收账号
        public string Code { get; set; }                // 发送方邮箱授权码

        public Email(string from, string to, string code)
        {
            From = from;
            To = to;
            Code = code;
        }
    }


    public class Condition
    {
        public double OverDiskOccupy { get; set; }      // 磁盘占用超过此限制时触发
        public double OverMemory { get; set; }          // 运行内存占用超过此限制时触发
        public double OverNum { get; set; }             // 托管图片总数超过此限制时触发

        public Condition(double overDiskOccupy, double overMemory, double overNum)
        {
            OverDiskOccupy = overDiskOccupy;
            OverMemory = overMemory;
            OverNum = overNum;
        }
    }

    public class Update
    {
        public bool AutoUpdate { get; set; }            // 自动更新
        public UpdateMode Mode { get; set; }            // 更新模式
        public string CheckUrl { get; set; }            // 更新检查链接
        public string DownloadUrl { get; set; }         // 更新包下载链接

        public Update(bool autoUpdate, UpdateMode mode, string checkUrl, string downloadUrl)
        {
            AutoUpdate = autoUpdate;
            Mode = mode;
            CheckUrl = checkUrl;
            DownloadUrl = downloadUrl;
        }
    }


    public enum UpdateMode
    {
        INCREMENT = 0,          // 增量更新
        FULL                    // 全量更新
    }

    public class Image
    {
        public string RootPath { get; set; }            // 图片存储根路径
        public RenameFormat RenameFormat { get; set; }  // 重命名格式
        public UrlFormat UrlFormat { get; set; }        // Url格式

        public Image(string rootPath, RenameFormat renameFormat, UrlFormat urlFormat)
        {
            RootPath = rootPath;
            RenameFormat = renameFormat;
            UrlFormat = urlFormat;
        }
    }
}
