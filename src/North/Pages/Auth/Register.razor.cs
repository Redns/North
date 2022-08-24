using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MimeKit;
using MudBlazor;
using North.Common;
using North.Core.Entities;
using North.Core.Helper;
using North.Data.Access;
using North.Models.Auth;
using North.Models.Notification;

namespace North.Pages.Auth
{
    partial class Register
    {
        /// <summary>
        /// 注册进行中标志
        /// </summary>
        public bool RegisterRunning { get; set; } = false;

        /// <summary>
        /// 背景图片链接
        /// </summary>
        public string BackgroundImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 注册模型
        /// </summary>
        public RegisterModel RegisterModel { get; set; } = new RegisterModel();

        /// <summary>
        /// 系统注册设置
        /// </summary>
        public RegisterSetting RegisterSettings { get; set; } = GlobalValues.AppSettings.Register;


        /// <summary>
        /// 加载完 css 和 js 之后再加载背景图片，优化用户体验
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(() =>
                {
                    BackgroundImageUrl = GlobalValues.AppSettings.Appearance.BackgroundUrl;
                    StateHasChanged();
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        /// <summary>
        /// 用户注册
        /// </summary>
        /// <returns></returns>
        private async Task UserRegister()
        {
            RegisterRunning = true;

            try
            {
                if (!RegisterSettings.AllowRegister)
                {
                    _snackbar.Add("系统当前未开放注册", Severity.Error); return; 
                }

                // 校验用户输入
                var validCheckMessage = RegisterModel.ValidCheck();
                if (!string.IsNullOrEmpty(validCheckMessage))
                {
                    _snackbar.Add(validCheckMessage, Severity.Error);
                }
                else
                {
                    await Task.Delay(500);

                    using var context = new OurDbContext();
                    var users = new SqlUserData(context);
                    if (users.Any(u => u.Email == RegisterModel.Email))
                    {
                        _snackbar.Add("邮箱已被注册", Severity.Error);
                    }
                    else
                    {
                        // TODO 此处后期根据 AppSetting 中的设置项确定保存路径
                        var newUser = RegisterModel.ToUser();
                        var avatarMaxSize = RegisterSettings.MaxAvatarSize * 1024 * 1024;
                        var avatarName = $"{IdentifyHelper.Generate()}.{RegisterModel.AvatarExtension}";

                        // 保存用户头像
                        await JS.DownloadBlob(RegisterModel.Avatar, $"Data/Images/{newUser.Id}/{avatarName}", (long)avatarMaxSize);

                        // 发送验证邮件
                        await SendRegisterVerifyEmail();

                        // 录入用户
                        newUser.Avatar = $"api/image/{newUser.Id}/{avatarName}";
                        users.Add(newUser);

                        _snackbar.Add("验证邮件已发送", Severity.Success);
                        _navigationManager.NavigateTo("login");
                    }
                }
            }
            catch (Exception e)
            {
                await JS.DestroyBlob(RegisterModel.Avatar);

                RegisterModel.Avatar = string.Empty;
                RegisterModel.AvatarExtension = string.Empty;

                _logger.Error("Register failed", e);
                _snackbar.Add("注册失败，系统内部错误", Severity.Error);
            }
            finally
            {
                RegisterRunning = false;
            } 
        }


        private async Task EnterToRegister(KeyboardEventArgs args)
        {
            if(args.Code is "Enter")
            {
                await UserRegister();
            }
        }


        /// <summary>
        /// 发送注册验证邮件
        /// </summary>
        /// <returns></returns>
        private async ValueTask SendRegisterVerifyEmail()
        {
            var emailSettings = GlobalValues.AppSettings.Notify.Email;

            // 添加验证邮件至数据库
            var verifyEmail = new VerifyEmailEntity(IdentifyHelper.Generate(), RegisterModel.Email,
                                                    IdentifyHelper.TimeStamp + RegisterSettings.VerifyEmailValidTime,
                                                    VerifyType.Register);
            using var context = new OurDbContext();
            await new SqlVerifyEmailData(context).AddAsync(verifyEmail);

            // 构造验证邮件并发送
            var verifyEmailBody = $"欢迎注册 North 图床，" +
                                  $"<a href=\"{_navigationManager.BaseUri}verify?type=register&id={verifyEmail.Id}\">点击链接</a> " +
                                  $"以验证您的账户 {RegisterModel.Name}";
            await new Mail(new MailboxAddress("North", emailSettings.Account),
                           new MailboxAddress(RegisterModel.Name, RegisterModel.Email),
                           "North 图床注册验证",
                           verifyEmailBody,
                           emailSettings.Code,
                           true).SendAsync();
        }


        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="args"></param>
        private async Task UploadAvatar(InputFileChangeEventArgs args)
        {
            try
            {
                var avatar = args.GetMultipleFiles()[0];
                var avatarMaxSize = RegisterSettings.MaxAvatarSize * 1024 * 1024;
                if (avatar.Size > avatarMaxSize)
                {
                    _snackbar.Add($"头像大小不能超过 {RegisterSettings.MaxAvatarSize} MB", Severity.Error);
                }
                else
                {
                    using var avatarReadStream = avatar.OpenReadStream((long)avatarMaxSize);
                    using var avatarReadStreamRef = new DotNetStreamReference(avatarReadStream);

                    RegisterModel.Avatar = await JS.UploadToBlob(avatarReadStream, avatar.ContentType);
                    RegisterModel.AvatarExtension = avatar.ContentType.Split('/').Last();
                }
            }
            catch(Exception e)
            {
                _logger.Error("Avatar upload failed", e);
            }
        }


        /// <summary>
        /// 清除头像
        /// </summary>
        private async Task ClearAvatar()
        {
            if (!string.IsNullOrEmpty(RegisterModel.Avatar))
            {
                await JS.DestroyBlob(RegisterModel.Avatar);

                RegisterModel.Avatar = string.Empty;
                RegisterModel.AvatarExtension = string.Empty;
            }
        }
    }
}
