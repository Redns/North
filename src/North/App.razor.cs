using North.Core.Services.AuthService;

namespace North
{
    partial class App
    {
        /// <summary>
        /// 用户授权
        /// </summary>
        /// <returns></returns>
        public async Task AuthAsync()
        {
            /* 获取当前路径 */
            var relativeUrl = _nav.ToBaseRelativePath(_nav.Uri).Split('?').First().ToLower();
            if (_accessor.HttpContext is null)
            {
                _nav.NavigateTo($"/login?redirect={relativeUrl}", true);
                return;
            }

            // 检测用户授权状态
            var authResult = await _authService.AuthAsync(_accessor.HttpContext, relativeUrl);
            if (authResult is AuthState.None)
            {
                _nav.NavigateTo($"/login?redirect={relativeUrl}", true);
            }
            else if (authResult is AuthState.Expired)
            {
                _nav.NavigateTo($"/signout?redirect={relativeUrl}", true);
            }
        }
    }
}
