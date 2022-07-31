using North.Models.Setting;

namespace North.Common
{
    public class GlobalValues
    {
        public const string AvatarDir = "Data/Images/Avatars";

        private static AppSetting? _appSettings;
        public static AppSetting AppSettings
        {
            get
            {
                if(_appSettings is null)
                {
                    _appSettings = AppSetting.Load();
                }
                return _appSettings;
            }
        }
    }
}
