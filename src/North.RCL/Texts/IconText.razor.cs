using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace North.RCL.Texts
{
    partial class IconText
    {
        [Parameter]
        public string Icon { get; set; } = string.Empty;

        [Parameter]
        public string Text { get; set; } = string.Empty;

        [Parameter]
        public Typo Typo { get; set; }

        [Parameter]
        public string IconStyle { get; set; } = string.Empty;

        [Parameter]
        public string Style { get; set; } = "width:fit-content; height:fit-content;";
    }
}
