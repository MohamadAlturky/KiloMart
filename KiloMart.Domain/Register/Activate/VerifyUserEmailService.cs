using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;

namespace KiloMart.Domain.Register.Activate;

public class UserIdRoleDto
{
    public int Id { get; set; }
    public byte Role { get; set; }
}
public class VerifyUserEmailService
{

    // activate user
    public static async Task<bool> VerifyEmail(string email,
    string verificationToken, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            // Step 1: Check if there’s a valid VerificationToken for the provided email and token value
            UserIdRoleDto? dto = await connection.QueryFirstOrDefaultAsync<UserIdRoleDto?>(
                @"SELECT MU.Id As Id, MU.Role As Role
                  FROM MembershipUser MU
                  JOIN VerificationToken VT ON VT.MembershipUser = MU.Id
                  WHERE MU.Email = @Email AND VT.Value = @Token",
                new { Email = email, Token = verificationToken },
                transaction
            );

            // If no matching record is found, return false (activation failed)
            if (dto is null)
            {
                return false;
            }

            // Step 2: Update the MembershipUser record to set IsActive = true and EmailConfirmed = true
            if (dto.Role == (byte)UserRole.Customer)
            {

                await connection.ExecuteAsync(
                    @"UPDATE MembershipUser 
                  SET IsActive = 1, EmailConfirmed = 1 
                  WHERE Id = @UserId",
                    new { UserId = dto.Id },
                    transaction
                );
            }
            else
            {
                await connection.ExecuteAsync(
                    @"UPDATE MembershipUser 
                  SET EmailConfirmed = 1 
                  WHERE Id = @UserId",
                    new { UserId = dto.Id },
                    transaction
                );
            }
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
