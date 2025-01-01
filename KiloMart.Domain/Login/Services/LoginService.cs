// using Dapper;
// using KiloMart.Core.Contracts;
// using KiloMart.DataAccess.Database;
// using KiloMart.Domain.Login.Handlers;
// using KiloMart.Domain.Login.Models;
// using KiloMart.Domain.Register.Utils;
// using Microsoft.Extensions.Configuration;

// namespace KiloMart.Domain.Login.Services;

// public class LoginService
// {
//     public static async Task<LoginResult> Login(string email, string password, IDbFactory dbFactory, IConfiguration configuration)
//     {
//         using var connection = dbFactory.CreateDbConnection();
//         connection.Open();
//         var user = await connection.QueryFirstOrDefaultAsync<MembershipUserDto>(
//                 @"SELECT Id, EmailConfirmed, PasswordHash, IsActive, Role, Party, Email, Language
//                   FROM MembershipUser
//                   WHERE Email = @Email",
//                 new { Email = email }
//             );


//         if (user == null || !VerifyPassword(password, user.PasswordHash))
//         {
//             return new LoginResult { Success = false, Errors = ["Invalid email or password."] };
//         }
//         if (!user.IsActive)
//         {
//             return new LoginResult { Success = false, Errors = ["This User account needs to be activated from the admin."] };
//         }
//         if (!user.EmailConfirmed)
//         {
//             return new LoginResult { Success = false, Errors = ["Email is not confirmed."] };
//         }

//         // var token = JwtTokenHandler.GenerateAccessToken(user, configuration);
//         var token = JwtTokenHandler.GenerateJwtToken(user, configuration);
//         var party = await Db.GetPartyByIdAsync(user.Party, connection);
//         if (party is null)
//         {
//             return new LoginResult { Success = false, Errors = ["The User Party Is Not Found"] };
//         }
//         return new LoginResult
//         {
//             Success = true,
//             Token = token,
//             UserName = party.DisplayName,
//             Party = party.Id,
//             Email = user.Email,
//             UserId = user.Id,
//             Role = CheckRole(user.Role),
//             RoleNumber = user.Role, 
//             Language = user.Language
//         };
//     }

//     private static string CheckRole(short role)
//     {
//         if (role == (byte)Roles.Customer)
//         {
//             return "Customer";
//         }
//         if (role == (byte)Roles.Admin)
//         {
//             return "Admin";
//         }
//         if (role == (byte)Roles.Provider)
//         {
//             return "Provider";
//         }
//         if (role == (byte)Roles.Delivery)
//         {
//             return "Delivery";
//         }
//         throw new Exception("Not Supported Role");
//     }

//     private static bool VerifyPassword(string inputPassword, string storedPasswordHash)
//     {
//         var hash = HashHandler.GetHash(inputPassword);
//         return hash == storedPasswordHash;
//     }
// }

