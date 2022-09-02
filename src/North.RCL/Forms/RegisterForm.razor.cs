using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;

namespace North.RCL.Forms
{
    partial class RegisterForm
    {
        /// <summary>
        /// 图床名称
        /// </summary>
        [Parameter]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 注册按钮回调事件
        /// </summary>
        [Parameter]
        public EventCallback<EditContext> OnRegister { get; set; }

        /// <summary>
        /// 密码框键盘点击回调事件
        /// </summary>
        [Parameter]
        public EventCallback<KeyboardEventArgs> OnEnter { get; set; }

        /// <summary>
        /// 上传头像
        /// </summary>
        [Parameter]
        public EventCallback<InputFileChangeEventArgs> OnAvatarUpload { get; set; }

        /// <summary>
        /// 清初头像
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnAvatarClear { get; set; }

        /// <summary>
        /// 注册进行标志
        /// </summary>
        [Parameter]
        public bool RegisterRunning { get; set; } = false;

        /// <summary>
        /// 登录页面链接
        /// </summary>
        [Parameter]
        public string LoginLink { get; set; } = string.Empty;

        /// <summary>
        /// 组件样式
        /// </summary>
        [Parameter]
        public string Style { get; set; } = "width:90%; max-width:330px; min-width:240px; height:fit-content;";

        /// <summary>
        /// 登录模型
        /// </summary>
        [Parameter]
        public RegisterModel Model { get; set; }
    }


    public class RegisterModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(32, ErrorMessage = "用户名长度不能超过32")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required(ErrorMessage = "请输入邮箱")]
        [StringLength(32, ErrorMessage = "邮箱长度不能超过32")]
        [EmailAddress(ErrorMessage = "邮箱格式错误")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; } = string.Empty;

        /// <summary>
        /// 头像文件类型
        /// </summary>
        public string AvatarContentType { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "密码长度必须介于8 ~ 32之间")]
        public string Password { get; set; } = string.Empty;

        public RegisterModel() { }
        public RegisterModel(string name, string email, string avatar, string avatarContentType, string password)
        {
            Name = name;
            Email = email;
            Avatar = avatar;
            AvatarContentType = avatarContentType;
            Password = password;
        }
    }
}
