using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using KiloMart.Domain.Login.Models;

namespace KiloMart.Presentation.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
{
    public static string SECRET_KEY = "";
    public static string ISSUER = "";
    public static string AUDIENCE = "";

    private readonly int _requiredRoleId;

    public AuthorizeRoleAttribute(int roleId)
    {
        _requiredRoleId = roleId;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var token = context.HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
        if (token is null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        try
        {

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY)),
                ValidateIssuer = true,
                ValidIssuer = ISSUER,
                ValidateAudience = true,
                ValidAudience = AUDIENCE,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;

            var roleIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Role);
            if (roleIdClaim == null || !int.TryParse(roleIdClaim.Value, out int roleIdFromToken))
            {
                context.Result = new ForbidResult();
                return;
            }

            if (roleIdFromToken != _requiredRoleId)
            {
                context.Result = new ForbidResult();
            }
        }
        catch (Exception)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
