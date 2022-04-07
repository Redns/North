using ImageBed.Common;
using ImageBed.Data.Entity;
using System.Data.SQLite;

namespace ImageBed.Data.Access
{
    public class SQLiteHelper
    {
        /// <summary>
        /// 创建SQLite数据库
        /// </summary>
        /// <param name="path">数据库存储路径</param>
        /// <returns></returns>
        public static string CreateSQLiteDatabase(string path)
        {
            if(GlobalValues.appSetting != null)
            {
                try
                {
                    if (File.Exists(GlobalValues.appSetting?.Data?.Resources?.Database?.TemplatePath))
                    {
                        File.Copy(GlobalValues.appSetting?.Data?.Resources?.Database?.TemplatePath ?? $"Data/Database/imagebed-blank.sqlite", path);
                        return $"Data Source={path};";
                    }
                }
                catch { }
            }
            return string.Empty;           
        }
    }
}
