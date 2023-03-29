using Microsoft.Data.SqlClient;
using North.Core.Entities;
using North.Core.Services.Logger;
using SqlSugar;
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

    #region 通用设置

    /// <summary>
    /// 通用设置
    /// </summary>
    public class GeneralSetting
    {
        /// <summary>
        /// 数据库相关设置
        /// </summary>
        public DataBaseSetting DataBase { get; set; }

        /// <summary>
        /// 应用监听链接
        /// </summary>
        public string ApplicationUrl { get; set; }

        public GeneralSetting(DataBaseSetting dataBase, string applicationUrl)
        {
            DataBase = dataBase;
            ApplicationUrl = applicationUrl;
        }

        public GeneralSetting Clone() => new(DataBase.Clone(), ApplicationUrl);
    }


    /// <summary>
    /// 数据库设置
    /// </summary>
    public class DataBaseSetting
    {
        /// <summary>
        /// 使能的数据库名称
        /// </summary>
        public string EnabledName { get; set; }

        /// <summary>
        /// 可用的数据库
        /// </summary>
        public Database[] Databases { get; set; }

        public DataBaseSetting(string enabledName, Database[] databases)
        {
            EnabledName = enabledName;
            Databases = databases;
        }

        public DataBaseSetting Clone()
        {
            var databases = new List<Database>(Databases.Length);
            foreach (var database in Databases)
            {
                databases.Add(database.Clone());
            }
            return new DataBaseSetting(EnabledName, databases.ToArray());
        }
    }


    /// <summary>
    /// 数据库
    /// </summary>
    public class Database
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbType Type { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 是否自动关闭连接
        /// </summary>
        public bool IsAutoCloseConnection { get; set; }

        /// <summary>
        /// 数据库图标
        /// </summary>
        [JsonIgnore]
        public string IconClass => Type switch
        {
            DbType.MySqlConnector => "iconfont icon-mysql",
            DbType.SqlServer => "iconfont icon-SQLserver",
            DbType.Oracle => "iconfont icon-xuniku",
            DbType.PostgreSQL => "iconfont icon-postgresql",
            DbType.Sqlite => "iconfont icon-sqlite",
            DbType.OpenGauss => "iconfont icon-dakai-GAUSS",
            _ => throw new NotSupportedException("The database is not yet supported")
        };

        /// <summary>
        /// 图标颜色
        /// </summary>
        [JsonIgnore]
        public string IconColor => Type switch
        {
            DbType.MySqlConnector => "#2A8393",
            DbType.SqlServer => "#DA5969",
            DbType.Oracle => "#F80F0A",
            DbType.PostgreSQL => "#2F5F91",
            DbType.Sqlite => "#00CCFF",
            DbType.OpenGauss => "#297EB6",
            _ => throw new NotSupportedException("The database is not yet supported")
        };

        /// <summary>
        /// 数据库连接字符串模板
        /// </summary>
        [JsonIgnore]
        public string ConnectionStringTemplate => Type switch
        {
            DbType.MySqlConnector => "server=localhost;Database=SqlSugar4xTest;Uid=root;Pwd=haosql;",
            DbType.SqlServer => "server=.;uid=sa;pwd=haosql;database=SQLSUGAR4XTEST",
            DbType.Oracle => "Data Source=localhost/orcl;User ID=system;Password=haha;",
            DbType.PostgreSQL => "PORT=5432;DATABASE=SqlSugar4xTest;HOST=localhost;PASSWORD=haosql;USER ID=postgres",
            DbType.Sqlite => "DataSource=DataBase/SqlSugar4xTest.sqlite",
            DbType.OpenGauss => "PORT=5432;DATABASE=SqlSugar4xTest;HOST=localhost;PASSWORD=haosql;USER ID=postgres;No Reset On Close=true;",
            _ => throw new NotSupportedException("The database is not yet supported")
        };

        public Database(string name, DbType type, string connectionString, bool isAutoCloseConnection)
        {
            Name = name;
            Type = type;
            ConnectionString = connectionString;
            IsAutoCloseConnection = isAutoCloseConnection;
        }


        /// <summary>
        /// 验证数据库连接字符串是否有效
        /// </summary>
        /// <returns></returns>
        public async ValueTask<bool> CheckConnection(TimeSpan? timeout = null)
        {
            if (Type is DbType.Sqlite)
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

        public Database Clone() => new(Name, Type, ConnectionString, IsAutoCloseConnection);
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

    #endregion

    #region 外观设置

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
        /// 图床图标
        /// </summary>
        public string Icon { get; set; }

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

        public AppearanceSetting(string name, string icon, bool navAutoExpand, string backgroundUrl, string footer)
        {
            Name = name;
            Icon = icon;
            NavAutoExpand = navAutoExpand;
            BackgroundUrl = backgroundUrl;
            Footer = footer;
        }

        public AppearanceSetting Clone() => new(Name, Icon, NavAutoExpand, BackgroundUrl, Footer);
    }

    #endregion

    #region 注册设置

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
        public bool IsApiAvailable { get; set; }            // 是否启用 API
        public long MaxUploadNums { get; set; }            // 最大上传数量（张）
        public double MaxUploadCapacity { get; set; }        // 最大上传容量（MB）
        public long SingleMaxUploadNums { get; set; }      // 单次最大上传数量（张）
        public double SingleMaxUploadCapacity { get; set; }  // 单次最大上传容量（MB）

        public RegisterSettingDefault(bool isApiAvailable, long maxUploadNums, double maxUploadCapacity, long singleMaxUploadNums, double singleMaxUploadCapacity)
        {
            IsApiAvailable = isApiAvailable;
            MaxUploadNums = maxUploadNums;
            MaxUploadCapacity = maxUploadCapacity;
            SingleMaxUploadNums = singleMaxUploadNums;
            SingleMaxUploadCapacity = singleMaxUploadCapacity;
        }

        public RegisterSettingDefault Clone() => new(IsApiAvailable, MaxUploadNums, MaxUploadCapacity, SingleMaxUploadNums, SingleMaxUploadCapacity);
    }

    #endregion

    #region 通知设置

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

    #endregion

    #region 授权设置

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

    #endregion

    #region 插件设置

    /// <summary>
    /// 插件设置
    /// </summary>
    public class PluginSetting
    {
        /// <summary>
        /// 镜像地址
        /// </summary>
        public string ImageSource { get; set; }

        public PluginSetting(string imageSource)
        {
            ImageSource = imageSource;
        }

        public PluginSetting Clone() => new(ImageSource);
    }

    #endregion
}
