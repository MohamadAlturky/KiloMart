using Microsoft.AspNetCore.Identity;

namespace KiloMart.Api.Authentication;
public class MemberShipUser : IdentityUser<int>
{
    public short Role { get; set; }
}
