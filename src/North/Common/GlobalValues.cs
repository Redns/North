using North.Models.Setting;

namespace North.Common
{
    public class GlobalValues
    {
        public const string AvatarDir = "Data/Images/Avatars";
        public static AppSetting AppSettings { get; set; } = AppSetting.Load();
    }
}
