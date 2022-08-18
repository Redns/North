using Microsoft.AspNetCore.Components;

namespace North.Shared
{
    partial class Loading
    {
        [Parameter]
        public bool Enable { get; set; } = false;

        [Parameter]
        public string Style { get; set; } = string.Empty;
    }
}
