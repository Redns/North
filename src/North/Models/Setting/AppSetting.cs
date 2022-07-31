using Newtonsoft.Json;
using North.Data.Entities;

namespace North.Models.Setting
{
    public class AppSetting
    {
        public GeneralSetting General { get; set; }
        public RegisterSetting Register { get; set; }

        public AppSetting(GeneralSetting general, RegisterSetting register)
        {
            General = general;
            Register = register;
        }


        /// <summary>
        /// 加载设置
        /// </summary>
        /// <param name="path">设置文件路径（默认 appsettings.json）</param>
        /// <returns></returns>
        public static AppSetting Load(string path = "appsettings.json")
        {
            var appSetting = JsonConvert.DeserializeObject<AppSetting>(File.ReadAllText(path));
            if(appSetting is not null) 
            { 
                return appSetting; 
            }
            else
            {
                throw new Exception($"Failed to parse {path}");
            }
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
        /// 复制当前设置
        /// </summary>
        /// <returns></returns>
        public AppSetting? Clone()
        {
            return JsonConvert.DeserializeObject<AppSetting>(ToString());
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
        public long MaxAvatarSize { get; set; }                 // 头像最大尺寸（MB）
        public RegisterSettingDefault Default { get; set; }     // 默认注册设置

        public RegisterSetting(long maxAvatarSize, RegisterSettingDefault @default)
        {
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
        public long MaxUploadNums { get; set; }             // 最大上传数量（张）
        public long MaxUploadCapacity { get; set; }         // 最大上传容量（MB）
        public long SingleMaxUploadNums { get; set; }       // 单次最大上传数量（张）
        public long SingleMaxUploadCapacity { get; set; }   // 单次最大上传容量（MB）

        public RegisterSettingDefault(Permission permission, bool isApiAvailable, long maxUploadNums, long maxUploadCapacity, long singleMaxUploadNums, long singleMaxUploadCapacity)
        {
            Permission = permission;
            IsApiAvailable = isApiAvailable;
            MaxUploadNums = maxUploadNums;
            MaxUploadCapacity = maxUploadCapacity;
            SingleMaxUploadNums = singleMaxUploadNums;
            SingleMaxUploadCapacity = singleMaxUploadCapacity;
        }
    }
}
