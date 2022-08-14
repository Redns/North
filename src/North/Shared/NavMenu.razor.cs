using Microsoft.AspNetCore.Components;

namespace North.Shared
{
    partial class NavMenu
    {
        [Parameter]
        public string Style { get; set; } = string.Empty;

        [Parameter]
        public bool IsExpanded { get; set; } = false;
    }
}
