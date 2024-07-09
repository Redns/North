using Masuit.Tools.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using North.Core.Entities;
using North.Core.Helpers;
using North.Core.Repository;
using North.RCL.Forms;

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
        public RegisterModel Model { get; set; } = new RegisterModel();

        /// <summary>
        /// 检查系统是否开放注册
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            if (!_appSetting.Register.AllowRegister)
            {
                _nav.NavigateTo("login", true);
            }
            await base.OnInitializedAsync();
        }


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
                    BackgroundImageUrl = _appSetting.Appearance.BackgroundUrl;
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
                // 判断用户是否上传头像
                await Task.Delay(500);
                if (string.IsNullOrEmpty(Model.Avatar))
                {
                    _snackbar.Add("头像不能为空", Severity.Error); return;
                }

                // 判断账号是否已被注册
                var userRepository = new UserRepository(_client, _appSetting.General.DataBase.EnabledName);
                if (await userRepository.AnyAsync(u => u.Email == Model.Email))
                {
                    _snackbar.Add("邮箱已被注册", Severity.Error); return;
                }

                // TODO 添加注册逻辑
                await userRepository.AddAsync(new UserEntity
                {
                    Name = Model.Name,
                    Email = Model.Email,
                    Password = Model.EncryptedPassword,
                    LastModifyTime = DateTime.Now
                });

                _nav.NavigateTo("login", true);
            }
            catch (Exception e)
            {
                await ClearAvatar();
                _logger.Error("Register failed", e);
                _snackbar.Add("注册失败，系统内部错误", Severity.Error);
            }
            finally
            {
                RegisterRunning = false;
            } 
        }


        /// <summary>
        /// 监测 Enter 键
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
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
        private async Task SendRegisterVerifyEmail()
        {
            var emailSettings = _appSetting.Notify.Email;

            // 添加验证邮件至数据库
            var verifyEmail = new EmailEntity 
            {
                Email = Model.Email,
                ExpireTime = DateTime.Now.AddMilliseconds(_appSetting.Register.VerifyEmailValidTime),
                VerifyType = VerifyType.Register
            };
            await new EmailRepository(_client, _appSetting.General.DataBase.EnabledName).AddAsync(verifyEmail);

            // 构造验证邮件并发送
            var verifyEmailBody = $"欢迎注册 {_appSetting.Appearance.Name} 图床，" +
                                  $"<a href=\"{_nav.BaseUri}verify?type=register&id={verifyEmail.Id}\">点击链接</a> " +
                                  $"以验证您的账户 {Model.Name}";
            new Email()
            {
                SmtpServer = $"smtp.{emailSettings.Account.Split(new char[] { '@', '.' })[1]}.com",
                SmtpPort = 465,
                EnableSsl = true,
                Username = emailSettings.Account,
                Password = emailSettings.Code,
                Tos = Model.Email,
                Subject = $"{_appSetting.Appearance.Name} 图床注册验证",
                Body = verifyEmailBody
            }.SendAsync(s =>
            {

            });
        }


        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="args"></param>
        private async Task UploadAvatar(InputFileChangeEventArgs args)
        {
            try
            {
                var avatar = args.File;
                var avatarMaxSize = _appSetting.Register.MaxAvatarSize * 1024 * 1024;
                if (avatar.Size > avatarMaxSize)
                {
                    _snackbar.Add($"头像大小不能超过 {_appSetting.Register.MaxAvatarSize} MB", Severity.Error); return;
                }

                // 上传图片至 Blob
                using var avatarReadStream = avatar.OpenReadStream((long)avatarMaxSize);
                using var avatarReadStreamRef = new DotNetStreamReference(avatarReadStream);
                Model.Avatar = await JS.UploadToBlobAsync(avatarReadStream, avatar.ContentType);
                Model.AvatarContentType = avatar.ContentType;
            }
            catch(Exception e)
            {
                _logger.Error("Fail to upload avatar", e);
                _snackbar.Add("头像上传失败，服务器内部错误", Severity.Error);
            }
        }


        /// <summary>
        /// 清除头像
        /// </summary>
        private async Task ClearAvatar()
        {
            if (!string.IsNullOrEmpty(Model.Avatar))
            {
                await JS.DestroyBlobAsync(Model.Avatar);
                Model.Avatar = string.Empty;
                Model.AvatarContentType = string.Empty;
            }
        }
    }
}
