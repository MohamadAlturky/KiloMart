using Dapper;
using KiloMart.Core.Contracts;

namespace KiloMart.Domain.Register.Services;

public class ActivateUserService
{

    // activate user
    public static async Task<bool> ActivateUser(string email, string verificationToken, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            // Step 1: Check if there’s a valid VerificationToken for the provided email and token value
            var membershipUserId = await connection.QueryFirstOrDefaultAsync<int?>(
                @"SELECT MU.Id 
                  FROM MembershipUser MU
                  JOIN VerificationToken VT ON VT.MembershipUser = MU.Id
                  WHERE MU.Email = @Email AND VT.Value = @Token",
                new { Email = email, Token = verificationToken },
                transaction
            );

            // If no matching record is found, return false (activation failed)
            if (membershipUserId is null)
            {
                return false;
            }

            // Step 2: Update the MembershipUser record to set IsActive = true and EmailConfirmed = true
            await connection.ExecuteAsync(
                @"UPDATE MembershipUser 
                  SET IsActive = 1, EmailConfirmed = 1 
                  WHERE Id = @UserId",
                new { UserId = membershipUserId },
                transaction
            );
            transaction.Commit();
            // Step 3: Return true to indicate successful activation
            return true;
        }
        catch (Exception)
        {
            transaction.Rollback();
            return false;
        }
    }
}
