using Microsoft.AspNetCore.Http;
using North.Core.Entities;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Linq.Expressions;
using System.Security.Claims;

namespace North.Core.Helpers
{
    public static class AuthorizationHelper
    {
        /// <summary>
        /// 从 Claims 中获取用户实体
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static UserClaimEntity GetUserClaimEntity(this ClaimsIdentity identity)
        {
            var email = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;
            var token = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value ?? string.Empty;
            var role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
            return new UserClaimEntity(email, token, role);
        }
    }
}