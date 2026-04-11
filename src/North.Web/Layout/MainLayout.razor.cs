namespace North.Web.Layout
{
    public partial class MainLayout
    {
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
            @"<a href=""https://github.com/Redns/North"" target=""_blank"" style=""color: rgba(0, 164, 255, 1); font-size: 16px"">Powered by North © 2022-2026 Redns, All rights reserved</a>";

        /// <summary>
        /// 鼠标悬浮时自动展开侧边栏
        /// </summary>
        public bool OpenMiniOnHover { get; set; } = false;

        /// <summary>
        /// 侧边导航栏是否展开
        /// </summary>
        public bool IsExpanded { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            PWAUpdaterService.NextVersionIsWaiting += OnUpdateReady;
            await ThemeState.InitializeAsync();
        }

        /// <summary>
        /// 切换主题
        /// </summary>
        public async Task SwitchTheme()
        {
            await ThemeState.SwitchThemeAsync();
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
            GC.SuppressFinalize(this);
        }
    }
}
