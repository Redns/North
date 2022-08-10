using Newtonsoft.Json;
using North.Data.Entities;

namespace North.Models.Setting
{
    public class AppSetting
    {
        public GeneralSetting General { get; set; }
        public RegisterSetting Register { get; set; }
        public NotifySetting Notify { get; set; }
        public ApiSetting Api { get; set; }
        public LogSetting Log { get; set; }

        public AppSetting(GeneralSetting general, RegisterSetting register, NotifySetting notify, ApiSetting api, LogSetting log)
        {
            General = general;
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
            return JsonConvert.DeserializeObject<AppSetting>(File.ReadAllText(path)) ?? throw new Exception($"Failed to parse {path}");
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


    public class RegisterSetting
    {
        public bool AllowRegister { get; set; } = true;         // 是否允许注册
        public ulong MaxAvatarSize { get; set; }                // 头像最大尺寸（MB）
        public RegisterSettingDefault Default { get; set; }     // 默认注册设置

        public RegisterSetting(bool allowRegister, ulong maxAvatarSize, RegisterSettingDefault @default)
        {
            AllowRegister = allowRegister;
            MaxAvatarSize = maxAvatarSize;
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
        public ulong MaxUploadNums { get; set; }            // 最大上传数量（张）
        public ulong MaxUploadCapacity { get; set; }        // 最大上传容量（MB）
        public ulong SingleMaxUploadNums { get; set; }      // 单次最大上传数量（张）
        public ulong SingleMaxUploadCapacity { get; set; }  // 单次最大上传容量（MB）

        public RegisterSettingDefault(Permission permission, bool isApiAvailable, ulong maxUploadNums, ulong maxUploadCapacity, ulong singleMaxUploadNums, ulong singleMaxUploadCapacity)
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
        public string Account { get; set; }     // 邮箱账号
        public string Code { get; set; }        // 授权码
        public ulong ValidTime { get; set; }    // 有效时间（ms）

        public EmailSetting(string account, string code, ulong validTime)
        {
            Account = account;
            Code = code;
            ValidTime = validTime;
        }
    }


    public class ApiSetting
    {
        public ulong TokenValidTime { get; set; }

        public ApiSetting(ulong tokenValidTime)
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
