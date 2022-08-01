using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using North.Common;
using North.Data.Access;
using North.Data.Entities;
using North.Models.Auth;
using North.Models.Setting;

namespace North.Pages.Auth
{
    partial class Register
    {
        public bool RegisterRunning { get; set; } = false;
        public RegisterModel RegisterModel { get; set; } = new RegisterModel();
        public RegisterSetting RegisterSettings { get; set; } = GlobalValues.AppSettings.Register;

        protected override async Task OnInitializedAsync()
        {
            if(_accessor.HttpContext?.User?.Identity?.IsAuthenticated is true)
            {
                _navigationManager.NavigateTo("", true);
            }
            await base.OnInitializedAsync();
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
                // 系统未开放注册，仅 Root 用户可注册
                if (!RegisterSettings.AllowRegister)
                {
                    _snackbar.Add("系统当前未开放注册", Severity.Error); return;
                }

                // 核验注册信息
                // 注册信息无误时返回空信息
                var validCheckMessage = RegisterModel.ValidCheck();
                if (!string.IsNullOrEmpty(validCheckMessage))
                {
                    _snackbar.Add(validCheckMessage, Severity.Error); return;
                }

                // 依赖注入方式查找数据库较慢，添加延时以加载动画
                await Task.Delay(500);

                var sqlUserData = new SqlUserData(_context);
                var user = sqlUserData.Get(u => u.Name == RegisterModel.Name || u.Email == RegisterModel.Email).FirstOrDefault();
                if (user is not null)
                {
                    _snackbar.Add("用户名或邮箱已被注册", Severity.Error);
                }
                else if (await sqlUserData.AddAsync(RegisterModel.ToUser()))
                {
                    if(await SendRegisterVerifyEmail())
                    {
                        _snackbar.Add("验证邮件已发送", Severity.Success);
                        _navigationManager.NavigateTo("/login");
                    }
                    else
                    {
                        _snackbar.Add("验证邮件发送失败", Severity.Error);
                    }
                }
                else
                {
                    _snackbar.Add("注册失败", Severity.Error);
                }
            }
            catch (Exception)
            {
                RegisterModel.Avatar =string.Empty;

                _snackbar.Add("注册失败", Severity.Error);
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
        private async Task<bool> SendRegisterVerifyEmail()
        {
            var emailSettings = GlobalValues.AppSettings.Notify.Email;

            // 添加验证邮件至数据库
            var sqlVerifyEmailData = new SqlVerifyEmailData(_context);
            var verifyEmail = new VerifyEmailEntity(IdentifyHelper.GenerateId(), RegisterModel.Email,
                                                    TimeHelper.TimeStamp + emailSettings.ValidTime,
                                                    VerifyType.Register);
            if (await sqlVerifyEmailData.AddAsync(verifyEmail))
            {
                var verifyEmailBody = $"欢迎注册 North 图床，" + 
                                      $"<a href=\"{_navigationManager.BaseUri}verify/register/{verifyEmail.Id}\">点击链接</a> " + 
                                      $"以验证您的账户 {RegisterModel.Name}";
                await MailHelper.SendAsync(new Mail(emailSettings.Account, new string[] { RegisterModel.Email },
                                           "[North]注册验证",
                                           verifyEmailBody,
                                           emailSettings.Code, true));
                return true;
            }
            return false;
        }


        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="args"></param>
        private async Task UploadAvatar(InputFileChangeEventArgs args)
        {
            var avatar = args.GetMultipleFiles()[0];
            var maxAvatarSize = RegisterSettings.MaxAvatarSize * 1024 * 1024;
            if(avatar.Size > maxAvatarSize)
            {
                _snackbar.Add($"头像大小不能超过 {RegisterSettings.MaxAvatarSize} MB", Severity.Error);
            }
            else
            {
                var avatarName = $"{IdentifyHelper.GenerateId()}{Path.GetExtension(avatar.Name)}";
                using (var avatarReadStream = avatar.OpenReadStream(maxAvatarSize))
                {
                    using (var avatarWriteStream = File.Create($"{GlobalValues.AvatarDir}/{avatarName}"))
                    {
                        await avatarReadStream.CopyToAsync(avatarWriteStream);
                        await avatarWriteStream.FlushAsync();
                    }
                    RegisterModel.Avatar = avatarName;
                }
            }
        }


        /// <summary>
        /// 清除头像
        /// </summary>
        private void ClearAvatar()
        {
            try
            {
                File.Delete($"{GlobalValues.AvatarDir}/{RegisterModel.Avatar}");
            }
            catch (Exception)
            {

            }
            finally
            {
                RegisterModel.Avatar = string.Empty;
            }
        }
    }
}
