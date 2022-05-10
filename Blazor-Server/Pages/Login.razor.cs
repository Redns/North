using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;

namespace ImageBed.Pages
{
    partial class Login
    {
        AuthEntity login = new("Redns", "12345");


        /// <summary>
        /// 用户注册
        /// </summary>
        async void UserRegister()
        {
            using(var context = new OurDbContext())
            {
                var sqlUserData = new SqlUserData(context);
                if((await sqlUserData.GetAsync(login.UserName)) != null)
                {
                    _ = _message.Error("用户已存在 !", 1.5);
                }
                else
                {
                    UserEntity user = new UserEntity(login.UserName, EncryptAndDecrypt.MD5Encrypt16($"{login.UserName}/{login.Password}"), "", 0, UserType.User, "", 10, 10);
                    if (await sqlUserData.AddAsync(user))
                    {
                        user.GenerateToken();
                        _ = _storage.SetItemAsync(GlobalValues.LOCALSTORE_KEY_TOKEN, user.Token);
                        _ = _message.Success("注册成功 !", 1.5);
                    }
                    else
                    {
                        _ = _message.Error("注册失败 !", 1.5);
                    }
                }
            }
        }



        /// <summary>
        /// 用户登录
        /// </summary>
        async void UserLogin()
        {
            using(var context = new OurDbContext())
            {
                var sqlUserData = new SqlUserData(context);

                var user = await sqlUserData.GetAsync(login.UserName);
                if((user != null) && (user.Password == EncryptAndDecrypt.MD5Encrypt16($"{login.UserName}/{login.Password}")))
                {
                    user.GenerateToken();
                    _ = _storage.SetItemAsync(GlobalValues.LOCALSTORE_KEY_TOKEN, user.Token);
                    _ = _message.Success("登录成功 !", 1.5);
                }
                else
                {
                    _ = _message.Error("用户不存在 !", 1.5);
                }
            }
        }
    }
}
