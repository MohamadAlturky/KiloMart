using System.Data;
using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using Microsoft.Extensions.Configuration;

namespace KiloMart.Domain.Register.Services;

public abstract class BaseRegisterService
{
    protected abstract string PartyTypeTableName { get; }
    protected abstract Roles UserRole { get; }

    public async Task<RegisterResult> Register(
        IDbFactory dbFactory,
        IConfiguration configuration,
        string email,
        string password,
        string displayName)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            if (await UserExists(connection, email, transaction))
            {
                return new RegisterResult { IsSuccess = false, ErrorMessage = "User already exists" };
            }

            var partyId = await CreateParty(connection, displayName, transaction);
            await CreatePartyType(connection, partyId, transaction);
            var membershipUserId = await CreateMembershipUser(connection, email, password, UserRole, partyId, transaction);
            var verificationToken = await GenerateVerificationToken(membershipUserId, connection, transaction);
            transaction.Commit();
            return new RegisterResult { IsSuccess = true, UserId = membershipUserId, PartyId = partyId, VerificationToken = verificationToken };
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return new RegisterResult { IsSuccess = false, ErrorMessage = ex.Message };
        }
    }

    private static async Task<bool> UserExists(IDbConnection connection, string email, IDbTransaction transaction)
    {
        var user = await connection.QueryFirstOrDefaultAsync<MembershipUserIdDto>(
            "SELECT Id FROM MembershipUser WHERE Email = @Email",
            new { Email = email },
            transaction
        );
        return user is not null;
    }

    private static async Task<int> CreateParty(IDbConnection connection, string displayName, IDbTransaction transaction)
    {
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO Party (DisplayName, IsActive) 
            OUTPUT INSERTED.Id 
            VALUES (@DisplayName, @IsActive);",
            new { DisplayName = displayName, IsActive = true },
            transaction
        );
    }

    private async Task CreatePartyType(IDbConnection connection, int partyId, IDbTransaction transaction)
    {
        await connection.ExecuteAsync(
            $"INSERT INTO {PartyTypeTableName} (Party) VALUES (@Party)",
            new { Party = partyId },
            transaction
        );
    }


    private static async Task<int> CreateMembershipUser(IDbConnection connection, string email, string password, Roles role, int partyId, IDbTransaction transaction)
    {
        var passwordHash = HashHandler.GetHash(password);
        return await connection.QuerySingleAsync<int>(
            @"INSERT INTO MembershipUser (Email, EmailConfirmed, PasswordHash, Role, Party,IsActive) 
            OUTPUT INSERTED.Id 
            VALUES (@Email, @EmailConfirmed, @PasswordHash, @Role, @Party, @IsActive)",
            new { Email = email, EmailConfirmed = false, PasswordHash = passwordHash, Role = (byte)role, Party = partyId, IsActive = false },
            transaction
        );
    }

    public async Task<string> GenerateVerificationToken(int membershipUserId, IDbConnection connection, IDbTransaction transaction)
    {
        // Generate a 5-digit random number as a verification token
        Random random = new Random();
        string token = random.Next(10000, 99999).ToString(); // Generates a 5-digit number as a string

        // Insert the token into the VerificationToken table
        await connection.ExecuteAsync(
            @"INSERT INTO VerificationToken (MembershipUser, Value, CreatedAt)
                VALUES (@MembershipUser, @Value, @CreatedAt)",
            new { MembershipUser = membershipUserId, Value = token, CreatedAt = DateTime.UtcNow },
            transaction
        );

        // Return the generated token
        return token;
    }
}