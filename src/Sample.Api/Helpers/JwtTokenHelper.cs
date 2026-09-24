using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Sample.Api.Helpers
{
    public static class JwtTokenHelper
    {
        public static TokenValidationParameters CreateParameters(string issuer, string audience, string key)
        {
            return new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
            };
        }
    }
}