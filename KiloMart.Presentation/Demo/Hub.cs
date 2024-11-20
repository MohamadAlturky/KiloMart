// using Microsoft.AspNetCore.SignalR;
// using KiloMart.Presentation.RealTime;

// namespace FFF;


// // [Authorize]
// public class NotificationHub : Hub
// {
//     private readonly ILogger<NotificationHub> _logger;

//     public NotificationHub(ILogger<NotificationHub> logger)

//     {
//         _logger = logger;
//     }

//     public async Task SendNotification(string message)
//     {
//        await Clients.All.SendAsync("ReceiveNotification", message);
//     }
//     public override async Task OnConnectedAsync()
//     {
//         int userId = HubUserProvider.GetUserId(Context);

//         await Clients.All.SendAsync("ReceiveNotification", new { userId = userId });

//         await base.OnConnectedAsync();
//     }
// }