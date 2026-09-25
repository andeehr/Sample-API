using Sample.Common.DTOs.Responses;
using Sample.Core.Services.Interfaces;

namespace Sample.Api.Helpers
{
    public interface IAuthManager
    {
        Task<UserResponse> AuthenticateAsync(HttpContext httpContext, string username, string password);
        void Logout(HttpContext httpContext);
    }

    public class AuthManager : IAuthManager
    {
        private readonly IUserService _usuarioService;
        private readonly IJwtTokenWrapper _jwtTokenWrapper;

        public AuthManager(IUserService usuarioService, IJwtTokenWrapper jwtTokenWrapper)
        {
            _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
            _jwtTokenWrapper = jwtTokenWrapper ?? throw new ArgumentNullException(nameof(jwtTokenWrapper));
        }

        public async Task<UserResponse> AuthenticateAsync(HttpContext httpContext, string username, string password)
        {
            var user = await _usuarioService.LoginAsync(username, password);
            var token = _jwtTokenWrapper.WriteJwtToken(username, user.Role, user.Permissions);

            var authCookie = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTime.UtcNow.AddHours(24),
            };

            httpContext.Response.Cookies.Append("auth-token", token, authCookie);
            return user;
        }

        public void Logout(HttpContext httpContext)
        {
            ArgumentNullException.ThrowIfNull(httpContext);

            httpContext.Response.Cookies.Delete("auth-token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
            });
        }
    }
}
