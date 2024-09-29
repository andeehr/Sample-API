using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Sample.Api.Helpers;
using Sample.Common.DTOs;
using Sample.Common.Exceptions;
using System.Security.Claims;

namespace Sample.Api.Middlewares
{
    public class JwtMiddleware : IMiddleware
    {
        private readonly IJwtTokenWrapper _jwtTokenWrapper;

        public JwtMiddleware(IJwtTokenWrapper jwtTokenWrapper)
        {
            _jwtTokenWrapper = jwtTokenWrapper ?? throw new ArgumentNullException(nameof(jwtTokenWrapper));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            if (!token.IsNullOrEmpty())
            {
                try
                {
                    var claims = _jwtTokenWrapper.ReadJwtToken(token).Claims;
                    var userInfo = GetUserInfo(ToDictionary(claims));

                    context.Items["UserInfo"] = userInfo;
                }
                catch (SecurityTokenExpiredException)
                {
                    throw new UnauthorizedException("Token expired");
                }
                catch (Exception ex)
                {
                    throw new UnauthorizedException($"An error occurred while reading the token: {ex.Message}");
                }
            }
            await next(context);
        }

        private object GetValue(IEnumerable<Claim> claims)
        {
            var claimList = claims.Select(c => c.Value).ToList();
            return claimList.Count == 1 ? claimList.First() : claimList;
        }

        private IDictionary<string, object> ToDictionary(IEnumerable<Claim> claims)
            => claims.GroupBy(x => x.Type).ToDictionary(x => x.Key, GetValue);

        private UserInfo GetUserInfo(IDictionary<string, object> claims)
        {
            return new()
            {
                Username = claims[ClaimTypes.NameIdentifier].ToString()!,
                Permissions = claims["permissions"] as IEnumerable<string> ?? JsonConvert.DeserializeObject<List<string>>(claims["permissions"].ToString()!),
                Role = claims[ClaimTypes.Role].ToString()!
            };
        }
    }
}