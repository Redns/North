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
using Microsoft.JSInterop;
using North.Core.Helpers;

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
        /// 用户登录授权接口
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
                    var user = await userRepository.SingleAsync(u => (u.Name == userAccount || u.Email == userAccount) && u.Password == userEncryptedPassword);
                    if (_accessor.HttpContext is not null && user?.State is UserState.Normal && user.IsApiAvailable)
                    {
                        await _accessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                                new ClaimsPrincipal(user.ClaimsIdentify),
                                                                new AuthenticationProperties()
                                                                {
                                                                    IsPersistent = true,
                                                                    ExpiresUtc = DateTime.Now.AddSeconds(GlobalValues.AppSettings.Auth.CookieValidTime)
                                                                });
                        await new UserLoginHistoryRepository(_client, GlobalValues.AppSettings.General.DataBase.EnabledName).AddAsync(new UserLoginHistoryEntity
                        {
                            DeviceName = $"API ({Request.Headers.UserAgent.ToString() ?? "Unknown"})",
                            IPAddress = _accessor.HttpContext?.Connection.RemoteIpAddress?.MapToIPv4()?.ToString() ?? "UnKnown",
                            UserId = user.Id
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


        /// <summary>
        /// 用户登陆历史查询接口
        /// </summary>
        /// <param name="account">待查询用户名或邮箱</param>
        /// <returns></returns>
        [HttpGet("user/login_history")]
        [Authorize(Roles = "System,User")]
        public async ValueTask<ApiResult<IEnumerable<UserLoginHistoryEntity>>> GetUserLoginHistories([FromQuery] string account)
        {
            try
            {
                // TODO 代码复用此处
                var userRepository = new UserRepository(_client, GlobalValues.AppSettings.General.DataBase.EnabledName);
                var currentOperateUserId = User.Identities.FirstOrDefault()?.FindFirst(ClaimTypes.SerialNumber)?.Value;
                var currentOperateUserLastModifyTime = User.Identities.FirstOrDefault()?.FindFirst("LastModifyTime")?.Value;
                var currentOperateUser = await userRepository.SingleAsync(u => u.Id.ToString() == currentOperateUserId);
                if(currentOperateUser is null)
                {
                    // 用户不存在
                    return new(ApiStatusCode.AccountNotExist, Enumerable.Empty<UserLoginHistoryEntity>());
                }
                else if(!currentOperateUser.IsApiAvailable)
                {
                    // 无权访问 API
                    return new(ApiStatusCode.OperationDenied, Enumerable.Empty<UserLoginHistoryEntity>());
                }
                else if((currentOperateUser.State is not UserState.Normal) || (currentOperateUser.LastModifyTime.ToString("G") != currentOperateUserLastModifyTime))
                {
                    // 用户状态异常或改变
                    return new(ApiStatusCode.AccountStateAbnormal, "User status has changed", Enumerable.Empty<UserLoginHistoryEntity>());
                }
                else
                {
                    // 当前登录有效
                    // 检查待查询用户是否存在
                    var queriedUser = await userRepository.GetList(u => u.Name == account || u.Email == account)
                                                          .Includes(u => u.LoginHistories)
                                                          .FirstAsync();
                    if(queriedUser is null)
                    {
                        // 待查询用户不存在
                        return new(ApiStatusCode.AccountNotExist, "The requested user does not exist", Enumerable.Empty<UserLoginHistoryEntity>());
                    }
                    else if ((currentOperateUser.Permission is UserPermission.System) || (queriedUser.Id.ToString() == currentOperateUserId))
                    {
                        // 当前登录权限为系统，允许查询所有成员
                        // 当前登录权限为用户，仅允许查询本人登陆历史
                        return new(ApiStatusCode.Success, "The user login history query succeeded", queriedUser.LoginHistories);
                    }
                    else
                    {
                        // 当前登录权限为普通用户且非查询本人
                        // 拒绝查询
                        return new(ApiStatusCode.PermissionDenied);
                    }
                }
            }
            catch(Exception e)
            {
                _logger.Error("Failed to get user login history", e);
                return new(ApiStatusCode.ServerInternalError, Enumerable.Empty<UserLoginHistoryEntity>());
            }
        }
    }
}