using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using North.Common;
using North.Data.Access;
using North.Data.Entities;
using North.Models.Auth;
using System.Security.Claims;

namespace North.Pages.Auth
{
    partial class Login
    {
        public bool LoginRunning { get; set; } = false;
        public LoginModel LoginModel { get; set; } = new LoginModel();


        /// <summary>
        /// 若用户已认证则跳转至首页
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            if(_accessor.HttpContext?.User?.Identity?.IsAuthenticated is true)
            {
                _navigationManager.NavigateTo("", true);
            }
            await base.OnInitializedAsync();
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        /// <returns></returns>
        private async Task UserLogin()
        {
            LoginRunning = true;

            // 校验用户输入
            if (string.IsNullOrEmpty(LoginModel.UserName) || string.IsNullOrEmpty(LoginModel.Password))
            {
                _snackbar.Add("用户名或密码为空", Severity.Error); return;
            }

            // 短暂延时以加载动画
            await Task.Delay(500);

            // 检索核验用户身份
            var user = await new SqlUserData(_context).FindAsync(u => (u.Name == LoginModel.UserName) || (u.Email == LoginModel.UserName));
            if (user?.Password == EncryptHelper.MD5($"{user?.Name}:{LoginModel.Password}") && (user?.State == State.Normal))
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Actor, user.Avatar),
                        new Claim(ClaimTypes.Role, user.Permission.ToString())
                    };
                var loginIdentify = new UnitLoginIdentify(IdentifyHelper.GenerateId(), new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                
                _identifies.Add(loginIdentify);
                _navigationManager.NavigateTo($"signin/{loginIdentify.Id}", true);
            }
            else
            {
                _snackbar.Add("账号密码错误或账户状态异常", Severity.Error);
            }

            LoginRunning = false;
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


        /// <summary>
        /// 前往登录界面
        /// </summary>
        private void GoToRegister()
        {
            if (!GlobalValues.AppSettings.Register.AllowRegister)
            {
                _snackbar.Add("系统当前未开放注册", Severity.Error);
            }
            else
            {
                _navigationManager.NavigateTo("register", true);
            }
        }
    }
}
