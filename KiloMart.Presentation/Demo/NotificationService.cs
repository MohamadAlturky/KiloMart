using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace FFF;
public class NotificationService : BackgroundService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", 
            new {Message = $"Notification at {DateTime.Now}"});
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}