using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;

namespace North.RCL.Forms
{
    /// <summary>
    /// 登录表单
    /// </summary>
    partial class LoginForm
    {
        /// <summary>
        /// 图床名称
        /// </summary>
        [Parameter]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 登录按钮回调事件
        /// </summary>
        [Parameter]
        public EventCallback<EditContext> OnLogin { get; set; }

        /// <summary>
        /// 密码框键盘点击回调事件
        /// </summary>
        [Parameter]
        public EventCallback<KeyboardEventArgs> OnEnter { get; set; }

        /// <summary>
        /// 登录进行标志
        /// </summary>
        [Parameter]
        public bool LoginRunning { get; set; } = false;

        /// <summary>
        /// 注册页面链接
        /// </summary>
        [Parameter]
        public string RegisterLink { get; set; } = string.Empty;

        /// <summary>
        /// 找回密码页面链接
        /// </summary>
        [Parameter]
        public string RetrievePasswordLink { get; set; } = string.Empty;

        /// <summary>
        /// 组件样式
        /// </summary>
        [Parameter]
        public string Style { get; set; } = "width:90%; max-width:330px; min-width:240px; height:fit-content;";

        /// <summary>
        /// 登录模型
        /// </summary>
        [Parameter]
        public LoginModel Model { get; set; }
    }


    /// <summary>
    /// 登录模型
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        [Required(ErrorMessage = "请输入邮箱")]
        [StringLength(32, ErrorMessage = "邮箱长度不能超过32")]
        [EmailAddress(ErrorMessage = "邮箱格式错误")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "密码长度必须介于8 ~ 32之间")]
        public string Password { get; set; } = string.Empty;

        public LoginModel() { }
        public LoginModel(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
