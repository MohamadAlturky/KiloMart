using System.Data;
using System.Text.Json;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.DateServices;
using KiloMart.Presentation.RealTime;
using Microsoft.AspNetCore.SignalR;

namespace KiloMart.Domain.Notifications;

public static class NotificationsService
{
    public static async Task SendNotification(
        IDbConnection connection,
        string title,
        string message,
        int userId,
        IHubContext<NotificationHub> hubContext,
        IDbTransaction transaction)
    {
        await Db.InsertNotificationAsync(
            connection,
            title,
            message,
            SaudiDateTimeHelper.GetCurrentTime(),
            userId,
            "",
            transaction);
        foreach (var connectionId in NotificationHub._connections.GetConnections(userId))
        {
            await hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification",
             new { Message = JsonSerializer.Serialize(new 
             {
                title = title,
                message = message,
                date = SaudiDateTimeHelper.GetCurrentTime()
             }) });
        }
    }
}

