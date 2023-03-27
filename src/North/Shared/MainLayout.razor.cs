using North.Common;
using North.Core.Entities;
using North.Core.Helpers;
using North.Core.Repository;
using System.Security.Claims;

namespace North.Shared
{
    partial class MainLayout
    {
        public bool IsExpanded { get; set; } = false;
        public bool DarkTheme { get; set; } = false;


        /// <summary>
        /// 切换主题
        /// </summary>
        /// <returns></returns>
        public async Task SwitchTheme()
        {
            DarkTheme = !DarkTheme;
            if (DarkTheme)
            {
                await JS.SetBodyStyleAsync("#1A1A27", "invert(1) hue-rotate(180deg)");
            }
            else
            {
                await JS.SetBodyStyleAsync(string.Empty, string.Empty);
            }
        }


        /// <summary>
        /// 退出登录
        /// </summary>
        public void SignOut()
        {
            _nav.NavigateTo("signout", true);
        }
    }
}
