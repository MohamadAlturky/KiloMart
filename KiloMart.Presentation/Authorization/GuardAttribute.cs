using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using KiloMart.Domain.Login.Models;
using KiloMart.Domain.Register.Utils;

namespace KiloMart.Presentation.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class GuardAttribute : Attribute, IAuthorizationFilter
{
    public static string SECRET_KEY = "";
    public static string ISSUER = "";
    public static string AUDIENCE = "";

    private readonly int _requiredRoleId;

    public GuardAttribute(Roles userRole)
    {
        _requiredRoleId = (byte)userRole;
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
            bool decodingResult = JwtTokenValidator.ValidateToken(token, SECRET_KEY, ISSUER, AUDIENCE, out JwtSecurityToken? decodedToken);
            if(!decodingResult || decodedToken is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            

            var roleIdClaim = decodedToken?.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Role);
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
