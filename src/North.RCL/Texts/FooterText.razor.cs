using Microsoft.AspNetCore.Components;

namespace North.RCL.Texts
{
    partial class FooterText
    {
        [Parameter]
        public string Content { get; set; } = string.Empty;

        [Parameter]
        public string Class { get; set; } = "d-flex mud-width-full justify-center py-2 overflow-hidden";

        [Parameter]
        public string Style { get; set; } = "height:fit-content; max-height:50px; text-align:center;";
    }
}
