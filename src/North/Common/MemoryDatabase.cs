using North.Core.Data.Access;
using North.Core.Data.Entities;
using System.Timers;

namespace North.Common
{
    public class MemoryDatabase
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnStr { get; set; }

        /// <summary>
        /// 数据库同步间隔（单位：秒）
        /// </summary>
        public ulong SyncTimeInterval { get; set; }

        /// <summary>
        /// 数据库同步定时器
        /// </summary>
        public System.Timers.Timer SyncTimer { get; set; }  
        public List<UserEntity> Users { get; set; }
        public List<VerifyEmailEntity> VerifyEmails { get; set; }

        public MemoryDatabase(string connStr, ulong syncTimeInterval)
        {
            ConnStr = connStr;

            using var context = new OurDbContext(connStr);
            Users = new SqlUserData(context).Get().ToList();
            VerifyEmails = new SqlVerifyEmailData(context).Get().ToList();

            // 创建数据库同步定时器
            SyncTimeInterval = syncTimeInterval;
            SyncTimer = new System.Timers.Timer()
            {
                AutoReset = true,
                Interval = SyncTimeInterval * 1000
            };
            SyncTimer.Elapsed += SyncDatabase;
            SyncTimer.Enabled = true;
        }

        public MemoryDatabase(string connStr, ulong syncTimeInterval, List<UserEntity> users, List<VerifyEmailEntity> verifyEmails)
        {
            ConnStr = connStr;
            Users = users;
            VerifyEmails = verifyEmails;

            // 创建数据库同步定时器
            SyncTimeInterval = syncTimeInterval;
            SyncTimeInterval = syncTimeInterval;
            SyncTimer = new System.Timers.Timer()
            {
                AutoReset = true,
                Interval = SyncTimeInterval * 1000
            };
            SyncTimer.Elapsed += SyncDatabase;
            SyncTimer.Enabled = true;
        }


        /// <summary>
        /// 同步数据库
        /// </summary>
        public void SyncDatabase(object? source = null, ElapsedEventArgs? e = null)
        {
            using var context = new OurDbContext(ConnStr);

            // 更新用户信息
            var sqlUserData = new SqlUserData(context);
            sqlUserData.RemoveRange(sqlUserData.Get());
            sqlUserData.AddRange(GlobalValues.MemoryDatabase.Users);

            // 更新邮件信息
            var sqlVerifyEmailData = new SqlVerifyEmailData(context);
            sqlVerifyEmailData.RemoveRange(sqlVerifyEmailData.Get());
            sqlVerifyEmailData.AddRange(GlobalValues.MemoryDatabase.VerifyEmails);
        }
    }
}
