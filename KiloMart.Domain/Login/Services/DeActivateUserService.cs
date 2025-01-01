// using Dapper;
// using KiloMart.Core.Contracts;
// using KiloMart.DataAccess.Database;
// using KiloMart.Domain.Login.Handlers;
// using KiloMart.Domain.Login.Models;
// using KiloMart.Domain.Register.Utils;
// using Microsoft.Extensions.Configuration;

// namespace KiloMart.Domain.Login.Services;

// public class DeActivateUserService
// {
//     public static async Task<DeactivateUserResult> DeactivateUser(int userId, int partyId, IDbFactory dbFactory)
//     {
//         using var connection = dbFactory.CreateDbConnection();
//         connection.Open();
//         using var transaction = connection.BeginTransaction();

//         // Update the IsActive status of the user
//         var rowsAffected = await connection.ExecuteAsync(
//             @"UPDATE MembershipUser
//           SET IsActive = 0
//           WHERE Id = @UserId",
//             new { UserId = userId },
//             transaction);

//         if (rowsAffected == 0)
//         {
//             return new DeactivateUserResult { Success = false, Errors = ["User not found or already inactive."] };
//         }
//         rowsAffected = await connection.ExecuteAsync(
//             @"UPDATE Party
//           SET IsActive = 0
//           WHERE Id = @PartyId",
//             new { PartyId = partyId },
//             transaction);

//         if (rowsAffected == 0)
//         {
//             return new DeactivateUserResult { Success = false, Errors = ["Party not found or already inactive."] };
//         }
//         return new DeactivateUserResult { Success = true };
//     }



// }

// public class DeactivateUserResult
// {
//     public bool Success { get; set; }
//     public List<string> Errors { get; set; } = new List<string>();
// }