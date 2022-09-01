using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using North.Common;
using North.Core.Entities;
using North.Core.Helpers;
using North.Core.Models.Auth;
using North.Data.Access;
using North.RCL.Forms;
using System.Security.Claims;

namespace North.Pages.Auth
{
    partial class Login
    {
        /// <summary>
        /// 源地址
        /// </summary>
        [Parameter]
        [SupplyParameterFromQuery]
        public string Redirect { get; set; } = string.Empty;

        /// <summary>
        /// 登录进行中标志
        /// </summary>
        public bool LoginRunning { get; set; } = false;

        /// <summary>
        /// 背景图片链接
        /// </summary>
        public string BackgroundImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 登录模型
        /// </summary>
        public LoginModel LoginModel { get; set; } = new LoginModel();


        /// <summary>
        /// 加载完 css 和 js 之后再加载背景图片，优化用户体验
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(() =>
                {
                    BackgroundImageUrl = GlobalValues.AppSettings.Appearance.BackgroundUrl;
                    StateHasChanged();
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        /// <returns></returns>
        private async Task UserLogin()
        {
            LoginRunning = true;

            try
            {
                await Task.Delay(500);
                using var context = new OurDbContext();
                var user = await new SqlUserData(context).FindAsync(u => u.Email == LoginModel.Email);
                if ((user?.Password != $"{LoginModel.Email}:{LoginModel.Password}".MD5()) || (user?.State != State.Normal))
                {
                    _snackbar.Add("账号密码错误或账户状态异常", Severity.Error);
                }
                else
                {
                    var loginIdentify = new UnitLoginIdentify(IdentifyHelper.Generate(), new ClaimsIdentity(new Claim[]
                    {
                            new Claim(ClaimTypes.SerialNumber, user.Id),
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role, user.Permission.ToString()),
                            new Claim(ClaimTypes.Actor, user.Avatar)
                    }, CookieAuthenticationDefaults.AuthenticationScheme));

                    _identifies.Add(loginIdentify);
                    _navigationManager.NavigateTo($"signin/?id={loginIdentify.Id}&redirect={Redirect}", true);
                }
            }
            catch(Exception e)
            {
                _logger.Error("Failed to login", e);
                _snackbar.Add("登陆失败，系统内部错误", Severity.Error);
            }
            finally
            {
                LoginRunning = false;
            }
        }


        /// <summary>
        /// 监控密码框的 Enter 键
        /// </summary>
        /// <param name="args"></param>
        private async Task EnterToLogin(KeyboardEventArgs args)
        {
            if(args.Code is "Enter")
            {
                await UserLogin();
            }
        }
    }
}
