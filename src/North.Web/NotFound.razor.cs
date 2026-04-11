using Microsoft.AspNetCore.Components;

namespace North.Web
{
    public partial class NotFound
    {
        /// <summary>
        /// Footer content
        /// </summary>
        [Parameter]
        public string FooterText { get; set; } = string.Empty;
    }
}
