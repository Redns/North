using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using North.Common;
using North.Core.Entities;
using North.Core.Helpers;
using North.Data.Access;
using System.Security.Claims;

namespace North.Controllers
{
    /// <summary>
    /// 认证授权接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly OurDbContext _context;
        private readonly Core.Services.Logger.ILogger _logger;

        public AuthController(OurDbContext context, Core.Services.Logger.ILogger logger)
        {
            _context = context;
            _logger = logger;
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async ValueTask<ApiResult<object>> UserLogin()
        {
            // 获取邮箱和密码
            var email = Request.Headers["Email"].ToString();
            var password = Request.Headers["Password"].ToString();

            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    return new ApiResult<object>(ApiStatusCode.ParamIncomplete, "Request parameter missing");
                }
                else
                {
                    // 查找用户
                    // 状态异常、API 禁用均无法登录
                    var sqlUserData = new SqlUserData(_context);
                    var user = await sqlUserData.FindAsync(u => (u.Email == email) && u.IsApiAvailable && (u.State == State.Normal) && (u.Password == password));
                    if (user is not null)
                    {
                        // 判断当前用户是否已生成 Token
                        if (!user.HasValidToken)
                        {
                            user.GenerateToken(Array.ConvertAll((await sqlUserData.GetAsync(u => u.HasValidToken)).ToArray(), u => u.Token));
                            await sqlUserData.UpdateAsync(user);
                        }
                        await Request.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                              new ClaimsPrincipal(user.ClaimsIdentify),
                                                              GlobalValues.AuthenticationProperties);
                        return new ApiResult<object>(ApiStatusCode.Success, "Login success");
                    }
                    return new ApiResult<object>(ApiStatusCode.AccountNotExist | ApiStatusCode.OperationDenied, "Account not exist or api inavailable");
                }
            }
            catch (Exception e)
            {
                _logger.Error($"{email} login failed", e);
                return new ApiResult<object>(ApiStatusCode.ServerInternalError);
            }
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="email">用户邮箱</param>
        /// <returns></returns>
        [HttpGet("user/{email}")]
        [Authorize]
        public async ValueTask<ApiResult<UserDTOEntity?>> GetUser(string email)
        {
            try
            {
                // 检查 Cookie 信息
                // 账户封禁、权限更改等均会清空当前用户数据库中的令牌，造成之前获取的 Cookie 失效
                var sqlUserData = new SqlUserData(_context);
                var signedUser = Request.HttpContext.User.Identities.First().GetUserClaimEntity();
                var realtimeUser = await sqlUserData.FindAsync(u => (u.Token == signedUser.Token) && u.IsApiAvailable && u.State == State.Normal);
                if((realtimeUser is null) || !realtimeUser.HasValidToken)
                {
                    await Request.HttpContext.SignOutAsync();
                    return new ApiResult<UserDTOEntity?>(ApiStatusCode.AccountStateChanged | ApiStatusCode.OperationDenied, "Account state changed or api unavailable");
                }

                // 返回用户信息
                return new ApiResult<UserDTOEntity?>(ApiStatusCode.Success, "Success to get user info", 
                       (string.IsNullOrEmpty(email) || realtimeUser.Email == email) ? realtimeUser.DTO : (await sqlUserData.FindAsync(u => (u.Email == email) && (realtimeUser.Permission > u.Permission)))?.DTO);

            }
            catch (Exception e)
            {
                _logger.Error($"Fail to get {email}'s info", e);
                return new ApiResult<UserDTOEntity?>(ApiStatusCode.ServerInternalError);
            }
        }


        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("users")]
        [Authorize]
        public async ValueTask<ApiResult<IEnumerable<UserDTOEntity>>> GetUsers()
        {
            try
            {
                // 检查 Cookie 信息
                // 账户封禁、权限更改等均会清空当前用户数据库中的令牌，造成之前获取的 Cookie 失效
                var sqlUserData = new SqlUserData(_context);
                var signedUser = Request.HttpContext.User.Identities.First().GetUserClaimEntity();
                var realtimeUser = await sqlUserData.FindAsync(u => (u.Token == signedUser.Token) && u.IsApiAvailable && u.State == State.Normal);
                if ((realtimeUser is null) || !realtimeUser.HasValidToken)
                {
                    await Request.HttpContext.SignOutAsync();
                    return new ApiResult<IEnumerable<UserDTOEntity>>(ApiStatusCode.AccountStateChanged | ApiStatusCode.OperationDenied, "Account state changed or api unavailable");
                }

                // 返回用户信息
                return new ApiResult<IEnumerable<UserDTOEntity>>(ApiStatusCode.Success, "Success to get users", 
                    Array.ConvertAll((await sqlUserData.GetAsync(u => u.Permission < realtimeUser.Permission)).ToArray(), u => u.DTO).Append(realtimeUser.DTO));
            }
            catch(Exception e)
            {
                _logger.Error("Fail to get users", e);
                return new ApiResult<IEnumerable<UserDTOEntity>>(ApiStatusCode.ServerInternalError);
            }
        }
    }
}