namespace KiloMart.Domain.Login.Models;

public class LoginResult
{
    public bool Success { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string UserName { get; set; }
    public string? Token { get; set; }
    public string[] Errors { get; set; } = [];
    public byte Language { get; set; }
    public int Party { get; set; }
    public int UserId { get; set; }
    public short RoleNumber { get; set; }
}
