using North.Core.Entities;
using North.Core.Helpers;
using System.Text.RegularExpressions;

namespace North.Core.Models.Auth
{
    public class RegisterModel
    {
        public string Name { get; set; }            // 用户名
        public string Email { get; set; }           // 邮箱
        public string Avatar { get; set; }          // 头像（xxx.xx）
        public string AvatarExtension { get; set; } // 头像后缀（不带 '.'）
        public string Password { get; set; }        // 密码

        public RegisterModel()
        {
            Name = string.Empty;
            Email = string.Empty;
            Avatar = string.Empty;
            AvatarExtension = string.Empty;
            Password = string.Empty;
        }

        public RegisterModel(string name, string email, string avatar, string avatarExtension, string password)
        {
            Name = name;
            Email = email;
            Avatar = avatar;
            AvatarExtension = avatarExtension;
            Password = password;
        }


        /// <summary>
        /// 验证注册信息
        /// </summary>
        /// <returns>提示信息</returns>
        public string ValidCheck()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "用户名不能为空";
            }
            else if (string.IsNullOrEmpty(Email))
            {
                return "邮箱不能为空";
            }
            else if (string.IsNullOrEmpty(Password))
            {
                return "密码不能为空";
            }
            else if (string.IsNullOrEmpty(Avatar))
            {
                return "头像不能为空";
            }
            else if (!new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$").IsMatch(Email))
            {
                return "邮箱格式错误";
            }
            return string.Empty;
        }


        /// <summary>
        /// 生成注册用户
        /// </summary>
        /// <returns></returns>
        public UserEntity ToUser(RegisterSettingDefault @default)
        {
            return new UserEntity(IdentifyHelper.Generate(),
                                  Name,
                                  Email,
                                  $"{Email}:{Password}".MD5(),
                                  $"api/image/avatar/{Avatar}",
                                  State.Checking,
                                  @default.Permission,
                                  @default.IsApiAvailable,
                                  @default.MaxUploadNums,
                                  @default.MaxUploadCapacity,
                                  @default.SingleMaxUploadNums,
                                  @default.SingleMaxUploadCapacity);
        }
    }


    /// <summary>
    /// 默认注册设置
    /// </summary>
    public class RegisterSettingDefault
    {
        public Permission Permission { get; set; }          // 用户权限
        public bool IsApiAvailable { get; set; }            // 是否启用 API
        public long MaxUploadNums { get; set; }            // 最大上传数量（张）
        public double MaxUploadCapacity { get; set; }        // 最大上传容量（MB）
        public long SingleMaxUploadNums { get; set; }      // 单次最大上传数量（张）
        public double SingleMaxUploadCapacity { get; set; }  // 单次最大上传容量（MB）

        public RegisterSettingDefault(Permission permission, bool isApiAvailable, long maxUploadNums, double maxUploadCapacity, long singleMaxUploadNums, double singleMaxUploadCapacity)
        {
            Permission = permission;
            IsApiAvailable = isApiAvailable;
            MaxUploadNums = maxUploadNums;
            MaxUploadCapacity = maxUploadCapacity;
            SingleMaxUploadNums = singleMaxUploadNums;
            SingleMaxUploadCapacity = singleMaxUploadCapacity;
        }

        public RegisterSettingDefault Clone() => new(Permission, IsApiAvailable, MaxUploadNums, MaxUploadCapacity, SingleMaxUploadNums, SingleMaxUploadCapacity);
    }
}
