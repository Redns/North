using Microsoft.AspNetCore.Components;

namespace North.RCL.Buttons
{
    partial class Button
    {
        [Parameter]
        public string IconStart { get; set; } = string.Empty;

        [Parameter]
        public string IconEnd { get; set; } = string.Empty;

        [Parameter]
        public string Text { get; set; } = string.Empty;

        [Parameter]
        public string IconSize { get; set; } = "20px";

        [Parameter]
        public string TextSize { get; set; } = "20px";

        [Parameter]
        public string Class { get; set; } = "d-flex flex-row pa-2 align-center";

        [Parameter]
        public string Style { get; set; } = string.Empty;
    }
}
