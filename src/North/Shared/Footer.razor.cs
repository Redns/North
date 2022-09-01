using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;

namespace North.Shared
{
    partial class Footer
    {
        [Parameter]
        public string Content { get; set; } = string.Empty;
    }
}
