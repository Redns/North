using North.Models.Setting;

namespace North.Common
{
    public static class GlobalValues
    {
        public static readonly string AvatarDir = "Data/Images/Avatars";
        public static readonly string[] WithoutAuthenticationPages = new string[] { "login", "register", "signin", "verify" };
        public static AppSetting AppSettings { get; set; } = AppSetting.Load();
    }
}
