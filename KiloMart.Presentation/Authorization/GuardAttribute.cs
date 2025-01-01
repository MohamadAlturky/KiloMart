using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using KiloMart.Domain.Login.Models;
using KiloMart.Domain.Register.Utils;
using Microsoft.Data.SqlClient;
using KiloMart.DataAccess.Database;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KiloMart.Presentation.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class GuardAttribute : Attribute, IAuthorizationFilter
{
    public static string SECRET_KEY = "";
    public static string ISSUER = "";
    public static string AUDIENCE = "";
    public static string CONNECTION_STRING = "";

    private readonly List<byte> _roles;

    public GuardAttribute(Roles[] roles)
    {
        _roles = [];
        foreach (var role in roles)
        {
            _roles.Add((byte)role);
        }

    }

    public async void OnAuthorization(AuthorizationFilterContext context)
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
            if (!decodingResult || decodedToken is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            var roleIdClaim = decodedToken?.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Role);

            var codeClaim = decodedToken?.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Code);
            if (codeClaim is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            using var connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();
            Sessions? session = await Db.GetSessionByCodeAsync(connection, codeClaim.Value);
            if (session is null)
            {
                context.Result = new JsonResult("Your Session Is Not Active");
                return;
            }
            if (session.ExpireDate <= DateTime.Now)
            {
                context.Result = new JsonResult("Your Session Expired");
                return;
            }

            if (roleIdClaim == null || !byte.TryParse(roleIdClaim.Value, out byte roleIdFromToken))
            {
                context.Result = new ForbidResult();
                return;
            }
            bool roleExists = false;
            foreach (var role in _roles)
            {
                if (role == roleIdFromToken)
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
