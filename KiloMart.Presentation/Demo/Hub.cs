using Microsoft.AspNetCore.SignalR;
using KiloMart.Domain.Login.Models;
using KiloMart.Core.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using KiloMart.Presentation.Authorization;

namespace FFF;


// [Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)

    {
        _logger = logger;
    }

    public async Task SendNotification(string message)
    {
       await Clients.All.SendAsync("ReceiveNotification", message);
    }
    public override async Task OnConnectedAsync()
    {
        Microsoft.Extensions.Primitives.StringValues? token = 
        (Context.GetHttpContext()?.Request.Query["access_token"]) ?? 
            throw new UnauthorizedAccessException();
        bool decodingResult = JwtTokenValidator.ValidateToken(token!,
            GuardAttribute.SECRET_KEY,
            GuardAttribute.ISSUER,
            GuardAttribute.AUDIENCE,
            out JwtSecurityToken? decodedToken);

        if (!decodingResult || decodedToken is null)
        {
            throw new UnauthorizedAccessException();
        }

        var userIdClaim = decodedToken?.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserId);

        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            throw new UnauthorizedAccessException();
        }

        await Clients.All.SendAsync("ReceiveNotification", new { userId = userId });

        await base.OnConnectedAsync();
    }
}