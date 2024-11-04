namespace KiloMart.Authentication.Models;

public class MembershipUser
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public int Role { get; set; }
    public int Party { get; set; }
    public bool IsActive { get; set; }
}

public class Party
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}
