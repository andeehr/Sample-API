using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Sample.Api.Helpers;
using Sample.Common.Exceptions;
using System.Security.Claims;

namespace Sample.Api.Config
{
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute(params string[] permissions) : base(typeof(PermissionFilter))
        {
            Arguments = [permissions];
        }
    }

    public class PermissionFilter : IAuthorizationFilter
    {
        private readonly string[] _permission;
        private readonly JwtOptions _jwtOptions;
        private readonly IJwtTokenWrapper _jwtTokenWrapper;

        public PermissionFilter(string[] permission, IOptionsSnapshot<JwtOptions> jwtOptions, IJwtTokenWrapper jwtTokenWrapper)
        {
            _permission = permission ?? throw new ArgumentNullException(nameof(permission));
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            _jwtTokenWrapper = jwtTokenWrapper ?? throw new ArgumentNullException(nameof(jwtTokenWrapper));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_jwtOptions.Enabled)
                return;

            var token = context.HttpContext.Request.Cookies["auth-token"]
                ?? throw new UnauthorizedException();

            var claims = new Dictionary<string, object>();

            try
            {
                var result = _jwtTokenWrapper.ReadJwtToken(token).Claims;
                claims = result.GroupBy(x => x.Type).ToDictionary(x => x.Key, GetValue);
            }
            catch (SecurityTokenExpiredException)
            {
                throw new UnauthorizedException("Expired token");
            }
            catch (Exception ex)
            {
                throw new UnauthorizedException($"An error occurred while reading the token: {ex.Message}");
            }

            var permissions = claims["permissions"] as IEnumerable<string> ?? JsonConvert.DeserializeObject<List<string>>(claims["permissions"].ToString()!);

            var hasPermission = _permission.All(p => permissions?.Contains(p) ?? false);

            if (!hasPermission)
                throw new ForbiddenException();
        }

        private object GetValue(IEnumerable<Claim> claims) => claims.Select(c => c.Value);
    }
}