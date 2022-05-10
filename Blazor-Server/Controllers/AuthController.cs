using ImageBed.Common;
using ImageBed.Data.Access;
using ImageBed.Data.Entity;
using Microsoft.AspNetCore.Mvc;

namespace ImageBed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">用户密码</param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ApiResult<object>> Login([FromQuery] string UserName, [FromQuery] string Password)
        {
            try
            {
                using (var context = new OurDbContext())
                {
                    var sqlUserData = new SqlUserData(context);
                    var user = await sqlUserData.GetFirstAsync(u => (u.UserName == UserName) && (u.Password == Password));
                    if (user != null)
                    {
                        user.GenerateToken();
                        if (sqlUserData.Update(user))
                        {
                            return new ApiResult<object>(ApiResultCode.Success, "Login success", new
                            {
                                user.Token,
                                user.ExpireTime
                            });
                        }
                        return new ApiResult<object>(ApiResultCode.TokenGenerateFailed, "Generate token failed", null);
                    }
                    return new ApiResult<object>(ApiResultCode.UserNotFound, "User not found, please check the username and password", null);
                }
            }
            catch (Exception ex)
            {
                GlobalValues.Logger.Error($"{UserName} login failed, {ex.Message}");
                return new ApiResult<object>(ApiResultCode.InternalError, "Server interval error", null);
            }
        }


        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param name="logoutInfo">用户信息</param>
        /// <returns></returns>
        [HttpPost("logout")]
        public async Task<ApiResult<object>> Logout([FromQuery] string UserName, [FromQuery] string Token)
        {
            try
            {
                using (var context = new OurDbContext())
                {
                    var sqlUserData = new SqlUserData(context);
                    var user = await sqlUserData.GetFirstAsync(u => u.UserName == UserName);
                    if (user != null)
                    {
                        if((user.Token == Token) && user.IsTokenValid())
                        {
                            user.DestroyToken();
                            if (sqlUserData.Update(user))
                            {
                                return new ApiResult<object>(ApiResultCode.Success, "Logout success", null);
                            }
                            return new ApiResult<object>(ApiResultCode.TokenDestroyFailed, "Token destroy failed", null);
                        }
                        else
                        {
                            return new ApiResult<object>(ApiResultCode.TokenInvalid, "Token not correct or expired", null);
                        }
                    }
                    else
                    {
                        return new ApiResult<object>(ApiResultCode.UserNotFound, "User not found, please check the username", null);
                    }
                }
            }
            catch(Exception ex)
            {
                GlobalValues.Logger.Error($"{UserName} logout failed, {ex.Message}");
                return new ApiResult<object>(ApiResultCode.InternalError, "Server interval error", null);
            }
        }


        /// <summary>
        /// 获取用户密钥
        /// </summary>
        /// <param name="loginInfo">用户信息</param>
        /// <returns></returns>
        [HttpGet("token")]
        public async Task<ApiResult<object>> GetToken([FromQuery] string UserName, [FromQuery] string Password)
        {
            try
            {
                using (var context = new OurDbContext())
                {
                    var sqlUserData = new SqlUserData(context);
                    var user = await sqlUserData.GetFirstAsync(u => (u.UserName == UserName) && (u.Password == Password));
                    if (user != null)
                    {
                        if (!user.IsTokenValid())
                        {
                            user.GenerateToken();
                            if (!sqlUserData.Update(user))
                            {
                                return new ApiResult<object>(ApiResultCode.TokenGenerateFailed, "Token is invalid and generate new token failed", null);
                            }
                        }
                        return new ApiResult<object>(ApiResultCode.Success, "Get token success", user.Token);
                    }
                    return new ApiResult<object>(ApiResultCode.UserNotFound, "User not found, please check the username and password", null);
                }
            }
            catch (Exception ex)
            {
                GlobalValues.Logger.Error($"{UserName} get token failed, {ex.Message}");
                return new ApiResult<object>(ApiResultCode.InternalError, "Server interval error", null);
            }
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Token">用户令牌</param>
        /// <returns></returns>
        [HttpGet("i")]
        public async Task<ApiResult<object>> GetUserInfo([FromQuery] string UserName, [FromQuery] string Token)
        {
            try
            {
                using (var context = new OurDbContext())
                {
                    var sqlUserData = new SqlUserData(context);
                    var user = await sqlUserData.GetFirstAsync(u => u.UserName == UserName);
                    if (user != null)
                    {
                        if ((user.Token == Token) && user.IsTokenValid())
                        {
                            return new ApiResult<object>(ApiResultCode.TokenDestroyFailed, "Token destroy failed", user.DTO());
                        }
                        else
                        {
                            return new ApiResult<object>(ApiResultCode.TokenInvalid, "Token not correct or expired", null);
                        }
                    }
                    else
                    {
                        return new ApiResult<object>(ApiResultCode.UserNotFound, "User not found, please check the username", null);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalValues.Logger.Error($"Get {UserName}'s info failed, {ex.Message}");
                return new ApiResult<object>(ApiResultCode.InternalError, "Server interval error", null);
            }
        }
    }
}
