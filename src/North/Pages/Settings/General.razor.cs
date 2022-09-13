using Microsoft.EntityFrameworkCore;
using MudBlazor;
using North.Common;
using North.Core.Common;
using North.Core.Entities;

namespace North.Pages.Settings
{
    partial class General
    {
        private bool SaveRunning { get; set; } = false;
        private bool RestoreRunning { get; set; } = false;
        private GeneralSetting GeneralSetting { get; set; } = GlobalValues.AppSettings.General.Clone();

        private string DatabaseIconClass(DatabaseType type) => type switch
        {
            DatabaseType.Sqlite => "iconfont icon-sqlite",
            DatabaseType.SqlServer => "iconfont icon-SQLserver",
            DatabaseType.MySQL => "iconfont icon-mysql",
            DatabaseType.PostgreSQL => "iconfont icon-postgresql",
            _ => throw new NotSupportedException("Database not supported")
        };


        /// <summary>
        /// 保存设置
        /// </summary>
        /// <returns></returns>
        private async Task SaveSettings()
        {
            try
            {
                await InvokeAsync(() =>
                {
                    SaveRunning = true;
                    StateHasChanged();
                });

                _snackbar.Add("保存成功", Severity.Success);
            }
            catch (Exception e)
            {
                GeneralSetting = GlobalValues.AppSettings.General.Clone();

                _snackbar.Add("保存失败，已还原设置", Severity.Error);
                _logger.Error("Failed to save general settings", e);
            }
            finally
            {
                SaveRunning = false;
            }
        }


        /// <summary>
        /// 还原设置
        /// </summary>
        /// <returns></returns>
        private async Task RestoreSettings()
        {
            try
            {
                await InvokeAsync(() =>
                {
                    RestoreRunning = true;
                    StateHasChanged();
                });

                GeneralSetting = GlobalValues.AppSettings.General.Clone();

                _snackbar.Add("已还原设置", Severity.Success);
            }
            catch (Exception e)
            {
                _snackbar.Add("设置还原失败", Severity.Error);
                _logger.Error("Failed to restore settings", e);
            }
            finally
            {
                RestoreRunning = false;
            }
        }


        /// <summary>
        /// 生成连接字符串模板
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        private void GenerateConnectStringTemplate()
        {
            //GeneralSetting.DataBase.ConnectionString = GeneralSetting.DataBase.Type switch
            //{
            //    DatabaseType.Sqlite => "Data Source={DATABASE_LOCATION};",
            //    DatabaseType.SqlServer => "Data Source={SERVER_ADDRESS};Initial Catalog={DATABASE_NAME};User Id={USER_NAME};Password={USER_PASSWORD};",
            //    DatabaseType.MySQL => "Server={SERVER_ADDRESS};Database={DATABASE_NAME};Uid={USER_NAME};Pwd={USER_PASSWORD};",
            //    DatabaseType.PostgreSQL => "Host={SERVER_ADDRESS};Port={PORT};Database={DATABASE_NAME};Username={USER_NAME};Password={USER_PASSWORD};",
            //    _ => throw new NotSupportedException("Database not supported")
            //};
        }


        /// <summary>
        /// 迁移数据库
        /// </summary>
        /// <returns></returns>
        private static async Task MigrateDatabaseAsync(DataBaseSetting oldSetting, DataBaseSetting newSetting)
        {
            //var oldDbContextOptionsBuilder = new DbContextOptionsBuilder();
            //var newDbContextOptionsBuilder = new DbContextOptionsBuilder();

            //oldSetting.InitDbContextBuilder(oldDbContextOptionsBuilder);
            //newSetting.InitDbContextBuilder(newDbContextOptionsBuilder);

            //using var oldContext = new NorthDbContext(oldDbContextOptionsBuilder.Options);
            //using var newContext = new NorthDbContext(newDbContextOptionsBuilder.Options);

            //await oldContext.MigrateDatabaseAsync(newContext);
        }
    }
}
