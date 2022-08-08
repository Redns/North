using Microsoft.AspNetCore.Components;

namespace North.Shared
{
    partial class ActionBar
    {
        [Parameter]
        public EventCallback OnNavMenuStateChanged { get; set; }

        [Parameter]
        public bool IsExpanded { get; set; }
        public string ExpandTooltip => IsExpanded ? "收起" : "展开";
    }
}
