// using Dapper;
// using KiloMart.Core.Contracts;
// using KiloMart.DataAccess.Database;
// using KiloMart.Domain.Login.Models;
// using KiloMart.Domain.Register.Utils;
// using Microsoft.Extensions.Configuration;

// namespace KiloMart.Domain.Login.Services;

// public class ResetPasswordService
// {
//     public static async Task<ChangePasswordResult> ChangePassword(string email,
//         int membershipUser,
//         string currentPassword,
//         string newPassword,
//          IDbFactory dbFactory)
//     {
//         using var connection = dbFactory.CreateDbConnection();
//         connection.Open();

//         // var storedCode = await Db.GetResetPasswordCodeByCodeAsync(code, membershipUser, connection);

//         // if (storedCode is null)
//         // {
//         //     return new ChangePasswordResult { Success = false, Errors = ["Invalid Code."] };
//         // }

//         // Retrieve the user by email
//         var user = await connection.QueryFirstOrDefaultAsync<MembershipUserDto>(
//             @"SELECT Id, PasswordHash
//               FROM MembershipUser
//               WHERE Email = @Email",
//             new { Email = email }
//         );

//         // Check if user exists
//         if (user is null)
//         {
//             return new ChangePasswordResult { Success = false, Errors = ["User not found."] };
//         }

//         // Verify current password
//         if (!VerifyPassword(currentPassword, user.PasswordHash))
//         {
//             return new ChangePasswordResult { Success = false, Errors = ["Current password is incorrect."] };
//         }

//         // Validate new password (you can add more complex validation as needed)
//         if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
//         {
//             return new ChangePasswordResult { Success = false, Errors = ["New password must be at least 6 characters long."] };
//         }

//         // Hash the new password
//         var newPasswordHash = HashHandler.GetHash(newPassword);

//         // Update the password in the database
//         var rowsAffected = await connection.ExecuteAsync(
//             @"UPDATE MembershipUser
//               SET PasswordHash = @NewPasswordHash
//               WHERE Email = @Email",
//             new { NewPasswordHash = newPasswordHash, Email = email }
//         );

//         if (rowsAffected == 0)
//         {
//             return new ChangePasswordResult { Success = false, Errors = ["Failed to update password."] };
//         }

//         return new ChangePasswordResult { Success = true };
//     }
//     public static async Task<ChangePasswordResult> ChangeForgettedPassword(string email,
//         string code,
//         int membershipUser,
//         string newPassword,
//          IDbFactory dbFactory)
//     {
//         using var connection = dbFactory.CreateDbConnection();
//         connection.Open();

//         var storedCode = await Db.GetResetPasswordCodeByCodeAsync(code, membershipUser, connection);

//         if (storedCode is null)
//         {
//             return new ChangePasswordResult { Success = false, Errors = ["Invalid Code."] };
//         }

//         // Retrieve the user by email
//         var user = await connection.QueryFirstOrDefaultAsync<MembershipUserDto>(
//             @"SELECT Id, PasswordHash
//               FROM MembershipUser
//               WHERE Email = @Email",
//             new { Email = email }
//         );

//         // Check if user exists
//         if (user is null)
//         {
//             return new ChangePasswordResult { Success = false, Errors = ["User not found."] };
//         }


//         // Validate new password (you can add more complex validation as needed)
//         if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
//         {
//             return new ChangePasswordResult { Success = false, Errors = ["New password must be at least 6 characters long."] };
//         }

//         // Hash the new password
//         var newPasswordHash = HashHandler.GetHash(newPassword);

//         // Update the password in the database
//         var rowsAffected = await connection.ExecuteAsync(
//             @"UPDATE MembershipUser
//               SET PasswordHash = @NewPasswordHash
//               WHERE Email = @Email",
//             new { NewPasswordHash = newPasswordHash, Email = email }
//         );

//         if (rowsAffected == 0)
//         {
//             return new ChangePasswordResult { Success = false, Errors = ["Failed to update password."] };
//         }

//         return new ChangePasswordResult { Success = true };
//     }

//     private static bool VerifyPassword(string inputPassword, string storedPasswordHash)
//     {
//         var hash = HashHandler.GetHash(inputPassword);
//         return hash == storedPasswordHash;
//     }
// }

// // Result model for change password operation
// public class ChangePasswordResult
// {
//     public bool Success { get; set; }
//     public List<string> Errors { get; set; } = new List<string>();
// }
