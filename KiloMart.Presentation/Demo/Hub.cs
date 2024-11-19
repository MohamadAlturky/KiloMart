using Microsoft.AspNetCore.SignalR;
using KiloMart.Domain.Login.Models;
using KiloMart.Core.Authentication;
using Microsoft.AspNetCore.Authorization;

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
        foreach (string? o in Context.User.Claims.Select(e => e.Value).ToList())
        {
            _logger.LogInformation(o);

        }
        if (int.TryParse(Context.User?.FindFirst(CustomClaimTypes.UserId)?.Value, out var userId))
        {
            // Notify all clients of a disconnection (replace this with specific business logic if needed)
            await Clients.All.SendAsync("ReceiveNotification", new { userId });
        }
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
    public override async Task OnConnectedAsync()
    {
        var contexttttt = Context;
        var r = Context.GetHttpContext()?.Request.Query["access_token"];
        await Clients.All.SendAsync("ReceiveNotification", new { access_token = r });

        foreach (string? o in Context.User.Claims.Select(e => e.Value).ToList())
        {
            _logger.LogInformation(o);

        }
        if (int.TryParse(Context.User?.FindFirst(CustomClaimTypes.UserId)?.Value, out var userId))
        {
            await Clients.All.SendAsync("ReceiveNotification", new { userId = userId });
        }

        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (int.TryParse(Context.User?.FindFirst(CustomClaimTypes.UserId)?.Value, out var userId))
        {
            // Notify all clients of a disconnection (replace this with specific business logic if needed)
            await Clients.All.SendAsync("UserDisconnected", new { userId });
        }

        await base.OnDisconnectedAsync(exception);
    }
}