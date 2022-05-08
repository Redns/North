using Microsoft.JSInterop;

namespace ImageBed.Shared
{
    partial class MainLayout
    {
        bool showFooter = false;        // 是否显示页脚


        /// <summary>
        /// 页面渲染完成后调用
        /// </summary>
        /// <param name="firstRender">是否为第一次渲染</param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender && !showFooter)
            {
                if (await GetScreenWidth() > 500)
                {
                    showFooter = true;
                    StateHasChanged();
                }
            }
        }


        /// <summary>
        /// 获取屏幕宽度（单位：px）
        /// </summary>
        /// <returns></returns>
        private async Task<double> GetScreenWidth()
        {
            return await JS.InvokeAsync<double>("GetScreenWidth");
        }
    }
}
