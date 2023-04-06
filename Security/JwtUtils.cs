
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookStoreApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace BookStoreApi.Security;

public static class JwtUtils
{

    private static readonly IConfiguration _configuration;
    public static readonly string secret;

    static JwtUtils()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        secret = _configuration["Jwt:Key"]!;
        System.Console.WriteLine(secret);
    }

    public static string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        System.Console.WriteLine("key: " + key.ToString());
        System.Console.WriteLine("secret: " + secret);
        var claims = new[]
        {

           new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]!),
           new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
           new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
           new Claim("userid", user.Id!.ToString()),
           new Claim("Email", user.Email),
           new Claim("Role", user.Roles[0])
        };
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            notBefore: DateTime.UtcNow,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
        return tokenHandler.WriteToken(token);
    }

    public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}