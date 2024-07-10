namespace North.Shared
{
    partial class AuthLayout
    {
        /// <summary>
        /// 背景图片链接
        /// </summary>
        public string BackgroundImageUrl { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            if (!_appSetting.Appearance.ImageLazyLoad)
            {
                BackgroundImageUrl = _appSetting.Appearance.BackgroundUrl;
            }
            await base.OnInitializedAsync();
        }

        /// <summary>
        /// 背景图片懒加载
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (_appSetting.Appearance.ImageLazyLoad)
                {
                    await InvokeAsync(() =>
                    {
                        BackgroundImageUrl = _appSetting.Appearance.BackgroundUrl;
                        StateHasChanged();
                    });
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
