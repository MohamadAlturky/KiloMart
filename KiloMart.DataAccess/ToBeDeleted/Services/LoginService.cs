using System.Text;
using System.Data;
using System.Security.Claims;
using KiloMart.Authentication.Models;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace KiloMart.Authentication.Services;

public class LoginService
{
    private readonly IConfiguration _configuration;
    private readonly IDbConnection _dbConnection;

    public LoginService(IConfiguration configuration, IDbConnection dbConnection)
    {
        _configuration = configuration;
        _dbConnection = dbConnection;
    }


    public async Task<(string AccessToken, string RefreshToken)?> LoginAsync(string email, string password)
    {
        var user = await UserService.GetByEmailAsync(_dbConnection, email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.HashedPassword))
        {
            return null; // Invalid email or password
        }

        var accessToken = GenerateJwtToken(user, TokenType.Access);
        var refreshToken = GenerateJwtToken(user, TokenType.Refresh);

        return (accessToken, refreshToken);
    }

    private string GenerateJwtToken(User user, TokenType tokenType)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "User") // Add other claims if needed
            }),
            Expires = tokenType == TokenType.Access
                ? DateTime.UtcNow.AddMinutes(15)
                : DateTime.UtcNow.AddDays(7), // Refresh token expires later
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

        try
        {
            tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return true;
        }
        catch
        {
            return false; // Invalid token
        }
    }
}

public enum TokenType
{
    Access,
    Refresh
}
