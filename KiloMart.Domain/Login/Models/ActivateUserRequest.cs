namespace KiloMart.Domain.Login.Models;

public class ActivateUserRequest
{
    public string? Email { get; set; }
    public string? VerificationToken { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrEmpty(Email))
            errors.Add("Email is required");
        if (string.IsNullOrEmpty(VerificationToken))
            errors.Add("Verification token is required");
        return (errors.Count == 0, errors.ToArray());
    }
}

