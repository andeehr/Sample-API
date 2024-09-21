using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Sample.Common.DTOs;
using Sample.Common.Exceptions;

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

        public PermissionFilter(string[] permission, IOptionsSnapshot<JwtOptions> jwtOptions)
        {
            _permission = permission ?? throw new ArgumentNullException(nameof(permission));
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_jwtOptions.Enabled)
                return;

            var userInfo = context.HttpContext.Items["UserInfo"] as UserInfo;

            if (userInfo?.Username is null)
                throw new UnauthorizedException();

            var hasPermission = _permission.All(p => userInfo?.Permissions?.Contains(p) ?? false);

            if (!hasPermission)
                throw new ForbiddenException();
        }
    }
}