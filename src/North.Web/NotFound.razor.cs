using FastEnumUtility;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using North.RCL.ToolBars;
using North.Web.Common;

namespace North.Web
{
    public partial class NotFound
    {
        private MudThemeProvider? _mudThemeProvider;

        /// <summary>
        /// 页脚信息
        /// </summary>
        [Parameter]
        public string FooterText { get; set; } = string.Empty;

        /// <summary>
        /// 系统是否为深度模式
        /// </summary>
        public bool IsSystemDarkMode { get; set; } = false;

        /// <summary>
        /// 当前主题模式（仅用于获取全局页面设置，禁止在本组件做其他修改）
        /// </summary>
        public ThemeMode ThemeMode { get; set; } = ThemeMode.System;

        /// <summary>
        /// 网页主题模式
        /// </summary>
        public bool IsDarkMode =>
            ThemeMode is ThemeMode.Dark || (ThemeMode is ThemeMode.System && IsSystemDarkMode);

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // 加载本地主题模式
            var themeModeString = await localStorageService.GetItemAsStringAsync(
                GlobalValues.LOCAL_STORAGE_KEY_THEME_MODE
            );

            if (!string.IsNullOrEmpty(themeModeString))
            {
                ThemeMode = FastEnum.Parse<ThemeMode>(themeModeString);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender && _mudThemeProvider != null && ThemeMode is ThemeMode.System)
            {
                // 获取系统主题
                IsSystemDarkMode = await _mudThemeProvider.GetSystemDarkModeAsync();
                // 监听系统主题的变化
                await _mudThemeProvider.WatchSystemDarkModeAsync(async isDarkMode =>
                {
                    IsSystemDarkMode = isDarkMode;
                    await InvokeAsync(StateHasChanged);
                });
            }
        }
    }
}
