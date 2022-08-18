using Microsoft.AspNetCore.Components;

namespace North.Shared
{
    partial class Title
    {
        [Parameter]
        public string Text { get; set; } = string.Empty;

        [Parameter]
        public string Style { get; set; } = string.Empty;
    }
}
