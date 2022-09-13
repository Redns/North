using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using North.Core.Entities;
using North.Core.Services.Logger;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace North.Core.Common
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
        public static AppSetting Load(string path = "appsettings.json") => JsonSerializer.Deserialize<AppSetting>(File.ReadAllText(path)) ?? throw new Exception($"Load {path} failed");


        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path = "appsettings.json") => File.WriteAllText(path, ToString());

        public AppSetting Clone() => new(General.Clone(), Appearance.Clone(), Register.Clone(), Notify.Clone(), Auth.Clone(), Log.Clone(), Plugin.Clone());

        public override string ToString() => JsonSerializer.Serialize(this, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
    }


    /// <summary>
    /// 通用设置
    /// </summary>
    public class GeneralSetting
    {
        public DataBaseSetting DataBase { get; set; }

        public GeneralSetting(DataBaseSetting dataBase)
        {
            DataBase = dataBase;
        }

        public GeneralSetting Clone() => new(DataBase.Clone());
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

        public AppearanceSetting Clone() => new(Name, NavAutoExpand, BackgroundUrl, Footer);
    }


    /// <summary>
    /// 数据库设置
    /// </summary>
    public class DataBaseSetting
    {
        public string EnabledName { get; set; }
        public Database[] Databases { get; set; }

        public DataBaseSetting(string enabledName, Database[] databases)
        {
            EnabledName = enabledName;
            Databases = databases;
        }

        public DataBaseSetting Clone()
        {
            var databases = new List<Database>(Databases.Length);
            foreach(var database in Databases)
            {
                databases.Add(database.Clone());
            }
            return new DataBaseSetting(EnabledName, databases.ToArray());
        }
    }


    public class Database
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType Type { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        public Database(string name, DatabaseType type, string connectionString)
        {
            Name = name;
            Type = type;
            ConnectionString = connectionString;
        }


        /// <summary>
        /// 初始化 EFCore 上下文
        /// </summary>
        [JsonIgnore]
        public Action<DbContextOptionsBuilder> InitDbContextBuilder => (builder) =>
        {
            switch (Type)
            {
                case DatabaseType.Sqlite: builder.UseSqlite(ConnectionString); break;
                case DatabaseType.SqlServer: builder.UseSqlServer(ConnectionString); break;
                case DatabaseType.MySQL: builder.UseMySql(ServerVersion.AutoDetect(ConnectionString)); break;
                case DatabaseType.PostgreSQL: builder.UseNpgsql(ConnectionString); break;
                default: throw new NotSupportedException("Database is unsupported");
            }
        };


        /// <summary>
        /// 验证数据库连接字符串是否有效
        /// </summary>
        /// <returns></returns>
        public async ValueTask<bool> CheckConnection(TimeSpan? timeout = null)
        {
            if (Type is DatabaseType.Sqlite)
            {
                var databaseLocation = ConnectionString.Split('=').Last();
                return File.Exists(!databaseLocation.EndsWith(';') ? databaseLocation : databaseLocation[..^1]);
            }
            else
            {
                var conn = new SqlConnection(ConnectionString);
                try
                {
                    await conn.OpenAsync().WaitAsync(timeout ?? TimeSpan.FromSeconds(3));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    _ = conn.CloseAsync();
                    _ = conn.DisposeAsync();
                }
            }
        }

        public Database Clone() => new(Name, Type, ConnectionString);
    }


    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseType
    {
        Sqlite = 0,
        SqlServer,
        MySQL,
        PostgreSQL
    }


    /// <summary>
    /// 注册设置
    /// </summary>
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

        public RegisterSetting Clone() => new(AllowRegister, MaxAvatarSize, VerifyEmailValidTime, Default.Clone());
    }


    /// <summary>
    /// 默认注册设置
    /// </summary>
    public class RegisterSettingDefault
    {
        public UserPermission Permission { get; set; }          // 用户权限
        public bool IsApiAvailable { get; set; }            // 是否启用 API
        public long MaxUploadNums { get; set; }            // 最大上传数量（张）
        public double MaxUploadCapacity { get; set; }        // 最大上传容量（MB）
        public long SingleMaxUploadNums { get; set; }      // 单次最大上传数量（张）
        public double SingleMaxUploadCapacity { get; set; }  // 单次最大上传容量（MB）

        public RegisterSettingDefault(UserPermission permission, bool isApiAvailable, long maxUploadNums, double maxUploadCapacity, long singleMaxUploadNums, double singleMaxUploadCapacity)
        {
            Permission = permission;
            IsApiAvailable = isApiAvailable;
            MaxUploadNums = maxUploadNums;
            MaxUploadCapacity = maxUploadCapacity;
            SingleMaxUploadNums = singleMaxUploadNums;
            SingleMaxUploadCapacity = singleMaxUploadCapacity;
        }

        public RegisterSettingDefault Clone() => new(Permission, IsApiAvailable, MaxUploadNums, MaxUploadCapacity, SingleMaxUploadNums, SingleMaxUploadCapacity);
    }


    /// <summary>
    /// 通知设置
    /// </summary>
    public class NotifySetting
    {
        public EmailSetting Email { get; set; }

        public NotifySetting(EmailSetting email)
        {
            Email = email;
        }

        public NotifySetting Clone() => new(Email.Clone());
    }


    /// <summary>
    /// 邮箱设置
    /// </summary>
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

        public EmailSetting Clone() => new(Account, Code);
    }


    /// <summary>
    /// 授权设置
    /// </summary>
    public class AuthSetting
    {
        /// <summary>
        /// 网页 Cookie 有效期（s）
        /// </summary>
        public long CookieValidTime { get; set; }

        public AuthSetting(long cookieValidTime)
        {
            CookieValidTime = cookieValidTime;
        }

        public AuthSetting Clone() => new(CookieValidTime);
    }


    /// <summary>
    /// 插件设置
    /// </summary>
    public class PluginSetting
    {
        public string InstallDir { get; set; }
        public List<Plugin> Plugins { get; set; }
        public List<PluginCategory> Categories { get; set; }

        public PluginSetting(string installDir, List<Plugin> plugins, List<PluginCategory> categories)
        {
            InstallDir = installDir;
            Plugins = plugins;
            Categories = categories;
        }

        public PluginSetting Clone()
        {
            var plugins = new List<Plugin>(Plugins.Count);
            var categories = new List<PluginCategory>(Categories.Count);

            Plugins.ForEach(plugin => plugins.Add(plugin));
            Categories.ForEach(category => categories.Add(category));

            return new PluginSetting(InstallDir, plugins, categories);
        }
    }


    /// <summary>
    /// NuGet 插件
    /// </summary>
    public class Plugin
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Authors { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public SemanticVersion Version { get; set; }

        /// <summary>
        /// 下载量
        /// </summary>
        public long DownloadCount { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public PluginState State { get; set; }

        public Plugin(IPackageSearchMetadata package, PluginState state)
        {
            Id = package.Identity.Id;
            Authors = package.Authors;
            Version = package.Identity.Version;
            DownloadCount = package.DownloadCount ?? 0L;
            IconUrl = package.IconUrl?.AbsolutePath ?? "https://www.nuget.org/Content/gallery/img/default-package-icon.svg";
            Description = package.Description;
            State = state;
        }

        public Plugin(string id, string authors, SemanticVersion version, long downloadCount, string iconUrl, string description, PluginState state)
        {
            Id = id;
            Authors = authors;
            Version = version;
            DownloadCount = downloadCount;
            IconUrl = iconUrl;
            Description = description;
            State = state;
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
        /// 被执行的模块
        /// </summary>
        public List<string> Executed { get; set; }

        /// <summary>
        /// 未被执行的模块
        /// </summary>
        public List<string> UnExecuted { get; set; }

        public PluginCategory(string name, List<string> executed, List<string> unExecuted)
        {
            Name = name;
            Executed = executed;
            UnExecuted = unExecuted;
        }
    }


    public enum PluginState
    {
        UnInstall = 0,
        Enable,
        Disable
    }
}
