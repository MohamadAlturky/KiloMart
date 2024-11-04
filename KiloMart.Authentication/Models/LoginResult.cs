namespace KiloMart.Authentication.Models;

public record LoginResult
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public JwtToken? JwtToken { get; init; }
}
