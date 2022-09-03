using Microsoft.AspNetCore.Components;

namespace North.RCL.Texts
{
    partial class TitleText
    {
        [Parameter]
        public string Text { get; set; } = string.Empty;

        [Parameter]
        public string Class { get; set; } = string.Empty;

        [Parameter]
        public string Style { get; set; } = string.Empty;
    }
}
