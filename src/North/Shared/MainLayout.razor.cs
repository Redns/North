using North.Common;
using North.Core.Entities;
using North.Core.Helpers;
using Org.BouncyCastle.Asn1.Ocsp;
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
            if (_accessor.HttpContext?.User.Identities.FirstOrDefault()?.IsAuthenticated is true)
            {
                // 检查 Cookie 信息
                // 账户封禁、权限更改等均会清空当前用户数据库中的令牌，造成之前获取的 Cookie 失效
                var token = _accessor.HttpContext
                                     .User
                                     .Identities
                                     .First()
                                     .FindFirst("Token")
                                     ?.Value;
                var sqlUserData = new SqlUserData(_context);
                var user = await sqlUserData.FindAsync(u => u.Token == token);
                if (user?.HasValidToken is not true)
                {
                    _nav.NavigateTo("signout", true);
                }
                else if(relativeUrl.Contains(GlobalValues.WithoutAuthenticationPages, true))
                {
                    // 用户已授权且数据库信息未发生变动
                    // 拒绝其访问授权页面，自动跳转至首页
                    _nav.NavigateTo("", true);
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
                await JS.SetBodyStyle("#1A1A27", "invert(1) hue-rotate(180deg)");
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
