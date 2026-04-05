using Microsoft.AspNetCore.Components;

namespace North.RCL.Texts
{
    partial class FooterText
    {
        [Parameter]
        public string Content { get; set; } = string.Empty;

        [Parameter]
        public string Class { get; set; } = "d-flex justify-center overflow-hidden";

        [Parameter]
        public string Style { get; set; } = string.Empty;
    }
}
