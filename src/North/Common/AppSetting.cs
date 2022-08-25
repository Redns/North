using Newtonsoft.Json;
using North.Core.Entities;
using North.Pages.Settings;
using NuGet.Protocol.Core.Types;

namespace North.Common
{
    public class AppSetting
    {
        public GeneralSetting General { get; set; }
        public AppearanceSetting Appearance { get; set; }
        public RegisterSetting Register { get; set; }
        public NotifySetting Notify { get; set; }
        public AuthSetting Auth { get; set; }
        public LogSetting Log { get; set; }
        public PluginSetting Plugin { get; set; }

        public AppSetting(GeneralSetting general, AppearanceSetting appearance, RegisterSetting register, NotifySetting notify, AuthSetting auth, LogSetting log, PluginSetting plugin)
        {
            General = general;
            Appearance = appearance;
            Register = register;
            Notify = notify;
            Auth = auth;
            Log = log;
            Plugin = plugin;
        }


        /// <summary>
        /// 加载设置
        /// </summary>
        /// <param name="path">设置文件路径（默认 appsettings.json）</param>
        /// <returns></returns>
        public static AppSetting Load(string path = "appsettings.json")
        {
            return JsonConvert.DeserializeObject<AppSetting>(File.ReadAllText(path)) ?? throw new Exception($"Load {path} failed");
        }


        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path = "appsettings.json")
        {
            File.WriteAllText(path, ToString());
        }


        /// <summary>
        /// 复制应用设置（注意不能使用 record 和 struct）
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public AppSetting Clone()
        {
            // TODO 使用构造函数直接复制
            return JsonConvert.DeserializeObject<AppSetting>(ToString()) ?? throw new Exception("Clone AppSetting failed");
        }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }


    public class GeneralSetting
    {
        // TODO 数据库表不存在时自动创建
        // 增加测试连接按钮
        // 修改后弹窗提示是否迁移数据
        public DataBaseSetting DataBase { get; set; }

        public GeneralSetting(DataBaseSetting dataBase)
        {
            DataBase = dataBase;
        }

        public GeneralSetting Clone()
        {
            return new GeneralSetting(DataBase.Clone());
        }
    }


    /// <summary>
    /// 外观设置
    /// </summary>
    public class AppearanceSetting
    {
        /// <summary>
        /// 图床名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 侧边栏自动展开
        /// </summary>
        public bool NavAutoExpand { get; set; }

        /// <summary>
        /// 背景每日一图 API
        /// </summary>
        public string BackgroundUrl { get; set; }

        /// <summary>
        /// 页脚
        /// </summary>
        public string Footer { get; set; }

        public AppearanceSetting(string name, bool navAutoExpand, string backgroundUrl, string footer)
        {
            Name = name;
            NavAutoExpand = navAutoExpand;
            BackgroundUrl = backgroundUrl;
            Footer = footer;
        }

        public AppearanceSetting Clone()
        {
            return new AppearanceSetting(Name, NavAutoExpand, BackgroundUrl, Footer);
        }
    }


    public class StorageSetting
    {
        public DataBaseSetting DataBase { get; set; }

        public StorageSetting(DataBaseSetting dataBase)
        {
            DataBase = dataBase;
        }

        public StorageSetting Clone()
        {
            return new StorageSetting(DataBase.Clone());
        }
    }


    public class DataBaseSetting
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnStr { get; set; }

        public DataBaseSetting(string connStr)
        {
            ConnStr = connStr;
        }

        public DataBaseSetting Clone()
        {
            return new DataBaseSetting(ConnStr);
        }
    }


    public class RegisterSetting
    {
        /// <summary>
        /// 是否允许注册
        /// </summary>
        public bool AllowRegister { get; set; } = true;

        /// <summary>
        /// 头像最大尺寸（MB）
        /// </summary>
        public double MaxAvatarSize { get; set; }       
        
        /// <summary>
        /// 验证邮件有效期（ms）
        /// </summary>
        public long VerifyEmailValidTime { get; set; }

        /// <summary>
        /// 默认注册设置
        /// </summary>
        public RegisterSettingDefault Default { get; set; }

        public RegisterSetting(bool allowRegister, double maxAvatarSize, long verifyEmailValidTime, RegisterSettingDefault @default)
        {
            AllowRegister = allowRegister;
            MaxAvatarSize = maxAvatarSize;
            VerifyEmailValidTime = verifyEmailValidTime;
            Default = @default;
        }

        public RegisterSetting Clone()
        {
            return new RegisterSetting(AllowRegister, MaxAvatarSize, VerifyEmailValidTime, Default.Clone());
        }
    }


    /// <summary>
    /// 默认注册设置
    /// </summary>
    public class RegisterSettingDefault
    {
        public Permission Permission { get; set; }          // 用户权限
        public bool IsApiAvailable { get; set; }            // 是否启用 API
        public long MaxUploadNums { get; set; }            // 最大上传数量（张）
        public double MaxUploadCapacity { get; set; }        // 最大上传容量（MB）
        public long SingleMaxUploadNums { get; set; }      // 单次最大上传数量（张）
        public double SingleMaxUploadCapacity { get; set; }  // 单次最大上传容量（MB）

        public RegisterSettingDefault(Permission permission, bool isApiAvailable, long maxUploadNums, double maxUploadCapacity, long singleMaxUploadNums, double singleMaxUploadCapacity)
        {
            Permission = permission;
            IsApiAvailable = isApiAvailable;
            MaxUploadNums = maxUploadNums;
            MaxUploadCapacity = maxUploadCapacity;
            SingleMaxUploadNums = singleMaxUploadNums;
            SingleMaxUploadCapacity = singleMaxUploadCapacity;
        }

        public RegisterSettingDefault Clone()
        {
            return new RegisterSettingDefault(Permission, IsApiAvailable, MaxUploadNums, MaxUploadCapacity, SingleMaxUploadNums, SingleMaxUploadCapacity);
        }
    }


    public class NotifySetting
    {
        public EmailSetting Email { get; set; }

        public NotifySetting(EmailSetting email)
        {
            Email = email;
        }

        public NotifySetting Clone()
        {
            return new NotifySetting(Email.Clone());
        }
    }


    public class EmailSetting
    {
        /// <summary>
        /// 邮箱账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 授权码
        /// </summary>
        public string Code { get; set; }

        public EmailSetting(string account, string code)
        {
            Account = account;
            Code = code;
        }

        public EmailSetting Clone()
        {
            return new EmailSetting(Account, Code);
        }
    }


    public class AuthSetting
    {
        /// <summary>
        /// API 令牌有效期（ms）
        /// </summary>
        public long TokenValidTime { get; set; }

        /// <summary>
        /// 网页 Cookie 有效期（s）
        /// </summary>
        public long CookieValidTime { get; set; }

        public AuthSetting(long tokenValidTime, long cookieValidTime)
        {
            TokenValidTime = tokenValidTime;
            CookieValidTime = cookieValidTime;
        }

        public AuthSetting Clone()
        {
            return new AuthSetting(TokenValidTime, CookieValidTime);
        }
    }


    public class LogSetting
    {
        public string Output { get; set; }
        public Level Level { get; set; }
        public string Layout { get; set; }

        public LogSetting(string output, Level level, string layout)
        {
            Output = output;
            Level = level;
            Layout = layout;
        }

        public LogSetting Clone()
        {
            return new LogSetting(Output, Level.Clone(), Layout);
        }
    }


    public class Level
    {
        public LogLevel Min { get; set; }
        public LogLevel Max { get; set; }

        public Level(LogLevel min, LogLevel max)
        {
            Min = min;
            Max = max;
        }

        public Level Clone()
        {
            return new Level(Min, Max);
        }
    }


    /// <summary>
    /// 插件设置
    /// </summary>
    public class PluginSetting
    {
        public IPackageSearchMetadata[] Plugins { get; set; }
        public PluginCategory[] Categories { get; set; }

        public PluginSetting(IPackageSearchMetadata[] plugins, PluginCategory[] categories)
        {
            Plugins = plugins;
            Categories = categories;
        }

        public PluginSetting Clone()
        {
            return new PluginSetting((IPackageSearchMetadata[])Plugins.Clone(), (PluginCategory[])Categories.Clone());
        }
    }


    /// <summary>
    /// 插件类别
    /// </summary>
    public class PluginCategory
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 内部执行顺序
        /// </summary>
        public int[] ExecuteOrders { get; set; }

        public PluginCategory(string name, int[] executeOrders)
        {
            Name = name;
            ExecuteOrders = executeOrders;
        }
    }
}
