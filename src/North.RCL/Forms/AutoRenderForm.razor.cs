using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace North.RCL.Forms
{
    partial class AutoRenderForm
    {
        [Parameter]
        public object Model { get; set; }

        [Parameter]
        public string Style { get; set; } = "width:90%; max-width:330px; min-width:240px; height:fit-content;";
    }
}
