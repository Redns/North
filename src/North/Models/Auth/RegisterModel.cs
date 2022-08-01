using North.Common;
using North.Data.Entities;
using System.Text.RegularExpressions;

namespace North.Models.Auth
{
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
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Avatar) && !string.IsNullOrEmpty(Password))
            {
                if (!new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$").IsMatch(Email))
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
