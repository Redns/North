using North.Common;

namespace North.Pages.Install
{
    partial class Install
    {
        /// <summary>
        /// 背景图片链接
        /// </summary>
        /// TODO 此处为优化加载速度 appsettings.json 中使用固图片链接
        /// Bing 每日一图可制作为插件，api 为 "https://api.xygeng.cn/bing/"
        public string BackgroundImageUrl { get; set; } = string.Empty;


        /// <summary>
        /// 加载完 css 和 js 之后再加载背景图片，优化用户体验
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(() =>
                {
                    BackgroundImageUrl = _appSetting.Appearance.BackgroundUrl;
                    StateHasChanged();
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
