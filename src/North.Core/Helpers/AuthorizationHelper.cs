using Microsoft.AspNetCore.Http;
using North.Core.Entities;
using North.Core.Repository;
using System.Security.Claims;

namespace North.Core.Helpers
{
    public static class AuthorizationHelper
    {
        /// <summary>
        /// 用户授权判定
        /// </summary>
        /// <param name="context">http上下文</param>
        /// <param name="userRepository">用户仓储</param>
        /// <returns>授权有效返回true，否则返回false</returns>
        public static async ValueTask<bool> AuthAsync(this HttpContext context, UserRepository userRepository)
        {
            var userIdentify = context.User.Identities.FirstOrDefault();
            if (userIdentify?.IsAuthenticated is true)
            {
                // 解析 ClaimIdentifies 中的用户信息
                var userId = userIdentify.FindFirst(ClaimTypes.SerialNumber)?.Value;
                var userLastModifyTime = userIdentify.FindFirst("LastModifyTime")?.Value;
                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userLastModifyTime))
                {
                    // 检索用户
                    var user = await userRepository.SingleAsync(u => u.Id.ToString() == userId);
                    if ((user?.State is UserState.Normal) && (user.LastModifyTime.ToString("G") == userLastModifyTime))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
