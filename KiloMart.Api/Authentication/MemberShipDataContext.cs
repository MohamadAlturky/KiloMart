using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KiloMart.Api.Authentication;

public class MemberShipDataContext : IdentityDbContext<MemberShipUser, IdentityRole<int>, int>
{
    public MemberShipDataContext(DbContextOptions<MemberShipDataContext> options) : base(options)
    {
    }
}