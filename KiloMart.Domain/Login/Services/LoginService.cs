using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Login.Handlers;
using KiloMart.Domain.Login.Models;
using KiloMart.Domain.Register.Utils;
using Microsoft.Extensions.Configuration;

namespace KiloMart.Domain.Login.Services;

public class LoginService
{
    public static async Task<LoginResult> Login(string email, string password, IDbFactory dbFactory, IConfiguration configuration)
    {
        var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var user = await connection.QueryFirstOrDefaultAsync<MembershipUserDto>(
                @"SELECT Id, EmailConfirmed, PasswordHash, IsActive, Role, Party, Email
                  FROM MembershipUser
                  WHERE Email = @Email",
                new { Email = email }
            );


        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return new LoginResult { Success = false, Errors = ["Invalid email or password."] };
        }
        if (!user.IsActive)
        {
            return new LoginResult { Success = false, Errors = ["User is not active."] };
        }
        if (!user.EmailConfirmed)
        {
            return new LoginResult { Success = false, Errors = ["Email is not confirmed."] };
        }

        // var token = JwtTokenHandler.GenerateAccessToken(user, configuration);
        var token = JwtTokenHandler.GenerateJwtToken(user, configuration);
        return new LoginResult
        {
            Success = true,
            Token = token,
            UserName = user.Email,
            Email = user.Email,
            Role = CheckRole(user.Role)
        };
    }

    private static string CheckRole(short role)
    {
        if (role == (byte)Roles.Customer)
        {
            return "Customer";
        }
        if (role == (byte)Roles.Admin)
        {
            return "Admin";
        }
        if (role == (byte)Roles.Provider)
        {
            return "Provider";
        }
        if (role == (byte)Roles.Delivery)
        {
            return "Delivery";
        }
        throw new Exception("Not Supported Role");
    }

    private static bool VerifyPassword(string inputPassword, string storedPasswordHash)
    {
        var hash = HashHandler.GetHash(inputPassword);
        return hash == storedPasswordHash;
    }
}

