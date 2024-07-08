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
            var relativeUrl = _nav.ToBaseRelativePath(_nav.Uri).Split('?').First().ToLower();
            if ((_accessor.HttpContext is null) || (await _authService.AuthAsync(_accessor.HttpContext, relativeUrl) is false))
            {
                _nav.NavigateTo($"/login?redirect={relativeUrl}", true);
            }
        }
    }
}
