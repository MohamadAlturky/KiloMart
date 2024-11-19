using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FFF;

public class NotificationHub : Hub
{
    public async Task SendNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveNotification", "UnKonw");

        if (int.TryParse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
        {
            await Clients.All.SendAsync("ReceiveNotification", userId);
        }

            await base.OnConnectedAsync();
        }
}