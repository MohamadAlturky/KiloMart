using Dapper;
using KiloMart.Authentication.Handlers;
using KiloMart.Authentication.Models;
using KiloMart.Authentication.Utils;
using KiloMart.DataAccess.Contracts;
using Microsoft.Extensions.Configuration;

namespace KiloMart.Authentication.Services;

public static class RegisterService
{
    public static async Task<RegisterResult> RegisterDelivery(
        IDbFactory dbFactory,
        IConfiguration configuration,
        string email,
        string password)
    {
        return await Register(dbFactory, configuration, email, password, UserRole.DeliveryPerson);
    }

    public static async Task<RegisterResult> RegisterProvider(
        IDbFactory dbFactory,
        IConfiguration configuration,
        string email,
        string password)
    {
        return await Register(dbFactory, configuration, email, password, UserRole.Provider);
    }

    private static async Task<RegisterResult> Register(
        IDbFactory dbFactory,
        IConfiguration configuration,
        string email,
        string password,
        UserRole role)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        
        using var transaction = connection.BeginTransaction();
        try
        {
            // Check if email already exists
            var existingUser = await connection.QueryFirstOrDefaultAsync<MembershipUser>(
                "SELECT Email FROM MembershipUser WHERE Email = @Email",
                new { Email = email },
                transaction
            );

            if (existingUser != null)
            {
                return new RegisterResult
                {
                    Success = false,
                    Message = "Email already registered"
                };
            }

            // Create new user
            var newUser = new MembershipUser
            {
                Email = email,
                PasswordHash = HashHandler.GetHash(password),
                Role = (int)role,
                IsActive = false,
                IsEmailConfirmed = false
            };

            // Insert user into database
            var userId = await connection.QuerySingleAsync<int>(@"
                INSERT INTO MembershipUser (Email, PasswordHash, FullName, Phone, Role, IsActive, CreatedAt)
                VALUES (@Email, @PasswordHash, @FullName, @Phone, @Role, @IsActive, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)",
                newUser,
                transaction
            );

            // return the id of the party
            var partyId = await connection.QuerySingleAsync<int>(@"
                INSERT INTO Party (MembershipUserId, CreatedAt)
                VALUES (@UserId, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)",
                new { UserId = userId, CreatedAt = DateTime.UtcNow },
                transaction
            );

            transaction.Commit();
            return new RegisterResult
            {
                Success = true,
                Message = "Registration successful",
            };
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}