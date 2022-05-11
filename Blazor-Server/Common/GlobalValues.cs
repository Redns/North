namespace ImageBed.Common
{
    public class GlobalValues
    {
        public const double SCREEN_BOUND = 500.0F;                              // 屏幕宽度界限

        public const string LOCALSTORE_KEY_USERNAME = "UserName";               // 用户名标识
        public const string LOCALSTORE_KEY_TOKEN = "Token";                     // 令牌标识

        public const string CONNSTR_DATABASE = "Data Source=Data/Databases/imagebed.sqlite;";   // 数据库连接字符串

        public const string ROUTER_INDEX = "";
        public const string ROUTER_PICS = "pics";
        public const string ROUTER_DASHBOARD = "dashboard";
        public const string ROUTER_COG = "cog";
        public const string ROUTER_LOGIN = "login";
        public const string ROUTER_PLUGIN_STORE = "plugin_store";
        public const string ROUTER_USERS = "users";

        public static AppSetting? AppSetting { get; set; }                      // 全局设置
        public static LoggerHelper Logger { get; set; } = new LoggerHelper();   // 日志记录器
    }
}
