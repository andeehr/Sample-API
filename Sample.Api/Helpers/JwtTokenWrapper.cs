using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sample.Api.Config;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sample.Api.Helpers
{
    public interface IJwtTokenWrapper
    {
        string WriteJwtToken(string username, string rol, IEnumerable<string> permisos);

        ClaimsPrincipal ReadJwtToken(string token);
    }

    public class JwtTokenWrapper : IJwtTokenWrapper
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly JwtOptions _jwtOptions;

        public JwtTokenWrapper(IOptionsSnapshot<JwtOptions> jwtOptions, JwtSecurityTokenHandler jwtSecurityTokenHandler)
        {
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler ?? throw new ArgumentNullException(nameof(jwtSecurityTokenHandler));
        }

        public string WriteJwtToken(string username, string role, IEnumerable<string> permissions)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, username),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, role)
            };

            claims.AddRange(permissions.Select(permission => new Claim("permission", permission)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpirationHours),
                signingCredentials: creds);

            return _jwtSecurityTokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal ReadJwtToken(string token)
        {
            var parameters = JwtTokenHelper.CreateParameters(_jwtOptions.Issuer, _jwtOptions.Audience, _jwtOptions.JwtKey);
            return _jwtSecurityTokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);
        }
    }
}