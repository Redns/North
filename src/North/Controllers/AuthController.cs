using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using North.Common;
using North.Core.Entities;
using North.Core.Repository;
using SqlSugar;
using System.Security.Claims;
using ILogger = North.Core.Services.Logger.ILogger;

namespace North.Controllers
{
    /// <summary>
    /// 认证授权接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ISqlSugarClient _client;
        private readonly IHttpContextAccessor _accessor;

        public AuthController(ILogger logger, ISqlSugarClient client, IHttpContextAccessor accessor)
        {
            _logger = logger;
            _client = client;
            _accessor = accessor;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async ValueTask<ApiResult<object>> Login()
        {
            try
            {
                var userAccount = Request.Headers["Account"].ToString();
                var userEncryptedPassword = Request.Headers["Password"].ToString();
                if (!string.IsNullOrEmpty(userAccount) && !string.IsNullOrEmpty(userEncryptedPassword))
                {
                    var userRepository = new UserRepository(_client, GlobalValues.AppSettings.General.DataBase.EnabledName);
                    var user = await userRepository.SingleAsync(u => u.Name == userAccount || u.Email == userAccount);
                    if ((user?.State is UserState.Normal) && (_accessor.HttpContext is not null))
                    {
                        await _accessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                                new ClaimsPrincipal(user.ClaimsIdentify),
                                                                new AuthenticationProperties()
                                                                {
                                                                    IsPersistent = true,
                                                                    ExpiresUtc = DateTime.Now.AddSeconds(GlobalValues.AppSettings.Auth.CookieValidTime)
                                                                });
                        return new ApiResult<object>(ApiStatusCode.Success, "Login successfully");
                    }
                    return new ApiResult<object>(ApiStatusCode.AccountStateAbnormal, "Account does not exist or status is abnormal");
                }
                return new ApiResult<object>(ApiStatusCode.ParamIncomplete, "Incomplete account or password");
            }
            catch(Exception e)
            {
                _logger.Error("User login failed", e);
                return new ApiResult<object>(ApiStatusCode.ServerInternalError);
            }
        }


        ///// <summary>
        ///// 获取用户信息
        ///// </summary>
        ///// <param name="email">用户邮箱</param>
        ///// <returns></returns>
        //[HttpGet("user/{email}")]
        //[Authorize(Roles = "System")]
        //public async ValueTask<ApiResult<UserDTOEntity?>> GetUser(string email)
        //{

        //}


        ///// <summary>
        ///// 获取所有用户信息
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("users")]
        //[Authorize(Roles = "System")]
        //public async ValueTask<ApiResult<IEnumerable<UserDTOEntity>>> GetUsers()
        //{

        //}
    }
}