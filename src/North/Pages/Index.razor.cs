using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace North.Pages
{
    partial class Index
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) { await InvokeAsync(() => StateHasChanged()); }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
