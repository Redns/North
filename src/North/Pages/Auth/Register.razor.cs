using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using North.Common;
using North.Data.Access;
using North.Data.Entities;
using System.Text.RegularExpressions;

namespace North.Pages.Auth
{
    partial class Register
    {
        public bool RegisterRunning { get; set; } = false;
        public RegisterModel RegisterModel { get; set; } = new RegisterModel();

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
                var validCheckMessage = RegisterModel.ValidCheck();
                if (!string.IsNullOrEmpty(validCheckMessage))
                {
                    _snackbar.Add(validCheckMessage, Severity.Error);
                }
                else
                {
                    await Task.Delay(500);

                    var sqlUserData = new SqlUserData(_context);
                    var user = sqlUserData.Get(u => u.Name == RegisterModel.Name || u.Email == RegisterModel.Email).FirstOrDefault();
                    if (user is not null)
                    {
                        _snackbar.Add("用户名或邮箱已被注册", Severity.Error);
                    }
                    else if (await sqlUserData.AddAsync(RegisterModel.ToUser()))
                    {
                        _snackbar.Add("验证邮件已发送", Severity.Success);
                        _navigationManager.NavigateTo("/login");
                    }
                    else
                    {
                        _snackbar.Add("注册失败", Severity.Error);
                    }
                }
            }
            catch (Exception)
            {
                RegisterModel.Avatar =string.Empty;
                _snackbar.Add("注册失败", Severity.Error);
            }

            RegisterRunning = false;
        }


        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="args"></param>
        private async Task UploadAvatar(InputFileChangeEventArgs args)
        {
            var avatar = args.GetMultipleFiles()[0];
            var maxAvatarSize = GlobalValues.AppSettings.Register.MaxAvatarSize * 1024 * 1024;
            if(avatar.Size > maxAvatarSize)
            {
                _snackbar.Add($"头像大小不能超过 {GlobalValues.AppSettings.Register.MaxAvatarSize} MB", Severity.Error);
            }
            else
            {
                RegisterModel.Avatar = $"{IdentifyHelper.GenerateId()}{Path.GetExtension(avatar.Name)}";

                using var avatarReadStream = avatar.OpenReadStream(maxAvatarSize);
                using var avatarWriteStream = File.OpenWrite($"{GlobalValues.AvatarDir}/{RegisterModel.Avatar}");
                await avatarReadStream.CopyToAsync(avatarWriteStream);
                await avatarReadStream.FlushAsync();
                await avatarWriteStream.FlushAsync();
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


    public class RegisterModel
    {
        public string Name { get; set; }            // 用户名
        public string Email { get; set; }           // 邮箱
        public string Avatar { get; set; }          // 头像（xxx.xx）
        public string Password { get; set; }        // 密码

        public RegisterModel()
        {
            Name = string.Empty;
            Email = string.Empty;
            Avatar = string.Empty;
            Password = string.Empty;
        }

        public RegisterModel(string name, string email, string avatar, string password)
        {
            Name = name;
            Email = email;
            Avatar = avatar;
            Password = password;
        }


        /// <summary>
        /// 验证注册信息
        /// </summary>
        /// <returns>提示信息</returns>
        public string ValidCheck()
        {
            if(!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Avatar) && !string.IsNullOrEmpty(Password))
            {
                if(!new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$").IsMatch(Email))
                {
                    return "邮箱格式错误";
                }
                return string.Empty;
            }
            return "注册信息不能为空";
        }


        /// <summary>
        /// 生成注册用户
        /// </summary>
        /// <returns></returns>
        public UserEntity ToUser()
        {
            var registerDefaultSettings = GlobalValues.AppSettings.Register.Default;
            return new UserEntity(IdentifyHelper.GenerateId(),
                                  Name,
                                  Email,
                                  EncryptHelper.MD5($"{Name}:{Password}"),
                                  $"api/image/avatar/{Avatar}",
                                  State.Checking,
                                  string.Empty,
                                  0L,
                                  registerDefaultSettings.Permission,
                                  registerDefaultSettings.IsApiAvailable,
                                  registerDefaultSettings.MaxUploadNums,
                                  registerDefaultSettings.MaxUploadCapacity,
                                  registerDefaultSettings.SingleMaxUploadNums,
                                  registerDefaultSettings.SingleMaxUploadCapacity);
        }
    }
}
