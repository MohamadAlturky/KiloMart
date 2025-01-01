namespace KiloMart.Domain.Login.Models;

public class LogInRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrEmpty(Email))
            errors.Add("Email is required");
        if (string.IsNullOrEmpty(Password))
            errors.Add("Password is required");
        return (errors.Count == 0, errors.ToArray());
    }
}

