using North.Common;
using North.Core.Helper;
using North.Core.Data.Access;
using North.Core.Data.Entities;
using System.Security.Claims;

namespace North.Shared
{
    partial class MainLayout
    {
        public bool IsExpanded { get; set; } = true;


        /// <summary>
        /// 页面首次加载时核验用户身份
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            // TODO 使能账户鉴权
            // await UserAuthorization();
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
                // 使能账户鉴权
                // await UserAuthorization();
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        /// <summary>
        /// 收起/展开侧边栏
        /// </summary>
        public void OnNavMenuStateChanged()
        {
            IsExpanded = !IsExpanded;
        }


        /// <summary>
        /// 用户授权
        /// </summary>
        /// <returns></returns>
        public async Task UserAuthorization()
        {
            var signedUser = _accessor.HttpContext?.User;
            if (signedUser?.Identity?.IsAuthenticated is true)
            {
                var signedUserId = signedUser.Claims
                                             .FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?
                                             .Value ?? string.Empty;
                var signedUserRole = signedUser.Claims
                                               .FirstOrDefault(c => c.Type == ClaimTypes.Role)?
                                               .Value ?? string.Empty;
                var user = await new SqlUserData(_context).FindAsync(u => u.Id == signedUserId);
                if ((user?.State != State.Normal) || (user?.Permission.ToString() != signedUserRole))
                {
                    _navigationManager.NavigateTo("signout", true);
                }
            }
            else
            {
                var relativeUrl = _navigationManager.ToBaseRelativePath(_navigationManager.Uri)
                                                    .Split('?')
                                                    .First()
                                                    .ToLower();
                if (!relativeUrl.Contains(GlobalValues.WithoutAuthenticationPages, true))
                {
                    _navigationManager.NavigateTo($"/login?redirect={relativeUrl}", true);
                }
            }
        }
    }
}
