using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using Microsoft.AspNetCore.Components;

namespace ImageBed.Pages
{
    partial class Login
    {
        [CascadingParameter(Name = "CurrentUser")]
        protected UserDTOEntity? User { get; set; }

        private UserEntity UserForm { get; set; } = new();


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                if (User != null)
                {
                    UserForm.UserName = User.UserName;

                    if (!User.IsTokenValid())
                    {
                        _ = _message.Info("授权已过期，请重新登录! ", 1);
                    }
                }
                else
                {
                    string username, token;
                    if (!string.IsNullOrEmpty(username = await _storage.GetItemAsync<string>(GlobalValues.LOCALSTORE_KEY_USERNAME)) && !string.IsNullOrEmpty(token = await _storage.GetItemAsync<string>(GlobalValues.LOCALSTORE_KEY_TOKEN)))
                    {
                        using (var context = new OurDbContext())
                        {
                            var sqlUserData = new SqlUserData(context);
                            var user = await sqlUserData.GetFirstAsync(u => (u.UserName == username) && (u.Token == token));

                            if ((user != null) && user.IsTokenValid())
                            {
                                User = user.DTO();
                                NavigationManager.NavigateTo(GlobalValues.ROUTER_INDEX);
                            }
                            else
                            {
                                _ = _message.Info("授权已过期，请重新登录! ", 1);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        async void UserLogin()
        {
            using (var context = new OurDbContext())
            {
                var sqlUserData = new SqlUserData(context);

                var user = await sqlUserData.GetFirstAsync(u => (u.UserName == UserForm.UserName) && (u.Password == EncryptAndDecrypt.EncryptMD516($"{UserForm.UserName}/{UserForm.Password}")));
                if (user != null)
                {
                    user.GenerateToken();
                    sqlUserData.Update(user);

                    User = user.DTO();

                    await _storage.SetItemAsync(GlobalValues.LOCALSTORE_KEY_USERNAME, user.UserName);
                    await _storage.SetItemAsync(GlobalValues.LOCALSTORE_KEY_TOKEN, user.Token);
                    
                    _ = _message.Success("登录成功 !", 1);
                    NavigationManager.NavigateTo(GlobalValues.ROUTER_INDEX);
                }
                else
                {
                    _ = _message.Error("账号或密码错误 !", 1);
                }
            }
        }
    }
}
