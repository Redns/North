using MudBlazor;
using North.Common;
using North.Core.Common;
namespace North.Pages.Settings
{
    partial class General
    {
        private bool SaveRunning { get; set; } = false; 
        private bool RestoreRunning { get; set; } = false;
        private bool IsDialogVisiable { get; set; } = false;
        private GeneralSetting GeneralSetting { get; set; } = GlobalValues.AppSettings.General.Clone();

        private void OpenDatabaseEditDialog()
        {
            var databaseEditDialogParams = new DialogParameters
            {
                { "Databases", GeneralSetting.DataBase.Databases },
                { "EnableDatabaseName", GeneralSetting.DataBase.EnabledName }
            };
            IsDialogVisiable = true;
        }


        private void RemoveDatabase(Core.Common.Database database)
        {
            if(database.Name == GeneralSetting.DataBase.EnabledName)
            {
                _snackbar.Add("不能删除正在使用的数据源", Severity.Error);
            }
            else
            {
                GeneralSetting.DataBase.Databases = GeneralSetting.DataBase.Databases.Where(db => db != database).ToArray();

                _snackbar.Add($"已删除数据源 {database.Name}", Severity.Success);
            }
        }


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
