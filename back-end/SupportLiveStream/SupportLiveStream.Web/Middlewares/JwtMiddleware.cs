using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SupportLiveStream.Web.AppSettings;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportLiveStream.Web.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtConfig _jwtConfig;

        public JwtMiddleware(RequestDelegate next, IOptions<JwtConfig> options)
        {
            _next = next;
            _jwtConfig = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string token = String.Empty;
            if (!String.IsNullOrEmpty(context.Request.Headers["access_token"]))
            {
                token = context.Request.Headers["access_token"];
                ValidateToken(context, token);
            }
            else if (!String.IsNullOrEmpty(context.Request.Query["access_token"]))
            {
                token = context.Request.Query["access_token"];
                ValidateToken(context, token);
            }

            await _next(context);
        }

        private void ValidateToken(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtConfig.Key);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userName = jwtToken.Claims.First(x => x.Type == "usr").Value;
                var role = jwtToken.Claims.First(x => x.Type == "role").Value;

                context.Items["username"] = userName;
                context.Items["role"] = role;
            }
            catch (Exception ex)
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
