using North.Common;

namespace North.Pages.Install
{
    partial class Install
    {
        /// <summary>
        /// 背景图片链接
        /// </summary>
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
                    BackgroundImageUrl = GlobalValues.AppSettings.Appearance.BackgroundUrl;
                    StateHasChanged();
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
