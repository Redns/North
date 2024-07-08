using Microsoft.AspNetCore.Http;

namespace North.Core.Services.AuthService
{
    public interface IAuthService<UserDto> where UserDto : class
    {
        /// <summary>
        /// 检验用户授权
        /// </summary>
        /// <param name="context"></param>
        /// <param name="relativeUri"></param>
        /// <returns>返回用户当前授权状态</returns>
        ValueTask<AuthState> AuthAsync(HttpContext context, string? relativeUri = null);

        /// <summary>
        /// 检验用户授权（不检测白名单）
        /// </summary>
        /// <param name="context"></param>
        /// <returns>若授权有效则返回用户 DTO 实体，否则返回 null</returns>
        ValueTask<UserDto?> AuthAndQueryAsync(HttpContext context);
    }

    /// <summary>
    /// 用户授权状态
    /// </summary>
    public enum AuthState
    {
        None,           // 未授权
        Expired,        // 已过期
        Valid           // 有效
    }
}
