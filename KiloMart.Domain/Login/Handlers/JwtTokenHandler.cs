// using System.Text;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.Extensions.Configuration;
// using KiloMart.Domain.Login.Models;

// namespace KiloMart.Domain.Login.Handlers;
// public static class JwtTokenHandler
// {
//     public static string GenerateAccessToken(MembershipUserDto user, IConfiguration configuration)
//     {
//         var claims = new[]
//         {
//             new Claim(CustomClaimTypes.UserId, user.Id.ToString()),
//             new Claim(CustomClaimTypes.Email, user.Email),
//             new Claim(CustomClaimTypes.Role, user.Role.ToString()),
//             new Claim(CustomClaimTypes.Party, user.Party.ToString())
//         };
//         var secretKey = configuration["Jwt:Key"]!;
//         var issuer = configuration["Jwt:Issuer"];
//         var audience = configuration["Jwt:Audience"];
//         int expiryTime = int.Parse(configuration["Jwt:ExpiryTimeInMinutes"]!);

//         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
//         var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//         var token = new JwtSecurityToken(
//             issuer: issuer,
//             audience: audience,
//             claims: claims,
//             expires: DateTime.UtcNow.AddMinutes(expiryTime),
//             signingCredentials: credentials
//         );

//         return new JwtSecurityTokenHandler().WriteToken(token);
//     }


//     public static string GenerateJwtToken(MembershipUserDto user, IConfiguration configuration)
//     {
//         // Fetch the key, issuer, and audience from configuration
//         var secretKey = configuration["Jwt:Key"]!;
//         var issuer = configuration["Jwt:Issuer"];
//         var audience = configuration["Jwt:Audience"];
//         if (!int.TryParse(configuration["Jwt:ExpiryTimeInMinutes"], out int expiryTime))
//         {
//             throw new InvalidOperationException("Invalid expiry time in configuration.");
//         }

//         // Ensure none of the values are null
//         if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
//         {
//             throw new InvalidOperationException("JWT configuration is missing or invalid.");
//         }

//         // Key and signing credentials
//         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
//         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//         // Claims
//         var claims = new List<Claim>
//         {
//             new Claim(CustomClaimTypes.UserId, user.Id.ToString()),
//             new Claim(CustomClaimTypes.Email, user.Email),       
//             new Claim(CustomClaimTypes.Role, user.Role.ToString()),
//             new Claim(CustomClaimTypes.Party, user.Party.ToString())
//         };

//         var token = new JwtSecurityToken(
//             issuer: issuer,
//             audience: audience,
//             claims: claims,
//             expires: DateTime.UtcNow.AddMinutes(expiryTime),
//             signingCredentials: creds
//         );

//         return new JwtSecurityTokenHandler().WriteToken(token);
//     }
// }