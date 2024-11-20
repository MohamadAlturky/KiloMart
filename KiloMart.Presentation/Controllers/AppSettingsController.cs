using Microsoft.AspNetCore.Mvc;
using KiloMart.Core.Settings;
using Microsoft.AspNetCore.SignalR;
using KiloMart.Presentation.RealTime;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/admin/settings")]
public class AppSettingsController(IAppSettingsProvider settingsProvider,
IHubContext<NotificationHub> hubContext) : ControllerBase
{
    private readonly IAppSettingsProvider _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
    private readonly IHubContext<NotificationHub> _hubContext = hubContext;

    [HttpGet("{key}")]
    public async Task<ActionResult<string>> GetSetting(ConstantSettings key)
    {
        var setting = await _settingsProvider.GetSettingAsync((int)key);
        if (setting is null)
        {
            return NotFound($"Setting with key {key} not found.");
        }
        return Ok(setting);
    }

    [HttpPut("{key}")]
    public async Task<IActionResult> UpdateSetting(ConstantSettings key, [FromBody] string value)
    {
        await _settingsProvider.UpdateSettingAsync((int)key, value);
        return NoContent();
    }

    [HttpPost("invalidate-cache")]
    public async Task<IActionResult> InvalidateCache()
    {
        await _settingsProvider.InvalidateCacheAsync();
        return NoContent();
    }

    [HttpGet("cancel-multi-provider-order")]
    public async Task<ActionResult<bool>> GetCancelWhenOrderFromMultiProviders()
    {
        var setting = await _settingsProvider.GetSettingAsync((int)ConstantSettings.CancelWhenOrderFromMultiProviders);
        if (setting == null)
        {
            return NotFound("Setting not found.");
        }
        return Ok(bool.Parse(setting));
    }

    [HttpPut("cancel-multi-provider-order")]
    public async Task<IActionResult> SetCancelWhenOrderFromMultiProviders([FromBody] bool value)
    {
        await _settingsProvider.UpdateSettingAsync((int)ConstantSettings.CancelWhenOrderFromMultiProviders, value.ToString());
        return NoContent();
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
        return Ok();
    }
}