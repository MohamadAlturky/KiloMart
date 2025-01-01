namespace KiloMart.Core.Authentication;

public class UserPayLoad
{
    public int Id { get; set;}
    public int Role { get; set;}
    public int Party { get; set;}
    public string Email { get; set;} = null!;
    public string Code { get; set;} = null!;

}
