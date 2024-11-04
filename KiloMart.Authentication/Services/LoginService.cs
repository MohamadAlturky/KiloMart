using Dapper;
using KiloMart.Authentication.Handlers;
using KiloMart.Authentication.Models;
using KiloMart.Authentication.Utils;
using KiloMart.DataAccess.Contracts;
using Microsoft.Extensions.Configuration;

namespace KiloMart.Authentication.Services;

public static class LoginService
{
    public static async Task<LoginResult> Login(
        IDbFactory dbFactory,
        IConfiguration configuration,
        string email,
        string password)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var user = await connection.QueryFirstOrDefaultAsync<MembershipUser>(
            "SELECT * FROM MembershipUser WHERE Email = @Email",
            new { Email = email }
        );

        if (user is null)
        {
            return new LoginResult
            {
                Success = false,
                Message = "User not found"
            };
        }
        if (!user.IsActive)
        {
            return new LoginResult
            {
                Success = false,
                Message = "User is blocked"
            };
        }

        if (user.PasswordHash != HashHandler.GetHash(password))
        {
            return new LoginResult
            {
                Success = false,
                Message = "Invalid password"
            };
        }

        var token = new JwtToken(JwtTokenHandler.GenerateAccessToken(user,configuration));

        return new LoginResult
        {
            Success = true,
            JwtToken = token
        };
    }
}
