using Microsoft.AspNetCore.Mvc;
using KiloMart.Core.Settings;
using Microsoft.AspNetCore.SignalR;
using KiloMart.Presentation.RealTime;
using KiloMart.Core.Contracts;
using KiloMart.Core.Authentication;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/admin/settings")]
public class AppSettingsController : AppController
{
    private readonly IAppSettingsProvider _settingsProvider;
    private readonly IHubContext<NotificationHub> _hubContext;

    public AppSettingsController(
        IDbFactory dbFactory,
        IAppSettingsProvider settingsProvider,
        IHubContext<NotificationHub> hubContext,
        IUserContext userContext) 
            : base(dbFactory, userContext)
    {
        _settingsProvider = settingsProvider;
        _hubContext = hubContext;
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetSetting(ConstantSettings key)
    {
        var setting = await _settingsProvider.GetSettingAsync((int)key);
        if (setting is null)
        {
            return DataNotFound($"Setting with key {key} not found.");
        }
        return Success(setting);
    }

    [HttpPut("{key}")]
    public async Task<IActionResult> UpdateSetting(ConstantSettings key, [FromBody] string value)
    {
        await _settingsProvider.UpdateSettingAsync((int)key, value);
        return Success();
    }

    [HttpPost("invalidate-cache")]
    public async Task<IActionResult> InvalidateCache()
    {
        await _settingsProvider.InvalidateCacheAsync();
        return Success();
    }

    [HttpGet("cancel-multi-provider-order")]
    public async Task<IActionResult> GetCancelWhenOrderFromMultiProviders()
    {
        var setting = await _settingsProvider.GetSettingAsync((int)ConstantSettings.CancelWhenOrderFromMultiProviders);
        if (setting == null)
        {
            return DataNotFound("Setting not found.");
        }
        return Success(bool.Parse(setting));
    }

    [HttpPut("cancel-multi-provider-order")]
    public async Task<IActionResult> SetCancelWhenOrderFromMultiProviders([FromBody] bool value)
    {
        await _settingsProvider.UpdateSettingAsync((int)ConstantSettings.CancelWhenOrderFromMultiProviders, value.ToString());
        return Success();
    }

    [HttpPost("notify")]
    public async Task<IActionResult> Notify([FromBody] string message, [FromQuery] int userId)
    {
        //await _hubContext.SendChatMessage(userId,message);
        foreach (var connectionId in NotificationHub._connections.GetConnections(userId))
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification",
             new { Message = $"this message is just for you user Id = {userId} the message :\n {message}" });
        }
        return Success();
    }
    [HttpPost("connected-users")]
    public IActionResult ConnectedUser()
    {
        return Success(NotificationHub._connections.ConnectionsDictionary);
    }
}