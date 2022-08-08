using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace North.Pages.Auth
{
    partial class SignIn
    {
        [Parameter]
        public string Id { get; set; } = string.Empty;


        /// <summary>
        /// 添加 Cookies
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            var identify = _identifies.Find(identify => identify.Id == Id);
            if ((_accessor.HttpContext is not null) && identify is not null)
            {
                _identifies.Remove(identify);
                await _accessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                        new ClaimsPrincipal(identify.ClaimsIdentity),
                                                        new AuthenticationProperties());
                _navigationManager.NavigateTo("", true);
            }
            _navigationManager.NavigateTo("/login", true);
        }
    }
}
