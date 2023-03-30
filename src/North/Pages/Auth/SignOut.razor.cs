using Microsoft.AspNetCore.Authentication;

namespace North.Pages.Auth
{
    partial class SignOut
    {
        /// <summary>
        /// 注销用户身份信息
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            if (_accessor.HttpContext is not null)
            {
                await _accessor.HttpContext.SignOutAsync();
                await Task.Run(() =>
                {
                    _navigationManager.NavigateTo("login");
                });   
            }
            await base.OnInitializedAsync();
        }
    }
}
