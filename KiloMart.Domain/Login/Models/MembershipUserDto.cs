namespace KiloMart.Domain.Login.Models;

public class MembershipUserDto
{
    public int Id { get; set; }
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public short Role { get; set; }
    public bool IsActive { get; set; }
    public string Email { get; set; } = string.Empty;
    public int Party { get; set; }
}