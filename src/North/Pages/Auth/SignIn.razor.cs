using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using North.Common;
using System.Security.Claims;

namespace North.Pages.Auth
{
    partial class SignIn
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public string Key { get; set; } = string.Empty;

        [Parameter]
        [SupplyParameterFromQuery]
        public string Redirect { get; set; } = string.Empty;


        /// <summary>
        /// 添加 Cookies
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            var claimsIdentify = _identifies.GetValueOrDefault(Key);
            if ((_accessor.HttpContext is not null) && claimsIdentify is not null)
            {
                await _accessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                        new ClaimsPrincipal(claimsIdentify),
                                                        new AuthenticationProperties()
                                                        {
                                                            IsPersistent = true,
                                                            ExpiresUtc = DateTime.Now.AddSeconds(GlobalValues.AppSettings.Auth.CookieValidTime)
                                                        });
                _identifies.Remove(Key);
                _nav.NavigateTo(Redirect, true);
            }
            _nav.NavigateTo($"login?redirect={Redirect}", true);
        }
    }
}