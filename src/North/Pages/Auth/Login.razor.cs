using Masuit.Tools.Security;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using North.Common;
using North.Core.Entities;
using North.Core.Helpers;
using North.Core.Repository;
using North.RCL.Forms;

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
                    BackgroundImageUrl = _appSetting.Appearance.BackgroundUrl;
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

                // 检索用户
                var userRepository = new UserRepository(_client, _appSetting.General.DataBase.EnabledName);
                var user = await userRepository.SingleAsync(u => (u.Name == LoginModel.Account || u.Email == LoginModel.Account) && (u.State == UserState.Normal));
                if(user is null)
                {
                    _snackbar.Add("账户不存在或状态异常", Severity.Error);
                }
                else if($"{user.Email}:{LoginModel.Password}".MDString() != user.Password)
                {
                    _snackbar.Add("账户与密码不匹配", Severity.Error);
                }
                else
                {
                    // 存储并写入认证信息
                    var userClaimsIdentifyKey = Guid.NewGuid().ToString();
                    var userClaimsIdentify = user.ClaimsIdentify;
                    _identifies.Add(userClaimsIdentifyKey, userClaimsIdentify);

                    // 写入用户登陆历史
                    // ip地址默认为0.0.0.0，若为非ip字符串则ip2region解析会报错
                    var deviceInfo = await _js.GetDeviceInfoAsync();
                    var ipAddress = _accessor.HttpContext?.Connection.RemoteIpAddress?.MapToIPv4()?.ToString() ?? "0.0.0.0";
                    await new UserLoginHistoryRepository(_client, _appSetting.General.DataBase.EnabledName).AddAsync(new UserLoginHistoryEntity
                    {
                        DeviceName = $"{deviceInfo.Os} ({deviceInfo.Description})",
                        IPAddress = ipAddress,
                        IPRegion = _ipSearchar.Search(ipAddress) ?? "UnKnown",
                        UserId = user.Id
                    });

                    // 前往 Signin 页面写入注册信息
                    _nav.NavigateTo($"signin?key={userClaimsIdentifyKey}&redirect={Redirect}", true);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Fail to login", e);
                _snackbar.Add("登陆失败，服务器内部出错", Severity.Error);
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
