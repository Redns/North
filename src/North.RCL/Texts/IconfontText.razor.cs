using Microsoft.AspNetCore.Components;

namespace North.RCL.Texts
{
    partial class IconfontText
    {
        [Parameter]
        public string Icon { get; set; } = string.Empty;

        [Parameter]
        public string Color { get; set; } = string.Empty;

        [Parameter]
        public string Text { get; set; } = string.Empty;
    }
}
