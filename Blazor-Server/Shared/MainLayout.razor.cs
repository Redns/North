using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using Microsoft.JSInterop;

namespace ImageBed.Shared
{
    partial class MainLayout
    {
        bool ShowFooter { get; set; } = false;        // 是否显示页脚
        UserDTOEntity? User { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if(((User == null) || !User.IsTokenValid()) && GetRelativeUrl() != GlobalValues.ROUTER_LOGIN)
            {
                GoTo(GlobalValues.ROUTER_LOGIN);
            }
        }


        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }


        /// <summary>
        /// 返回页面相对路径
        /// </summary>
        /// <returns></returns>
        private string GetRelativeUrl()
        {
            return NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        }


        /// <summary>
        /// Url跳转
        /// </summary>
        /// <param name="url"></param>
        private void GoTo(string url)
        {
            NavigationManager.NavigateTo(url, true);
        }


        /// <summary>
        /// 获取屏幕宽度（单位：px）
        /// </summary>
        /// <returns></returns>
        private async Task<double> GetScreenWidth()
        {
            return await JS.InvokeAsync<double>("GetScreenWidth");
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        private async Task<UserDTOEntity?> CheckUser()
        {
            var token = await _storage.GetItemAsync<string>(GlobalValues.LOCALSTORE_KEY_TOKEN);
            var username = await _storage.GetItemAsync<string>(GlobalValues.LOCALSTORE_KEY_USERNAME);
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(username))
            {
                using(var context = new OurDbContext())
                {
                    var sqlUserData = new SqlUserData(context);
                    var user = await sqlUserData.GetFirstAsync(u => (u.UserName == username) && (u.Token == token) && u.IsTokenValid());
                    if (user != null)
                    {
                        return user.DTO();
                    }
                }
            }
            return null;
        } 


        /// <summary>
        /// 判断用户能否访问页面
        /// </summary>
        /// <param name="userType">用户类型</param>
        /// <param name="pageUrl">页面相对URL</param>
        /// <returns></returns>
        private bool CanAccessView(UserType userType, string pageUrl)
        {
            if(userType == UserType.SuperAdmin)
            {
                return true;
            }
            else if(userType == UserType.Admin)
            {
                if((pageUrl != GlobalValues.ROUTER_COG) || (pageUrl == GlobalValues.ROUTER_PLUGIN_STORE))
                {
                    return true;
                }
                return false;
            }
            else if(userType == UserType.User)
            {
                if((pageUrl == GlobalValues.ROUTER_INDEX) || (pageUrl == GlobalValues.ROUTER_LOGIN) || (pageUrl == GlobalValues.ROUTER_PICS))
                {
                    return true;
                }
                return false;
            }
            else
            {
                if(pageUrl == GlobalValues.ROUTER_LOGIN) { return true; }
                return false;
            }
        }
    }
}
