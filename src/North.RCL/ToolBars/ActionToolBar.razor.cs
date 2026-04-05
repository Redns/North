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

        /// <summary>
        /// 深色主题
        /// </summary>
        [Parameter]
        public ThemeMode ThemeMode { get; set; } = ThemeMode.System;

        /// <summary>
        /// 切换主题按钮图标（系统 -> 深色 -> 浅色 -> 系统）
        /// </summary>
        public string ThemeSwitchIcon =>
            ThemeMode switch
            {
                ThemeMode.System => Icons.Material.Outlined.DarkMode,
                ThemeMode.Dark => Icons.Material.Outlined.WbSunny,
                ThemeMode.Light => Icons.Material.Outlined.AutoMode,
                _ => Icons.Material.Outlined.AutoMode,
            };

        /// <summary>
        /// 切换主题按钮提示
        /// </summary>
        public string ThemeSwitchTooltip =>
            ThemeMode switch
            {
                ThemeMode.System => "切换至深色模式",
                ThemeMode.Light => "切换至跟随模式",
                ThemeMode.Dark => "切换至浅色模式",
                _ => "跟随系统",
            };

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

    public enum ThemeMode
    {
        Light,
        Dark,
        System,
    }
}
