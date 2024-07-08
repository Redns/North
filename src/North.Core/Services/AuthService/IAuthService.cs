using Microsoft.AspNetCore.Http;

namespace North.Core.Services.AuthService
{
    public interface IAuthService<UserDto> where UserDto : class
    {
        ValueTask<bool> AuthAsync(HttpContext context, string? relativeUri = null);

        ValueTask<UserDto?> AuthAndQueryAsync(HttpContext context);
    }
}
