using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using KiloMart.Authentication.Models;

namespace KiloMart.Authentication.Handlers;
public static class JwtTokenHandler
{
    public static string GenerateAccessToken(MembershipUser user)
    {
        var claims = new[]
        {
            new Claim(CustomClaimTypes.UserId, user.Id.ToString()),
            new Claim(CustomClaimTypes.Email, user.Email),
            new Claim(CustomClaimTypes.Role, user.Role.ToString()),
            new Claim(CustomClaimTypes.Party, user.Party.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key-here"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "KiloMart",
            audience: "KiloMart",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
