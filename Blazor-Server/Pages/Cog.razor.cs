using ImageBed.Common;
using ImageBed.Data.Entity;
using Microsoft.AspNetCore.Components.Forms;
using System.Text.RegularExpressions;
using static ImageBed.Common.UnitNameGenerator;

namespace ImageBed.Pages
{
    partial class Cog
    {
        // 图片存储路径
        string imageDir = GlobalValues.appSetting?.Data?.Resources?.Images?.Path ?? "";

        // 是否实时刷新仪表盘
        bool RefreshDashboardRealTime = GlobalValues.appSetting?.Record?.RefreshRealTime ?? true;

        // 图片重命名状态
        RenameFormat renameFormat = GlobalValues.appSetting?.Data?.Resources?.Images?.RenameFormat ?? RenameFormat.MD5;

        // 系统资源统计开始时间
        double RefreshStartTime = GlobalValues.appSetting?.Record?.RefreshStartTime ?? 0;

        // 图片尺寸限制
        double ImageMaxSize = GlobalValues.appSetting?.Data.Resources.Images.MaxSize ?? 0;

        // 单次上传数量限制
        double ImageMaxNum = GlobalValues.appSetting?.Data.Resources.Images.MaxNum ?? 0;

        // 图库界面默认视图
        bool ViewList = GlobalValues.appSetting.Pics.ViewList;

        // 网页页脚
        string footer = GlobalValues.appSetting?.footer ?? "";

        // 上传图片格式限制
        string[] imageFormatOptions = { "jpg", "jpeg", "png", "gif", "bmp", "svg", "raw" };
        string[] imageFormatChoose = GlobalValues.appSetting.Data.Resources.Images.Format.Split(",");

        // 复制链接格式
        UrlFormat urlFormat = GlobalValues.appSetting.Data.Resources.Images.UrlFormat;

        // 邮件设置
        bool TestRunning = false;
        Email emailConfig = GlobalValues.appSetting.Notify.Email;
        Condition emailSendCondition = GlobalValues.appSetting.Notify.Condition;

        bool loading = true;


        protected override void OnAfterRender(bool firstRender)
        {
            loading = false;
            base.OnAfterRender(firstRender);
            StateHasChanged();
        }


        /// <summary>
        /// 设置上传图片格式限制
        /// </summary>
        /// <param name="checkedValues"></param>
        void SetImageFormat(string[] checkedValues)
        {
            imageFormatChoose = (string[])checkedValues.Clone();
        }


        /// <summary>
        /// 修改设置文件
        /// </summary>
        /// <param name="editContext"></param>
        private void OnFinish(EditContext editContext)
        {
            GlobalValues.appSetting.Data.Resources.Images.RenameFormat = renameFormat;
            GlobalValues.appSetting.Record.RefreshRealTime = RefreshDashboardRealTime;

            if (RefreshStartTime < 0) { RefreshStartTime = 0; }
            else if (RefreshStartTime > 18) { RefreshStartTime = 18; }
            GlobalValues.appSetting.Record.RefreshStartTime = (int)RefreshStartTime;

            if (ImageMaxSize < 0) { ImageMaxSize = 0; }
            else if (ImageMaxSize > 99999) { ImageMaxSize = 99999; }
            GlobalValues.appSetting.Data.Resources.Images.MaxSize = (int)ImageMaxSize;

            if (ImageMaxNum < 0) { ImageMaxNum = 0; }
            else if (ImageMaxNum > 99999) { ImageMaxNum = 99999; }
            GlobalValues.appSetting.Data.Resources.Images.MaxNum = (int)ImageMaxNum;

            GlobalValues.appSetting.Notify.Email = emailConfig;
            GlobalValues.appSetting.Notify.Condition = emailSendCondition;

            GlobalValues.appSetting.footer = footer;

            if(imageFormatChoose.Length == 0)
            {
                _message.Error("请至少选择一种图片格式 !");
                return;
            }
            else
            {
                string formatStr = "";
                foreach (string format in imageFormatChoose)
                {
                    formatStr += $"{format},";
                }
                GlobalValues.appSetting.Data.Resources.Images.Format = formatStr;
            }

            GlobalValues.appSetting.Data.Resources.Images.UrlFormat = urlFormat;
            GlobalValues.appSetting.Pics.ViewList = ViewList;

            AppSetting.Save(GlobalValues.appSetting, "appsettings.json");

            _message.Success("设置完成 !");
        }


        /// <summary>
        /// 修改设置失败后调用
        /// </summary>
        /// <param name="editContext"></param>
        private void OnFinishFailed(EditContext editContext)
        {
            GlobalValues.appSetting = AppSetting.Parse();
            _message.Error("设置失败 !");
        }


        /// <summary>
        /// 测试邮箱和验证码
        /// </summary>
        private async Task TestEmailConfig()
        {
            TestRunning = true;

            Regex emailRegex = new(@"^\w+@\w+.com$");
            if(emailRegex.IsMatch(emailConfig.From) && (emailRegex.IsMatch(emailConfig.To)))
            {
                await MailHelper.PostEmails(new MailEntity
                {
                    FromPerson = emailConfig.From,
                    RecipientArry = emailConfig.To.Split(","),
                    MailTitle = "ImageBed",
                    MailBody = "这是一封测试邮件, 您的邮箱配置成功 !",
                    Code = emailConfig.Code,
                    IsBodyHtml = false
                });

                TestRunning = false;

                _ = _message.Success("测试邮件发送成功, 请注意查收 !");
            }
            else
            {
                _ = _message.Error("请检查邮件配置是否正确 !");
            }
        }
    }
}
