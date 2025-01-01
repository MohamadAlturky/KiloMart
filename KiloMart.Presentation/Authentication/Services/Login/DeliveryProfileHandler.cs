// using System.Data;
// using KiloMart.Core.Contracts;
// using KiloMart.DataAccess.Database;
// using KiloMart.Domain.Login.Handlers;
// using KiloMart.Domain.Login.Models;
// using KiloMart.Domain.Register.Utils;

// namespace KiloMart.Domain.Login.Services;

// public class LoginService
// {
//     public static async Task<LoginResult> Login(string email, string password, IDbFactory dbFactory, IConfiguration configuration)
//     {
//         using var connection = dbFactory.CreateDbConnection();
//         connection.Open();

//         MembershipUser? user = await Db.GetMembershipUserByEmailAsync(connection, email);

//         if (user is null || !VerifyPassword(password, user.PasswordHash))
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
//         if (user.IsDeleted is not null)
//         {
//             if ((bool)user.IsDeleted)
//             {
//                 return new LoginResult { Success = false, Errors = ["This User account Is Deleted."] };
//             }
//         }

//         var sessionCode = Guid.NewGuid().ToString();

//         var token = JwtTokenHandler.GenerateJwtToken(user, sessionCode, configuration);

//         var party = await Db.GetPartyByIdAsync(user.Party, connection);

//         if (party is null)
//         {
//             return new LoginResult { Success = false, Errors = ["The User Party Is Not Found"] };
//         }
//         var loginResult = new LoginResult
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

//         return loginResult;
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


//     #region Profile Handler
//     private async Task<LoginResult> _handleProfile(IDbConnection connection, LoginResult result)
//     {
//         if (result.RoleNumber == (short)Roles.Customer)
//         {
//             return await _handleCustomerProfile(result, connection);
//         }
//         if (result.RoleNumber == (short)Roles.Delivery)
//         {
//             return await _handleDeliveryProfile(result, connection);
//         }
//         if (result.RoleNumber == (short)Roles.Provider)
//         {
//             return await _handleProviderProfile(result, connection);
//         }
//         var user = await Db.GetMembershipUserByIdAsync(connection, result.UserId);
//         var partyInfo = await Db.GetPartyByIdAsync(result.Party, connection);

//         result.PartyInfo = partyInfo;
//         result.UserInfo = new
//         {
//             user?.Id,
//             user?.Email,
//             user?.EmailConfirmed,
//             user?.IsActive,
//             user?.Role
//         };
//         return result;
//     }

//     private async Task<LoginResult> _handleProviderProfile(LoginResult result, IDbConnection connection)
//     {
//         var profile = await Db.GetActiveProviderProfileHistoryAsync(connection, result.Party);
//         var profiles = await Db.GetAllProviderProfileHistoryAsync(connection, result.Party);
//         result.ActiveProfile = profile;
//         result.AllProfiles = profiles;

//         if (profile is null)
//         {
//             result.Success = false;
//             result.Errors = ["This User Profile needs to be accepted from the admin, this are your profiles."];
//         }
//         var user = await Db.GetMembershipUserByIdAsync(connection, result.UserId);
//         var partyInfo = await Db.GetPartyByIdAsync(result.Party, connection);
//         result.PartyInfo = partyInfo;
//         result.UserInfo = new
//         {
//             user?.Id,
//             user?.Email,
//             user?.EmailConfirmed,
//             user?.IsActive,
//             user?.Role
//         };
//         return result;
//     }

//     private async Task<LoginResult> _handleDeliveryProfile(LoginResult result, IDbConnection connection)
//     {
//         var profile = await Db.GetDeliveryActiveProfileHistoryAsync(connection, result.Party);
//         var profiles = await Db.GetDeliveryAllProfileHistoryAsync(connection, result.Party);

//         result.ActiveProfile = profile;
//         result.AllProfiles = profiles;

//         if (profile is null)
//         {
//             result.Success = false;
//             result.Errors = ["This User Profile needs to be accepted from the admin, this are your profiles."];
//         }
//         var user = await Db.GetMembershipUserByIdAsync(connection, result.UserId);
//         var partyInfo = await Db.GetPartyByIdAsync(result.Party, connection);

//         result.PartyInfo = partyInfo;
//         result.UserInfo = new
//         {
//             user?.Id,
//             user?.Email,
//             user?.EmailConfirmed,
//             user?.IsActive,
//             user?.Role
//         };
//         return result;
//     }

//     private async Task<LoginResult> _handleCustomerProfile(LoginResult result, IDbConnection connection)
//     {
//         var profile = await Db.GetCustomerProfileByCustomerIdAsync(result.Party, connection);
//         var user = await Db.GetMembershipUserByIdAsync(connection, result.UserId);
//         var party = await Db.GetPartyByIdAsync(result.Party, connection);
//         result.ActiveProfile = profile;
//         result.PartyInfo = party;
//         result.UserInfo = new
//         {
//             user?.Id,
//             user?.Email,
//             user?.EmailConfirmed,
//             user?.IsActive,
//             user?.Role
//         };
//         return result;
//     }
//     #endregion
//     public class ProfileResult
//     {
//         public bool Success { get; set; }
//         public Object ActiveProfile { get; set; }
//         public Object AllProfile { get; set; }
//     }
// }


using System.Data;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Login.Models;

namespace KiloMart.Domain.Login.Services;

public class DeliveryProfileHandler : IProfileHandler
{
    public async Task<LoginResult> HandleProfileAsync(LoginResult result, IDbConnection connection)
    {
        var profile = await Db.GetDeliveryActiveProfileHistoryAsync(connection, result.Party);
        var profiles = await Db.GetDeliveryAllProfileHistoryAsync(connection, result.Party);

        result.ActiveProfile = profile;
        result.AllProfiles = profiles;

        if (profile is null)
        {
            result.Success = false;
            result.Errors = new[] { "This User Profile needs to be accepted from the admin, these are your profiles." };
        }

        var user = await Db.GetMembershipUserByIdAsync(connection, result.UserId);
        var party = await Db.GetPartyByIdAsync(result.Party, connection);

        result.PartyInfo = party;
        result.UserInfo = new
        {
            user?.Id,
            user?.Email,
            user?.EmailConfirmed,
            user?.IsActive,
            user?.Role
        };

        return result;
    }
}
