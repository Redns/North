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
        /// 检验用户授权
        /// </summary>
        /// <param name="context"></param>
        /// <param name="relativeUri"></param>
        /// <returns>返回用户当前授权状态</returns>
        public async ValueTask<AuthState> AuthAsync(HttpContext context, string? relativeUri = null)
        {
            /* 获取当前地址并判断是否需要校验 */
            if (relativeUri?.Contains(_withoutAuthenticationPages, true) is true)
            {
                return AuthState.Valid;
            }

            /* 查询用户并检测状态 */
            if (context.User.Identity?.IsAuthenticated is not true)
            {
                return AuthState.None;
            }

            // 解析 ClaimIdentifies 中的用户信息
            var userId = context.User.FindFirst(ClaimTypes.SerialNumber)?.Value;
            var userLastModifyTime = context.User.FindFirst("LastModifyTime")?.Value;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userLastModifyTime))
            {
                return AuthState.Expired;
            }

            // 核对云端认证信息
            var user = await _repository.SingleAsync(u => u.Id.ToString() == userId);
            if ((user?.LastModifyTime.ToString("G") != userLastModifyTime) || (user.State is not UserState.Normal))
            {
                return AuthState.Expired;
            }

            return AuthState.Valid;
        }

        /// <summary>
        /// 检验用户授权（默认当前页面需要授权）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="redirectUri"></param>
        /// <returns>授权有效则返回用户 DTO 实体，否则返回 null</returns>
        public async ValueTask<UserDTOEntity?> AuthAndQueryAsync(HttpContext context)
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
    }
}
