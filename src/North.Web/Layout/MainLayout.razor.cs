using FastEnumUtility;
using MudBlazor;
using North.RCL.ToolBars;
using North.Web.Common;

namespace North.Web.Layout
{
    public partial class MainLayout
    {
        private MudThemeProvider? _mudThemeProvider;

        /// <summary>
        /// PWA 更新内容
        /// </summary>
        public string ReleaseNotesText { get; set; } = "检测到新版本";

        /// <summary>
        /// TODO 通过 API 获取应用名称
        /// </summary>
        public string AppName { get; set; } = "North";

        /// <summary>
        /// TODO 通过 API 获取页脚信息
        /// </summary>
        public string FooterText { get; set; } =
            @"<a href=""https://github.com/Redns/ImageBed"" target=""_blank"" style=""color: rgba(0, 164, 255, 1); font-size: 16px"">Powered by North © 2022-2026 Redns, All rights reserved</a>";

        /// <summary>
        /// 鼠标悬浮时自动展开侧边栏
        /// TODO 通过 API 获取用户设置
        /// </summary>
        public bool OpenMiniOnHover { get; set; } = false;

        /// <summary>
        /// 侧边导航栏是否展开
        /// </summary>
        public bool IsExpanded { get; set; } = false;

        /// <summary>
        /// 系统是否为深度模式
        /// </summary>
        public bool IsSystemDarkMode { get; set; } = false;

        /// <summary>
        /// 当前主题模式（点击切换：系统 -> 深色 -> 浅色 -> 系统）
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

            // PWA 更新事件
            PWAUpdaterService.NextVersionIsWaiting += OnUpdateReady;

            // 加载本地主题模式
            var themeModeString = await localStorageService.GetItemAsStringAsync(
                GlobalValues.LOCAL_STORAGE_KEY_THEME_MODE
            );

            if (string.IsNullOrEmpty(themeModeString))
            {
                await localStorageService.SetItemAsStringAsync(
                    GlobalValues.LOCAL_STORAGE_KEY_THEME_MODE,
                    ThemeMode.FastToString()
                );
            }
            else
            {
                ThemeMode = FastEnum.Parse<ThemeMode>(themeModeString);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender && _mudThemeProvider != null)
            {
                // 获取系统主题
                IsSystemDarkMode = await _mudThemeProvider.GetSystemDarkModeAsync();
                // 监听系统主题的变化
                await _mudThemeProvider.WatchSystemDarkModeAsync(async isDarkMode =>
                {
                    IsSystemDarkMode = isDarkMode;
                    if (ThemeMode is ThemeMode.System)
                    {
                        await InvokeAsync(StateHasChanged);
                    }
                });
            }
        }

        /// <summary>
        /// 切换主题
        /// </summary>
        public async Task SwitchTheme()
        {
            ThemeMode = ThemeMode switch
            {
                ThemeMode.System => ThemeMode.Dark,
                ThemeMode.Dark => ThemeMode.Light,
                ThemeMode.Light => ThemeMode.System,
                _ => ThemeMode.System,
            };

            await localStorageService.SetItemAsStringAsync(
                GlobalValues.LOCAL_STORAGE_KEY_THEME_MODE,
                ThemeMode.FastToString()
            );
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        public void SignOut()
        {
            navigationManager.NavigateTo("signout", true);
        }

        private async void OnUpdateReady(object? sender, EventArgs e)
        {
            try
            {
                ReleaseNotesText = await httpClientFactory
                    .CreateClient(nameof(MainLayout))
                    .GetStringAsync("data/release_notes.txt");
            }
            catch
            {
                ReleaseNotesText = "检测到新版本";
            }
        }

        public void Dispose()
        {
            PWAUpdaterService.NextVersionIsWaiting -= OnUpdateReady;
        }
    }
}
