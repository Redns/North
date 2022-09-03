using Microsoft.AspNetCore.Components;
using MudBlazor;
using North.Core.Helpers;

namespace North.Shared
{
    partial class ActionBar
    {
        [Parameter]
        public string Style { get; set; } = string.Empty;

        [Parameter]
        public EventCallback OnNavMenuStateChanged { get; set; }

        [Parameter]
        public bool IsExpanded { get; set; }
        public string ExpandTooltip => IsExpanded ? "收起" : "展开";

        /// <summary>
        /// 深色主题
        /// </summary>
        public bool DarkTheme { get; set; } = false;

        /// <summary>
        /// 切换主题按钮图标
        /// </summary>
        public string ThemeSwitchIcon => DarkTheme ? Icons.Outlined.WbSunny : Icons.Outlined.DarkMode;

        /// <summary>
        /// 切换主题按钮提示
        /// </summary>
        public string ThemeSwitchTooltip => DarkTheme ? "切换至浅色主题" : "切换至深色主题"; 


        /// <summary>
        /// 切换主题
        /// </summary>
        /// <returns></returns>
        public async Task SwitchTheme()
        {
            DarkTheme = !DarkTheme;
            if (DarkTheme)
            {
                await JS.SetBodyStyle("#111", "invert(1) hue-rotate(180deg)");
            }
            else
            {
                await JS.SetBodyStyle(string.Empty, string.Empty);
            }
        }


        /// <summary>
        /// 退出登录
        /// </summary>
        public void SignOut()
        {
            _nav.NavigateTo("signout", true);
        }
    }
}
