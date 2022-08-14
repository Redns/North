using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace North.Shared
{
    partial class ActionBar
    {
        [Parameter]
        public string Style { get; set; } = string.Empty;

        [Parameter]
        public EventCallback OnNavMenuStateChanged { get; set; }

        [Parameter]
        public bool IsExpanded { get; set; }
        public string ExpandTooltip => IsExpanded ? "收起" : "展开";


        /// <summary>
        /// 退出登录
        /// </summary>
        public void SignOut()
        {
            _navigationManager.NavigateTo("signout", true);
        }
    }
}
