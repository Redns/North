//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using North.Common;
//using North.Core.Entities;

//namespace North.Controllers
//{
//    /// <summary>
//    /// 认证授权接口
//    /// </summary>
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AuthController : ControllerBase
//    {
//        /// <summary>
//        /// 用户登录
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost("login")]
//        [AllowAnonymous]
//        public async ValueTask<ApiResult<object>> UserLogin()
//        {
            
//        }


//        /// <summary>
//        /// 获取用户信息
//        /// </summary>
//        /// <param name="email">用户邮箱</param>
//        /// <returns></returns>
//        [HttpGet("user/{email}")]
//        [Authorize]
//        public async ValueTask<ApiResult<UserDTOEntity?>> GetUser(string email)
//        {
            
//        }


//        /// <summary>
//        /// 获取所有用户信息
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet("users")]
//        [Authorize(Roles = "System,Administrator")]
//        public async ValueTask<ApiResult<IEnumerable<UserDTOEntity>>> GetUsers()
//        {
            
//        }
//    }
//}