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
            AppSetting? appSetting = AppSetting.Parse();
            if(appSetting != null)
            {
                try
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    File.Copy(appSetting?.Data?.Resources?.Database?.Template ?? $"Data/Database/imagebed-blank.sqlite", $"{path}/imagebed.sqlite");
                    return $"Data Source={path}/imagebed.sqlite;";
                }
                catch { }
            }
            return string.Empty;           
        }


        /// <summary>
        /// 执行SQL命令
        /// </summary>
        /// <param name="sqlCommand">SQL命令</param>
        /// <returns></returns>
        public static bool ExecuteSQLCommand(string sqlCommand)
        {
            string? connStr = AppSetting.Parse()?.Data?.Resources?.Database?.ConnStr;
            if (!string.IsNullOrEmpty(connStr))
            {
                try
                {
                    // 打开数据库
                    var database = new SQLiteConnection(connStr);
                    database.Open();

                    // 执行SQL命令
                    if(new SQLiteCommand(sqlCommand, database).ExecuteNonQuery() != -1)
                    {
                        return true;
                    }
                }
                catch (Exception) { }
            }
            return false;
        }
    }
}
