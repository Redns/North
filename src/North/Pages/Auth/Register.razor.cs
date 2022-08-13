using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MimeKit;
using MudBlazor;
using North.Common;
using North.Core.Data.Entities;
using North.Core.Helper;
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
        /// 注册模型
        /// </summary>
        public RegisterModel RegisterModel { get; set; } = new RegisterModel();

        /// <summary>
        /// 系统注册设置
        /// </summary>
        public RegisterSetting RegisterSettings { get; set; } = GlobalValues.AppSettings.Register;


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
                    if (GlobalValues.MemoryDatabase.Users.FirstOrDefault(u => u.Email == RegisterModel.Email) is not null)
                    {
                        _snackbar.Add("邮箱已被注册", Severity.Error);
                    }
                    else
                    {
                        // TODO 此处后期根据 AppSetting 中的设置项确定保存路径
                        var newUser = RegisterModel.ToUser();
                        var avatarName = $"{IdentifyHelper.Generate()}.{RegisterModel.AvatarExtension}";

                        // 保存用户头像
                        await DownloadBlob(RegisterModel.Avatar,
                                           $"Data/Images/{newUser.Id}/{avatarName}",
                                           (long)RegisterSettings.MaxAvatarSize * 1024 * 1024);

                        // 发送验证邮件
                        await SendRegisterVerifyEmail();

                        // 录入用户
                        newUser.Avatar = $"api/image/{newUser.Id}/{avatarName}";
                        GlobalValues.MemoryDatabase.Users.Add(newUser);

                        _snackbar.Add("验证邮件已发送", Severity.Success);
                        _navigationManager.NavigateTo("login");
                    }
                }
            }
            catch (Exception e)
            {
                await DestroyBlob(RegisterModel.Avatar);

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


        /// <summary>
        /// 发送注册验证邮件
        /// </summary>
        /// <returns></returns>
        private async ValueTask SendRegisterVerifyEmail()
        {
            var emailSettings = GlobalValues.AppSettings.Notify.Email;

            // 添加验证邮件至数据库
            var verifyEmail = new VerifyEmailEntity(IdentifyHelper.Generate(), RegisterModel.Email,
                                                    IdentifyHelper.TimeStamp + emailSettings.ValidTime,
                                                    VerifyType.Register);
            GlobalValues.MemoryDatabase.VerifyEmails.Add(verifyEmail);

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
                if ((ulong)avatar.Size > avatarMaxSize)
                {
                    _snackbar.Add($"头像大小不能超过 {RegisterSettings.MaxAvatarSize} MB", Severity.Error);
                }
                else
                {
                    using var avatarReadStream = avatar.OpenReadStream((long)avatarMaxSize);
                    using var avatarReadStreamRef = new DotNetStreamReference(avatarReadStream);

                    RegisterModel.Avatar = await JS.InvokeAsync<string>("upload", avatarReadStreamRef, avatar.ContentType);
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
                RegisterModel.Avatar = string.Empty;
                RegisterModel.AvatarExtension = string.Empty;

                await DestroyBlob(RegisterModel.Avatar);
            }
        }


        /// <summary>
        /// 下载浏览器 Blob 文件
        /// </summary>
        /// <example>https://docs.microsoft.com/zh-cn/aspnet/core/blazor/javascript-interoperability/call-dotnet-from-javascript?view=aspnetcore-6.0#stream-from-javascript-to-net</example>
        /// <param name="url">文件链接</param>
        /// <param name="path">保存路径</param>
        /// <param name="maxAllowedSize">JavaScript 中读取操作允许的最大字节数（默认：512000字节）</param>
        /// <returns></returns>
        private async ValueTask DownloadBlob(string url, string path, long maxAllowedSize = 512000)
        {
            // 获取 Blob 文件流
            using var dataReferenceStream = await GetBlobStream(url, maxAllowedSize);

            // 创建文件夹
            var rootDirectory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(rootDirectory))
            {
                Directory.CreateDirectory(rootDirectory);
            }
            using var avatarWriteStream = File.OpenWrite(path);
            await dataReferenceStream.CopyToAsync(avatarWriteStream);
        }


        /// <summary>
        /// 获取浏览器 Blob 文件数据流
        /// </summary>
        /// <param name="url">文件链接</param>
        /// <param name="maxAllowedSize">JavaScript 中读取操作允许的最大字节数（默认：512000字节）</param>
        /// <returns></returns>
        private async ValueTask<Stream> GetBlobStream(string url, long maxAllowedSize = 512000)
        {
            var blobReadStreamRef = await JS.InvokeAsync<IJSStreamReference>("getBlobStream", url);
            return await blobReadStreamRef.OpenReadStreamAsync(maxAllowedSize);
        }


        /// <summary>
        /// 销毁浏览器 Blob 对象
        /// </summary>
        /// <param name="url">对象链接</param>
        /// <returns></returns>
        private async ValueTask DestroyBlob(string url)
        {
            await JS.InvokeVoidAsync("destroy", url);
        }
    }
}
