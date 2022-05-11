using ImageBed.Data.Entity;
using Microsoft.AspNetCore.Components;

namespace ImageBed.Shared
{
    partial class NavMenu
    {
        [CascadingParameter(Name = "CurrentUser")]
        protected UserDTOEntity? User { get; set; }


        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }


        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            // User.Token = "Hello World";
            return base.OnAfterRenderAsync(firstRender);
        }
    }
}
