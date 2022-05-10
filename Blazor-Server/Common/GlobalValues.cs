namespace ImageBed.Common
{
    public class GlobalValues
    {
        public const double SCREEN_BOUND = 500.0F;                              // 屏幕宽度界限

        public const string LOCALSTORE_KEY_USERNAME = "UserName";               // 用户名标识
        public const string LOCALSTORE_KEY_TOKEN = "Token";                     // 令牌标识

        public const string PATH_DATABASE = "Data/Databases/imagebed.sqlite";   // 数据库路径

        public static AppSetting? AppSetting { get; set; }                      // 全局设置
        public static LoggerHelper Logger { get; set; } = new LoggerHelper();   // 日志记录器
    }
}
