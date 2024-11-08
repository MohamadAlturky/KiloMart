using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace KiloMart.Api.RealTime
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly static ConnectionMapping<int> _connections =
            new ConnectionMapping<int>();

        // Broadcast message to specific user
        public async Task SendChatMessage(int userId, string message)
        {
            string name = Context.User?.Identity?.Name ?? "Unknown";

            foreach (var connectionId in _connections.GetConnections(userId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", name, message);
            }
        }

        public override async Task OnConnectedAsync()
        {
            if (int.TryParse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                _connections.Add(userId, Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (int.TryParse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                out var userId))
            {
                _connections.Remove(userId, Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        //public override async Task OnReconnectedAsync()
        //{
        //    if (int.TryParse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)
        //        && !_connections.GetConnections(userId).Contains(Context.ConnectionId))
        //    {
        //        _connections.Add(userId, Context.ConnectionId);
        //    }

        //    await base.OnReconnectedAsync();
        //}
    }
}