using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ImageBed.Components.MyForms
{
    partial class LoginForm
    {
        [Parameter]
        public EventCallback<MouseEventArgs> OnRegister { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnLogin { get; set; }
    }
}
