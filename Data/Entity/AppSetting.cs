using Newtonsoft.Json;
using static ImageBed.Common.UnitNameGenerator;

namespace ImageBed.Data.Entity
{
    public class AppSetting
    {
        public Data? Data { get; set; }

        /// <summary>
        /// 解析配置文件
        /// </summary>
        /// <returns></returns>
        public static AppSetting? Parse()
        {
            try
            {
                return JsonConvert.DeserializeObject<AppSetting>(File.ReadAllText("appSettings.json"));
            }
            catch (Exception) { }
            return null;
        }


        /// <summary>
        /// 解析配置文件
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static AppSetting? Parse(string content)
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                try
                {
                    return JsonConvert.DeserializeObject<AppSetting>(content);
                }
                catch (Exception) { }
            }
            return null;
        }


        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="GlobalValues.appSetting"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Save(AppSetting appSetting, string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(appSetting, Formatting.Indented));
                    return true;
                }
                catch(Exception) { }
            }
            return false;
        }
    }


    public class Data
    {
        public Resource? Resources { get; set; }
    }


    public class Resource
    {
        public Image? Images { get; set; }
        public Database? Database { get; set; }
    }


    public class Image
    {
        public string? Path { get; set; }
        public RenameFormat RenameFormat { get; set; }
    }


    public class Database
    {
        public string? Path { get; set; }
        public string? TemplatePath { get; set; }
    }
}
