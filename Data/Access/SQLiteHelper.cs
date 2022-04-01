using ImageBed.Common;
using System.Data.SQLite;

namespace ImageBed.Data.Access
{
    public class SQLiteHelper
    {
        // 连接字符串路径
        public const string connPath = "Data:Resources:Database:connStr";


        /// <summary>
        /// 创建SQLite数据库
        /// </summary>
        /// <param name="path">数据库存储路径</param>
        /// <returns></returns>
        public static string CreateSQLiteDatabase(string path)
        {
            if(UnitNameGenerator.GetFileExtension(path) == "sqlite")
            {
                SQLiteConnection.CreateFile(path);
                return $"Data Source={path};";
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
            string? connStr = AppSettings.Get(connPath).ToString();
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


        /// <summary>
        /// 生成"创建Images数据库表"命令
        /// </summary>
        /// <param name="tableName">数据库表名字</param>
        /// <returns></returns>
        public static string CreateImageTableCommand(string tableName)
        {
            return $"create table {tableName} (Id varchar(30), Name varchar(40), Url varchar(100), Size varchar(10), UploadTime varchar(20), Dpi varchar(20), Owner varchar(20))";
        }
    }
}
