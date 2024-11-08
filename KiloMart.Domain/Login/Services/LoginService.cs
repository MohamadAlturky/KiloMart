using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Dapper;
using KiloMart.DataAccess.Contracts;
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
                @"SELECT Id, EmailConfirmed, PasswordHash, IsActive, Role,Party
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

        var token = JwtTokenHandler.GenerateAccessToken(user, configuration);
        return new LoginResult { Success = true, Token = token };
    }

    private static bool VerifyPassword(string inputPassword, string storedPasswordHash)
    {
        var hash = HashHandler.GetHash(inputPassword);
        return hash == storedPasswordHash;
    }

}

