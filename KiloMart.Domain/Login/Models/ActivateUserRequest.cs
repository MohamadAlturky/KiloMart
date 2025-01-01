// namespace KiloMart.Domain.Login.Models;

// public class ActivateUserRequest
// {
//     public string Email { get; set; } = string.Empty;
//     public string VerificationToken { get; set; } = string.Empty;

//     public (bool Success, string[] Errors) Validate()
//     {
//         var errors = new List<string>();
//         if (string.IsNullOrEmpty(Email))
//             errors.Add("Email is required");
//         if (string.IsNullOrEmpty(VerificationToken))
//             errors.Add("Verification token is required");
//         return (errors.Count == 0, errors.ToArray());
//     }
// }

