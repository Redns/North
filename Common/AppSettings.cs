namespace ImageBed.Common
{
	public class AppSettings
	{
        /// <summary>
        /// 获取appsettings.json实例
        /// </summary>
        /// <returns></returns>
        public static IConfigurationRoot? GetInstance()
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
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .AddInMemoryCollection()
                .Build();
            return config[path];
        }
    }
}
