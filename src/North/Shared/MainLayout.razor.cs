using North.Common;
using North.Core.Entities;
using North.Core.Helpers;
using North.Data.Access;
using System.Security.Claims;

namespace North.Shared
{
    partial class MainLayout
    {
        public bool IsExpanded { get; set; } = false;


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
            var signedUser = _accessor.HttpContext?.User;
            var relativeUrl = _navigationManager.ToBaseRelativePath(_navigationManager.Uri).Split('?').First().ToLower();
            if (signedUser?.Identity?.IsAuthenticated is true)
            {
                // 根据 Cookie 信息在数据库中查询用户
                // 若用户为空、状态改变（被封禁）或权限发生变动时退出登录
                var signedUserId = signedUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value ?? string.Empty;
                var signedUserRole = signedUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
                var user = await new SqlUserData(_context).FindAsync(u => u.Id == signedUserId);
                if ((user?.State != State.Normal) || (user?.Permission.ToString() != signedUserRole))
                {
                    _navigationManager.NavigateTo("signout", true);
                }
                else if(relativeUrl.Contains(GlobalValues.WithoutAuthenticationPages, true))
                {
                    // 用户已授权且数据库信息未发生变动
                    // 拒绝其访问授权页面，自动跳转至首页
                    _navigationManager.NavigateTo("", true);
                }
            }
            else if(!relativeUrl.Contains(GlobalValues.WithoutAuthenticationPages, true))
            {
                // 用户未经授权且访问的不是授权界面
                // 自动跳转至授权页面
                _navigationManager.NavigateTo($"/login?redirect={relativeUrl}", true);
            }
        }
    }
}
