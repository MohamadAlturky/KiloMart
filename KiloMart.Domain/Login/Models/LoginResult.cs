namespace KiloMart.Domain.Login.Models;

public class LoginResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string[] Errors { get; set; } = [];
}
