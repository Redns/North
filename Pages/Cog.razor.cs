using ImageBed.Common;
using ImageBed.Data.Entity;
using Microsoft.AspNetCore.Components.Forms;
using static ImageBed.Common.UnitNameGenerator;

namespace ImageBed.Pages
{
    partial class Cog
    {
        string imageDir = GlobalValues.appSetting?.Data?.Resources?.Images?.Path ?? "";
        bool RefreshDashboardRealTime = GlobalValues.appSetting?.Record?.RefreshRealTime ?? true;
        RenameFormat renameFormat = GlobalValues.appSetting?.Data?.Resources?.Images?.RenameFormat ?? RenameFormat.MD5;


        /// <summary>
        /// 修改设置文件
        /// </summary>
        /// <param name="editContext"></param>
        private void OnFinish(EditContext editContext)
        {
            if ((renameFormat != GlobalValues.appSetting?.Data?.Resources?.Images?.RenameFormat) ||
               (RefreshDashboardRealTime != GlobalValues.appSetting?.Record?.RefreshRealTime))
            {
                GlobalValues.appSetting.Data.Resources.Images.RenameFormat = renameFormat;
                GlobalValues.appSetting.Record.RefreshRealTime = RefreshDashboardRealTime;
                AppSetting.Save(GlobalValues.appSetting, "appsettings.json");
            }
            _message.Success("设置完成！");
        }


        /// <summary>
        /// 修改设置失败后调用
        /// </summary>
        /// <param name="editContext"></param>
        private void OnFinishFailed(EditContext editContext)
        {
            GlobalValues.appSetting = AppSetting.Parse();
            _message.Error("设置失败！");
        }
    }
}
