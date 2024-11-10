using System.Text.RegularExpressions;
using KiloMart.Domain.Languages.Models;

namespace KiloMart.Domain.Register.Provider.Models;

public class RegisterProviderDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    public (bool Success,List<string> Errors) Validate()
    {
        List<string> errors = [];
        if(!IsValidEmail(Email))
        {
            errors.Add("Invalid email.");
        }
        if(string.IsNullOrEmpty(Email))
        {
            errors.Add("Email is required.");
        }
        if(string.IsNullOrEmpty(Password))
        {
            errors.Add("Password is required.");
        }
        if(string.IsNullOrEmpty(DisplayName))
        {
            errors.Add("Display name is required.");
        }
        return (errors.Count == 0, errors);
    }

    public static bool IsValidEmail(string email)
    {
        // use regex to validate email
        var regex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
        return regex.IsMatch(email);
    }
}
