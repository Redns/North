using ImageBed.Common;
using Newtonsoft.Json;
using ImageBed.Data.Entity;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Forms;
using static ImageBed.Common.UnitNameGenerator;

namespace ImageBed.Pages
{
    partial class Cog : IDisposable
    {
        bool spinning;                          // 页面加载标志
        bool emailTestRunning;                  // 邮箱测试中标志

        Image? imageConfig;                     // 图片设置
        Record? recordConfig;                   // 系统资源记录设置
        Data.Entity.Pics? picsConfig;           // 图库设置
        Notify? notifyConfig;                   // 消息设置
        Footer? footerConfig;                   // 页脚设置
        Update? updateConfig;                   // 更新设置


        // 上传图片格式限制
        string[] imageFormatChoose;
        string[] imageFormatOptions = { "jpg", "jpeg", "png", "gif", "bmp", "svg", "raw" };


        /// <summary>
        /// 初始化页面
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            spinning = true;

            emailTestRunning = false;

            var appConfig = JsonConvert.DeserializeObject<AppSetting>(JsonConvert.SerializeObject(GlobalValues.appSetting));
            if (appConfig != null)
            {
                imageConfig = appConfig.Data.Resources.Images;
                recordConfig = appConfig.Record;
                picsConfig = appConfig.Pics;
                notifyConfig = appConfig.Notify;
                footerConfig = appConfig.Footer;
                updateConfig = appConfig.Update;
            }

            imageFormatChoose = imageConfig.Format.Split('.', ',').Where(suffix => !string.IsNullOrEmpty(suffix)).ToArray();

            await base.OnInitializedAsync();
        }


        /// <summary>
        /// 页面渲染结束后调用
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                spinning = false;
                await InvokeAsync(() => { StateHasChanged(); });
            }
            await base.OnAfterRenderAsync(firstRender);
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
        async Task OnFinish(EditContext editContext)
        {
            if(imageFormatChoose.Length == 0)
            {
                await _message.Error("请至少选择一种图片格式 !");
            }
            else
            {
                // 格式化文件筛选列表
                string formatStr = string.Empty;
                foreach (string format in imageFormatChoose)
                {
                    formatStr += $".{format},";
                }
                imageConfig.Format = formatStr.Remove(formatStr.Length - 1);

                // 修改全局设置
                GlobalValues.appSetting.Data.Resources.Images = imageConfig;
                GlobalValues.appSetting.Record = recordConfig;
                GlobalValues.appSetting.Pics = picsConfig;
                GlobalValues.appSetting.Notify = notifyConfig;
                GlobalValues.appSetting.Footer = footerConfig;
                GlobalValues.appSetting.Update = updateConfig;

                // 重新加载设置
                // 这里是为了避免当页面销毁时, 全局设置 appSetting 引用为 null
                AppSetting.Save(GlobalValues.appSetting, "appsettings.json");
                GlobalValues.appSetting = AppSetting.Parse();

                _ = _message.Success("设置完成 !");
            }
        }


        /// <summary>
        /// 修改设置失败后调用
        /// </summary>
        /// <param name="editContext"></param>
        async Task OnFinishFailed(EditContext editContext)
        {
            GlobalValues.appSetting = AppSetting.Parse();
            await _message.Error("设置失败 !");
        }


        /// <summary>
        /// 测试邮箱和验证码是否正确
        /// </summary>
        async Task TestEmailConfig()
        {
            emailTestRunning = true;

            Regex emailRegex = new(@"^\w+@\w+.com$");
            if(emailRegex.IsMatch(notifyConfig.Email.From) && (emailRegex.IsMatch(notifyConfig.Email.To)))
            {
                await MailHelper.PostEmails(new MailEntity
                {
                    FromPerson = notifyConfig.Email.From,
                    RecipientArry = notifyConfig.Email.To.Split(","),
                    MailTitle = "ImageBed",
                    MailBody = "这是一封测试邮件, 您的邮箱配置成功 !",
                    Code = notifyConfig.Email.Code,
                    IsBodyHtml = false
                });

                emailTestRunning = false;

                _ = _message.Success("测试邮件已发送, 请注意查收 !");
            }
            else
            {
                _ = _message.Error("测试邮件发送失败, 请检查邮件配置是否正确 !");
            }
        }


        /// <summary>
        /// 检查更新
        /// </summary>
        /// <returns></returns>
        async Task CheckUpdate()
        {
            _ = _message.Info("检查更新中...");

            string latestVersion = await UpdateHelper.GetLatestVersion();
            if(!string.IsNullOrEmpty(latestVersion) && (latestVersion.ToLower() != updateConfig.Version.ToLower()))
            {
                _ = _message.Info($"发现新版本 {latestVersion}, 正在下载更新文件...更新文件下载完成后将关闭该程序");
                await UpdateHelper.SysUpdate(updateConfig.Pattern, $"{UpdateHelper.releaseDownloadBaseUrl}/{latestVersion}/{UpdateHelper.CheckOSPlatform()}.zip");
            }
            else
            {
                _ = _message.Info("已是最新版本, 无需更新! ");
            }
        }


        /// <summary>
        /// 释放页面资源
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            imageConfig = null;
            recordConfig = null;
            picsConfig = null;
            notifyConfig = null;
            footerConfig = null;
            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}
