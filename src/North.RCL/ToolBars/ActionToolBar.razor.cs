using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace North.RCL.ToolBars
{
    partial class ActionToolBar
    {
        [Parameter]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 侧边导航栏是否展开
        /// </summary>
        [Parameter]
        public bool IsExpanded { get; set; }
        public string ExpandTooltip => IsExpanded ? "收起" : "展开";

        /// <summary>
        /// 深色主题
        /// </summary>
        [Parameter]
        public bool DarkTheme { get; set; } = false;

        /// <summary>
        /// 切换主题按钮图标
        /// </summary>
        public string ThemeSwitchIcon => DarkTheme ? Icons.Material.Outlined.WbSunny : Icons.Outlined.DarkMode;

        /// <summary>
        /// 切换主题按钮提示
        /// </summary>
        public string ThemeSwitchTooltip => DarkTheme ? "切换至浅色主题" : "切换至深色主题";

        /// <summary>
        /// 侧边导航栏状态更改回调函数
        /// </summary>
        [Parameter]
        public EventCallback OnNavMenuStateChanged { get; set; }

        /// <summary>
        /// 切换主题
        /// </summary>
        /// <returns></returns>
        [Parameter]
        public EventCallback<MouseEventArgs> OnSwitchTheme { get; set; }

        /// <summary>
        /// 退出登录
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnUserClick { get; set; }

        [Parameter]
        public string Style { get; set; } = string.Empty;
    }
}
