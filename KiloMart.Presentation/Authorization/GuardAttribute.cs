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

    private readonly List<byte> _roles;

    public GuardAttribute(Roles[] roles)
    {
        _roles = [];
        foreach(var role in roles)
        {
            _roles.Add((byte)role);
        }

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
            if (roleIdClaim == null || !byte.TryParse(roleIdClaim.Value, out byte roleIdFromToken))
            {
                context.Result = new ForbidResult();
                return;
            }
            bool roleExists = false;
            foreach(var role in _roles)
            {
                if(role == roleIdFromToken)
                {
                    roleExists = true;
                    break;
                }
            }
            if (!roleExists)
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
