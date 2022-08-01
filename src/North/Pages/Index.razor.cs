using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Security.Claims;

namespace North.Pages
{
    partial class Index
    {
        protected override async Task OnInitializedAsync()
        {
            if(_accessor.HttpContext?.User.Identity?.IsAuthenticated is not true)
            {
                _navigationManager.NavigateTo("login", true);
            }
            await base.OnInitializedAsync();
        }
    }
}
