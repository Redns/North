using North.Common;
using North.Core.Helpers;
using North.Core.Repository;

namespace North
{
    partial class App
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await AuthorizationAsync();
        }


        /// <summary>
        /// 用户授权
        /// </summary>
        /// <returns></returns>
        public async Task AuthorizationAsync()
        {
            var relativeUrl = _nav.ToBaseRelativePath(_nav.Uri).Split('?').First().ToLower();
            var userRepository = new UserRepository(_client, GlobalValues.AppSettings.General.DataBase.EnabledName);
            if ((_accessor.HttpContext is not null) && (await _accessor.HttpContext.AuthAsync(userRepository) is true))
            {
                if (relativeUrl.Contains(GlobalValues.WithoutAuthenticationPages, true))
                {
                    // 用户已授权且数据库信息未发生变动
                    // 拒绝其访问授权页面，自动跳转至首页
                    _nav.NavigateTo("", true);
                }
            }
            else if (!relativeUrl.Contains(GlobalValues.WithoutAuthenticationPages, true))
            {
                // 用户未经授权且访问的不是登录/注册/邮箱验证/安装界面
                // 强制跳转至授权页面
                _nav.NavigateTo($"/login?redirect={relativeUrl}", true);
            }
        }
    }
}
