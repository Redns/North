using Newtonsoft.Json;
using North.Core.Data.Entities;

namespace North.Common
{
    public class AppSetting
    {
        public GeneralSetting General { get; set; }
        public StorageSetting Storage { get; set; }
        public RegisterSetting Register { get; set; }
        public NotifySetting Notify { get; set; }
        public ApiSetting Api { get; set; }
        public LogSetting Log { get; set; }

        public AppSetting(GeneralSetting general, StorageSetting storage, RegisterSetting register, NotifySetting notify, ApiSetting api, LogSetting log)
        {
            General = general;
            Storage = storage;
            Register = register;
            Notify = notify;
            Api = api;
            Log = log;
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

    }


    public class StorageSetting
    {
        public DataBaseSetting DataBase { get; set; }

        public StorageSetting(DataBaseSetting dataBase)
        {
            DataBase = dataBase;
        }
    }


    public class DataBaseSetting
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnStr { get; set; }

        /// <summary>
        /// 数据库同步时间间隔（单位：s）
        /// </summary>
        public long SyncTimeInterval { get; set; }

        public DataBaseSetting(string connStr, long syncTimeInterval)
        {
            ConnStr = connStr;
            SyncTimeInterval = syncTimeInterval;
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
    }


    public class NotifySetting
    {
        public EmailSetting Email { get; set; }

        public NotifySetting(EmailSetting email)
        {
            Email = email;
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
    }


    public class ApiSetting
    {
        public long TokenValidTime { get; set; }

        public ApiSetting(long tokenValidTime)
        {
            TokenValidTime = tokenValidTime;
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
    }
}
