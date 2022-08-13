using North.Core.Data.Access;
using North.Models.Auth;
using North.Services.Logger;
using ILogger = North.Services.Logger.ILogger;

namespace North.Common
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public static class GlobalValues
    {
        public static readonly string AvatarDir = "Data/Images/Avatars";
        public static readonly string[] WithoutAuthenticationPages = new string[] { "login", "register", "signin", "verify" };

        /// <summary>
        /// 应用设置
        /// </summary>
        private static AppSetting? _appSetting;
        public static AppSetting AppSettings
        {
            get { return _appSetting ??= AppSetting.Load(); }
            set { _appSetting = value; }
        }

        /// <summary>
        /// 内存数据库
        /// </summary>
        private static MemoryDatabase? _memoryDatabase;
        public static MemoryDatabase MemoryDatabase
        {
            get
            {
                if(_memoryDatabase is null)
                {
                    using var context = new OurDbContext(AppSettings.Storage.DataBase.ConnStr);
                    _memoryDatabase = new MemoryDatabase(new SqlUserData(context).Get().ToList(),
                                                        new SqlVerifyEmailData(context).Get().ToList());
                }
                return _memoryDatabase;
            }
            set { _memoryDatabase = value; }
        }

        /// <summary>
        /// Cookie 签名标识
        /// </summary>
        private static List<UnitLoginIdentify>? _unitLoginIdentifies;
        public static List<UnitLoginIdentify> UnitLoginIdentifies
        {
            get { return _unitLoginIdentifies ??= new List<UnitLoginIdentify>(); }
            set { _unitLoginIdentifies = value; }
        }
    }
}
