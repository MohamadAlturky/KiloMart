using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using KiloMart.Domain.Login.Models;
using KiloMart.Domain.Register.Utils;
using Microsoft.Data.SqlClient;
using KiloMart.DataAccess.Database;
using KiloMart.Core.Authentication;
using Dapper;

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

        var user = context.HttpContext?.User;

        var userPayLoad = new UserPayLoad
        {
            Id = int.Parse(user?.FindFirst(CustomClaimTypes.UserId)?.Value ?? "0"),
            Role = int.Parse(user?.FindFirst(CustomClaimTypes.Role)?.Value ?? "0"),
            Party = int.Parse(user?.FindFirst(CustomClaimTypes.Party)?.Value ?? "0"),
            Email = user?.FindFirst(CustomClaimTypes.Email)?.Value ?? string.Empty,
            Code = user?.FindFirst(CustomClaimTypes.Code)?.Value ?? string.Empty
        };


        // System.Console.WriteLine(userPayLoad.Code);
        // System.Console.WriteLine(userPayLoad.Id);
        // System.Console.WriteLine(userPayLoad.Party);
        // System.Console.WriteLine(userPayLoad.Email);
        // System.Console.WriteLine(userPayLoad.Role);


        using var connection = new SqlConnection(CONNECTION_STRING);
        connection.Open();

        var query = @"SELECT 
                        [Id], 
                        [Token], 
                        [UserId], 
                        [ExpireDate], 
                        [CreationDate], 
                        [Code]
                        FROM [dbo].[Sessions]
                        WHERE [Code] = @Code";
        Sessions? session = connection.QueryFirstOrDefault<Sessions>(query, new
        {
            Code = userPayLoad.Code
        });

        if (session is null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (session.ExpireDate <= DateTime.Now)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        bool roleExists = false;
        foreach (var role in _roles)
        {
            if (role == userPayLoad.Role)
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
}
