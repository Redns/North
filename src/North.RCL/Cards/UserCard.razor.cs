using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using North.Core.Entities;

namespace North.RCL.Cards
{
    partial class UserCard
    {
        /// <summary>
        /// 用户实体
        /// </summary>
        [Parameter]
        public UserEntity User { get; set; }

        [Parameter]
        public string Style { get; set; } = string.Empty;

        /// <summary>
        /// 编辑用户
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnEdit { get; set; }

        /// <summary>
        /// 删除用户
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnDelete { get; set; }

        /// <summary>
        /// 用户状态标志颜色
        /// </summary>
        public Color UserStateColor => User.State switch
        {
            UserState.Checking => Color.Warning,
            UserState.Normal => Color.Success,
            _ => Color.Error
        };

        /// <summary>
        /// 用户权限标签文本
        /// </summary>
        public string PermissionChipText => User.Permission switch
        {
            UserPermission.System => "系统",
            UserPermission.User => "用户",
            _ => "未知"
        };

        /// <summary>
        /// 用户权限标签颜色
        /// </summary>
        public Color PermissionChipColor => User.Permission switch
        {
            UserPermission.System => Color.Success,
            UserPermission.User => Color.Info,
            _ => Color.Error
        };

        /// <summary>
        /// 用户 API 访问标签颜色
        /// </summary>
        public Color ApiChipColor => User.IsApiAvailable ? Color.Success : Color.Error;

        public string UploadLimitText => $"{User.SingleMaxUploadNums}张 / {User.SingleMaxUploadSize:f2} MB (单次)，{User.MaxUploadNums}张 / {User.MaxUploadSize:f2} MB (全部)";
    }
}
