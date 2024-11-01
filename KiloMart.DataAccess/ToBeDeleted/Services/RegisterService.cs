using KiloMart.Authentication.Models;
using KiloMart.Authentication.Services;
using System;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KiloMart.Authentication.Services;
public static class RegisterService
{
    public static async Task RegisterAsync(IDbConnection dbConnection, User user, int roleId, string gmailUsername, string gmailPassword)
    {
        // Step 1: Hash the user's password
        user.HashedPassword = HashPassword(user.HashedPassword);

        // Step 2: Add user to the database
        int userId = await UserService.AddAsync(dbConnection, user);

        // Step 3: Assign a default role to the user
        var userRole = new UserRole { User = userId, Role = roleId };
        await UserRoleService.AddAsync(dbConnection, userRole);

        // Step 4: Generate an OTP
        var otpCode = GenerateOtp();

        // Step 5: Send the OTP to the user's email
        await SendOtpEmailAsync(user.Email, otpCode, gmailUsername, gmailPassword);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static string GenerateOtp()
    {
        var random = new Random();
        return random.Next(1000, 9999).ToString();
    }

    private static async Task SendOtpEmailAsync(string email, string otpCode, string gmailUsername, string gmailPassword)
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(gmailUsername, gmailPassword),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(gmailUsername),
            Subject = "Your OTP Code",
            Body = $"Your OTP code is: {otpCode}",
            IsBodyHtml = false,
        };
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }
}

//var registerService = new RegisterService(userService, userRoleService);
//await registerService.RegisterAsync(dbConnection, new User { Name = "John", Email = "john@example.com", HashedPassword = "UserPassword123" }, roleId, "yourgmail@gmail.com", "yourgmailpassword");
