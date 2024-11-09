using KiloMart.Core.Authentication;
using KiloMart.Domain.Login.Models;

namespace KiloMart.Presentation.Authorization;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserPayLoad Get()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        return new UserPayLoad
        {
            Id = int.Parse(user?.FindFirst(CustomClaimTypes.UserId)?.Value ?? "0"),
            Role = int.Parse(user?.FindFirst(CustomClaimTypes.Role)?.Value ?? "0"),
            Party = int.Parse(user?.FindFirst(CustomClaimTypes.Party)?.Value ?? "0"),
            Email = user?.FindFirst(CustomClaimTypes.Email)?.Value ?? string.Empty
        };
    }
}