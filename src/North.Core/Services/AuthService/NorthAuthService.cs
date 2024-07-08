using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using North.Core.Entities;
using North.Core.Helpers;
using North.Core.Repository;
using System.Security.Claims;

namespace North.Core.Services.AuthService
{
    public class NorthAuthService : IAuthService<UserDTOEntity>
    {
        private readonly UserRepository _repository;
        private readonly string[] _withoutAuthenticationPages;

        public NorthAuthService(UserRepository repository, string[] withoutAuthenticationPages)
        {
            _repository = repository;
            _withoutAuthenticationPages = withoutAuthenticationPages;
        }

        /// <summary>
        /// 查询认证用户信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="queryUserDelegate"></param>
        /// <returns>认证信息过期或用户不存在返回 null</returns>
        private async ValueTask<UserDTOEntity?> GetAuthUserAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated is not true)
            {
                return null;
            }

            // 解析 ClaimIdentifies 中的用户信息
            var userId = context.User.FindFirst(ClaimTypes.SerialNumber)?.Value;
            var userLastModifyTime = context.User.FindFirst("LastModifyTime")?.Value;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userLastModifyTime))
            {
                return null;
            }

            // 核对云端认证信息
            var user = await _repository.SingleAsync(u => u.Id.ToString() == userId);
            if ((user?.LastModifyTime.ToString("G") != userLastModifyTime) || (user.State is not UserState.Normal))
            {
                return null;
            }

            return user.DTO;
        }

        /// <summary>
        /// 认证用户
        /// </summary>
        /// <param name="context"></param>
        /// <param name="relativeUri">当前相对路径</param>
        /// <returns></returns>
        public async ValueTask<bool> AuthAsync(HttpContext context, string? relativeUri = null)
        {
            // 获取当前地址并判断是否需要校验
            if (relativeUri?.Contains(_withoutAuthenticationPages, true) is true)
            {
                return true;
            }

            // 查询用户
            var user = await GetAuthUserAsync(context);
            return user is not null;
        }

        /// <summary>
        /// 认证用户（默认当前页面需要授权）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public async ValueTask<UserDTOEntity?> AuthAndQueryAsync(HttpContext context)
        {
            // 查询用户
            return await GetAuthUserAsync(context);
        }
    }
}
