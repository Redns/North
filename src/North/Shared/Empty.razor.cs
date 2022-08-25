using Microsoft.AspNetCore.Components;

namespace North.Shared
{
    partial class Empty
    {
        [Parameter]
        public string Style { get; set; } = string.Empty;

        [Parameter]
        public string Text { get; set; } = string.Empty;

        [Parameter]
        public string TextStyle { get; set; } = string.Empty;
    }
}
