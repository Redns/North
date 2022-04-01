namespace ImageBed.Common
{
	public class AppSettings
	{
        /// <summary>
        /// 获取appsettings.json实例
        /// </summary>
        /// <returns></returns>
        public static IConfigurationRoot GetInstance()
        {
            return new ConfigurationBuilder()
                   .SetBasePath(Environment.CurrentDirectory)
                   .AddJsonFile("appsettings.json")
                   .AddInMemoryCollection()
                   .Build();
        }


        /// <summary>
        /// 读取appsettings.json中的数据
        /// </summary>
        /// <param name="path">属性在json中的路径</param>
        /// <returns></returns>
        public static object Get(string path)
        {
            return GetInstance()[path];
        }


        /// <summary>
        /// 修改appsettings.json文件
        /// </summary>
        /// <param name="path">待修改的变量路径</param>
        /// <param name="val">修改后的值</param>
        /// <returns></returns>
        public static bool Set(string path, object val)
        {
            try
            {
                var config = GetInstance();
                config[path] = val.ToString();
                config.Reload();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}