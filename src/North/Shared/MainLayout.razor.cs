using North.Common;
using North.Core.Entities;
using North.Core.Helpers;
using North.Core.Repository;
using System.Security.Claims;

namespace North.Shared
{
    partial class MainLayout
    {
        public bool IsExpanded { get; set; } = false;
        public bool DarkTheme { get; set; } = false;


        /// <summary>
        /// 页面首次加载时核验用户身份
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            // TODO 启用用户鉴权
            // await AuthorizationAsync();
            await base.OnInitializedAsync();
        }


        /// <summary>
        /// 页面切换时核验用户身份
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                // TODO 启用用户鉴权
                // await AuthorizationAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        /// <summary>
        /// 用户授权
        /// </summary>
        /// <returns></returns>
        public async Task AuthorizationAsync()
        {
            var relativeUrl = _nav.ToBaseRelativePath(_nav.Uri).Split('?').First().ToLower();
            var userIdentify = _accessor.HttpContext?.User.Identities.FirstOrDefault();
            if (userIdentify?.IsAuthenticated is true)
            {
                // 解析 ClaimIdentifies 中的用户信息
                var userId = userIdentify.FindFirst(ClaimTypes.SerialNumber)?.Value;
                var userLastModifyTime = userIdentify.FindFirst("LastModifyTime")?.Value;
                if(!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userLastModifyTime))
                {
                    // 检索用户
                    var userRepository = new UserRepository(_client, GlobalValues.AppSettings.General.DataBase.EnabledName);
                    var user = await userRepository.SingleAsync(u => u.Id.ToString() == userId);
                    if ((user?.State is not UserState.Normal) || (user.LastModifyTime.ToString("G") != userLastModifyTime))
                    {
                        // 用户不存在或状态异常，清除登录信息
                        _nav.NavigateTo("signout", true);
                    }
                    else if (relativeUrl.Contains(GlobalValues.WithoutAuthenticationPages, true))
                    {
                        // 用户已授权且数据库信息未发生变动
                        // 拒绝其访问授权页面，自动跳转至首页
                        _nav.NavigateTo("", true);
                    }
                }
            }
            else if(!relativeUrl.Contains(GlobalValues.WithoutAuthenticationPages, true))
            {
                // 用户未经授权且访问的不是授权界面
                // 自动跳转至授权页面
                _nav.NavigateTo($"/login?redirect={relativeUrl}", true);
            }
        }


        /// <summary>
        /// 切换主题
        /// </summary>
        /// <returns></returns>
        public async Task SwitchTheme()
        {
            DarkTheme = !DarkTheme;
            if (DarkTheme)
            {
                await JS.SetBodyStyleAsync("#1A1A27", "invert(1) hue-rotate(180deg)");
            }
            else
            {
                await JS.SetBodyStyleAsync(string.Empty, string.Empty);
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
