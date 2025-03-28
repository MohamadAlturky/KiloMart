using Microsoft.AspNetCore.SignalR;

namespace KiloMart.Presentation.RealTime;

public class NotificationHub : Hub
{
    public readonly static ConnectionMapping<int> _connections =
        new ConnectionMapping<int>();

    // Broadcast message to specific user
    public async Task SendChatMessage(int userId, string message)
    {
        foreach (var connectionId in _connections.GetConnections(userId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveNotification",
             new { Message = $"{userId} send this {message}" });
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = HubUserProvider.GetUserId(Context);

        _connections.Add(userId, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = HubUserProvider.GetUserId(Context);

        _connections.Remove(userId, Context.ConnectionId);
        await Clients.All.SendAsync("ReceiveNotification", new { Message = $"{userId} Disconnected" });
        await base.OnDisconnectedAsync(exception);
    }
}