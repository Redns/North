using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using North.Common;
using North.Data.Access;
using North.Models.Auth;
using System.Security.Claims;

namespace North.Pages.Auth
{
    partial class Login
    {
        public bool Loginning { get; set; } = false;
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
            Loginning = true;

            if(string.IsNullOrEmpty(LoginModel.UserName) || string.IsNullOrEmpty(LoginModel.Password))
            {
                _ = _message.Error("用户名或密码为空", 1.5);
            }
            else
            {
                await Task.Delay(100);

                var user = new SqlUserData(_context).Get(u => (u.Name == LoginModel.UserName) || (u.Email == LoginModel.UserName)).FirstOrDefault();
                if ((user is not null) && (user.Password == EncryptHelper.MD5($"{user.Name}:{LoginModel.Password}") && !user.IsForbidden))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Actor, user.Avatar),
                        new Claim(ClaimTypes.Role, user.Permission.ToString())
                    };
                    var loginIdentify = new UnitLoginIdentify(IdentifyHelper.GenerateId(),
                                                              new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                    _identifies.Add(loginIdentify);
                    _navigationManager.NavigateTo($"signin/{loginIdentify.Id}", true);
                }
                else
                {
                    _ = _message.Error("账号密码错误或已被封禁", 1.5);
                }
            }

            Loginning = false;
        }
    }


    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public LoginModel()
        {
            UserName = string.Empty;
            Password = string.Empty;
        }

        public LoginModel(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
