using North.Core.Entities;
using System.Security.Claims;

namespace North.Core.Helpers
{
    public static class ClaimHelper
    {
        /// <summary>
        /// 从 Claims 中获取用户实体
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static UserClaimEntity GetClaimEntity(this ClaimsIdentity identity)
        {
            var id = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value ?? string.Empty;
            var name = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
            var email = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;
            var role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
            var avatar = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value ?? string.Empty;
            return new UserClaimEntity(id, name, email, role, avatar);
        }
    }
}