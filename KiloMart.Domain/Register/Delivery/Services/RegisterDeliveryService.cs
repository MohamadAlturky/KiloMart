using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.Languages.Models;
using KiloMart.Domain.Register.Utils;
using Microsoft.Extensions.Configuration;

namespace KiloMart.Domain.Register.Delivery.Services;

public static class RegisterDeliveryService
{
    public static async Task<RegisterResult> Register(
        IDbFactory dbFactory,
        IConfiguration configuration,
        string email,
        string password,
        string displayName,
        Language language)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        var user = await connection.QueryFirstOrDefaultAsync<MembershipUserIdDto>(   
            "SELECT Id FROM MembershipUser WHERE Email = @Email",
            new { Email = email },
            transaction
        );

        if (user is not null)
        {
            return new RegisterResult { IsSuccess = false, ErrorMessage = "User already exists" };
        }
        try
        {
            // TODO: 
            // Insert into Party returning Id (Id is auto incremented)
            // the columns are DisplayName, IsActive
            var partyId = await connection.QuerySingleAsync<int>(
                @"
                INSERT INTO Party (DisplayName, IsActive) 
                OUTPUT INSERTED.Id 
                VALUES (@DisplayName, @IsActive);",
                new { DisplayName = displayName, IsActive = false },
                transaction
            );
            // TODO: Insert into Delivery the PartyId
            // the columns are PartyId
            await connection.ExecuteAsync(
                @"INSERT INTO Delivery (Party) VALUES (@Party)",
                new { Party = partyId },
                transaction
            );

            // TODO: Insert into PartyLocalized Party,Language,DisplayName
            // the columns are Party, Language, DisplayName
            await connection.ExecuteAsync(
                @"INSERT INTO PartyLocalized (Party, Language, DisplayName) VALUES (@Party, @Language, @DisplayName)",
                new { Party = partyId, Language = (int)language, DisplayName = displayName },
                transaction
            );


            // hash the password
            var passwordHash = HashHandler.GetHash(password);

            
            // TODO: Insert into MembershipUser
            // return Id of the inserted row
            var membershipUserId = await connection.QuerySingleAsync<int>(
                @"
                INSERT INTO MembershipUser (Email,EmailConfirmed, PasswordHash, Role, Party) 
                OUTPUT INSERTED.Id 
                VALUES (@Email, @EmailConfirmed, @PasswordHash, @Role, @Party)",
                new { Email = email, EmailConfirmed = false, PasswordHash = passwordHash, Role = (int)UserRole.Delivery, Party = partyId },
                transaction
            );

            transaction.Commit();

            return new RegisterResult { IsSuccess = true, UserId = membershipUserId, PartyId = partyId };
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return new RegisterResult { IsSuccess = false, ErrorMessage = ex.Message };
        }
    }
}
