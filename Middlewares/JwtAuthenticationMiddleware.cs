
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BookStoreApi.Security;
using Microsoft.IdentityModel.Tokens;

namespace BookStoreApi.Middlewares;

public class JwtAuthenticationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token is not null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtUtils.secret));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "userid").Value);
                context.Items["User"] = userId;
            }
            catch (System.Exception)
            {
                // return unauthorized if token validation fails
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
        }
        await next(context);
    }
}
