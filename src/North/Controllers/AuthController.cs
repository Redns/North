using Microsoft.AspNetCore.Mvc;
using North.Common;
using North.Core.Data.Access;
using North.Core.Data.Entities;

namespace North.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Services.Logger.ILogger _logger;

        public AuthController(Services.Logger.ILogger logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <returns></returns>
        [HttpPost("generate_token")]
        public ApiResult<object> GenerateToken()
        {
            var email = Request.Headers["Email"].ToString();
            var password = Request.Headers["Password"].ToString();

            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    return new ApiResult<object>(300, "Request parameter missing", null);
                }
                else
                {
                    var user = GlobalValues.MemoryDatabase.Users.FirstOrDefault(u => u.Email == email);
                    if (user?.Password == password)
                    {
                        if (user.GenerateToken(GlobalValues.AppSettings.Api.TokenValidTime))
                        {
                            return new ApiResult<object>(200, "Token generate succeeded", new
                            {
                                user.Token,
                                user.TokenExpireTime
                            });
                        }
                        return new ApiResult<object>(302, "Account status is abnormal or API is disabled", null);
                    }
                    return new ApiResult<object>(301, "Account not exist or password not correct", null);
                }
            }
            catch(Exception e)
            {
                _logger.Error($"Get {email}'s token failed", e);
                return new ApiResult<object>(500, "Server Internal Error", null);
            }
        }


        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <returns></returns>
        [HttpPost("refresh_token")]
        public ApiResult<object> RefreshToken()
        {
            var token = Request.Headers["Token"].ToString();

            try
            {
                var user = GlobalValues.MemoryDatabase.Users.FirstOrDefault(u => u.Token == token);
                if (user?.IsTokenValid() is true)
                {
                    if (user.GenerateToken())
                    {
                        return new ApiResult<object>(200, "Token refresh succeeded", new
                        {
                            user.Token,
                            user.TokenExpireTime
                        });
                    }
                    return new ApiResult<object>(301, "Token refresh failed", null);
                }
                return new ApiResult<object>(300, "Token invalid", null);
            }
            catch(Exception e)
            {
                _logger.Error($"Refresh token {token} failed", e);
                return new ApiResult<object>(500, "Server Internal Error", null);
            }
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("user")]
        public ApiResult<UserDTOEntity> GetUser()
        {
            var token = Request.Headers["Token"].ToString();
            var target = Request.Headers["Target"].ToString();

            try
            {
                var user = GlobalValues.MemoryDatabase.Users.FirstOrDefault(u => u.Token == token);
                if (user?.IsTokenValid() is true)
                {
                    if (target == string.Empty)
                    {
                        // 目标为空，返回查找者自身信息
                        return new ApiResult<UserDTOEntity>(200, "User information obtain succeeded", user.ToDTO());
                    }
                    else
                    {
                        // 查找目标账号信息
                        var targetUser = GlobalValues.MemoryDatabase.Users.FirstOrDefault(u => (u.Name == target) || (u.Email == target));
                        if ((targetUser is not null) && (user.Permission > targetUser.Permission))
                        {
                            return new ApiResult<UserDTOEntity>(200, "User information obtain succeeded", targetUser.ToDTO());
                        }
                        return new ApiResult<UserDTOEntity>(301, "User not exist or permission denied", null);
                    }
                }
                return new ApiResult<UserDTOEntity>(300, "User not exist or token invalid", null);
            }
            catch(Exception e)
            {
                _logger.Error($"Get {(!string.IsNullOrEmpty(target) ? "user" : target)}'s information failed", e);
                return new ApiResult<UserDTOEntity>(500, "Server Internal Error", null);
            }
        }


        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("users")]
        public ApiResult<IEnumerable<UserDTOEntity>> GetUsers()
        {
            var token = Request.Headers["Token"].ToString();
            try
            {
                var user = GlobalValues.MemoryDatabase.Users.FirstOrDefault(u => u.Token == token);
                if (user?.IsTokenValid() is true)
                {
                    var users = Array.ConvertAll(GlobalValues.MemoryDatabase
                                                             .Users
                                                             .FindAll(u => u.Permission < user.Permission)
                                                             .Append(user)
                                                             .ToArray(),
                                                 u => u.ToDTO());
                    return new ApiResult<IEnumerable<UserDTOEntity>>(200, "User information obtained succeeded", users);
                }
                return new ApiResult<IEnumerable<UserDTOEntity>>(300, "User not exist or token invalid", null);
            }
            catch(Exception e)
            {
                _logger.Error("Get user's information failed", e);
                return new ApiResult<IEnumerable<UserDTOEntity>>(500, "Server Internal Error", null);
            }
        }
    }
}
