using Microsoft.AspNetCore.Mvc;
using KiloMart.Core.Settings;
using Microsoft.AspNetCore.SignalR;
using KiloMart.Presentation.RealTime;
using KiloMart.Core.Contracts;
using KiloMart.Core.Authentication;
using KiloMart.DataAccess.Database;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/admin")]
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

    // [HttpGet("{key}")]
    // public async Task<IActionResult> GetSetting(ConstantSettings key)
    // {
    //     var setting = await _settingsProvider.GetSettingAsync((int)key);
    //     if (setting is null)
    //     {
    //         return DataNotFound($"Setting with key {key} not found.");
    //     }
    //     return Success(setting);
    // }

    // [HttpPut("{key}")]
    // public async Task<IActionResult> UpdateSetting(ConstantSettings key, [FromBody] string value)
    // {
    //     await _settingsProvider.UpdateSettingAsync((int)key, value);
    //     return Success();
    // }

    // [HttpPost("invalidate-cache")]
    // public async Task<IActionResult> InvalidateCache()
    // {
    //     await _settingsProvider.InvalidateCacheAsync();
    //     return Success();
    // }

    // [HttpGet("cancel-multi-provider-order")]
    // public async Task<IActionResult> GetCancelWhenOrderFromMultiProviders()
    // {
    //     var setting = await _settingsProvider.GetSettingAsync((int)ConstantSettings.CancelWhenOrderFromMultiProviders);
    //     if (setting == null)
    //     {
    //         return DataNotFound("Setting not found.");
    //     }
    //     return Success(bool.Parse(setting));
    // }

    // [HttpPut("cancel-multi-provider-order")]
    // public async Task<IActionResult> SetCancelWhenOrderFromMultiProviders([FromBody] bool value)
    // {
    //     await _settingsProvider.UpdateSettingAsync((int)ConstantSettings.CancelWhenOrderFromMultiProviders, value.ToString());
    //     return Success();
    // }

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
    #region settings
    [HttpGet("system-settings")]
    public async Task<IActionResult> GetSystemSettings()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var settings = await Db.GetSystemSettingsByIdAsync(0, connection);
        if (settings is null)
        {
            return DataNotFound($"System settings with ID {0} not found.");
        }

        return Success(settings);
    }

    [HttpPut("system-settings")]
    public async Task<IActionResult> UpdateSystemSettings([FromBody] SystemSettingsUpdateRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request.");
        }

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var settings = await Db.GetSystemSettingsByIdAsync(0, connection);
        if (settings is null)
        {
            return DataNotFound($"System settings with ID {0} not found.");
        }
        var updated = await Db.UpdateSystemSettingsAsync(
            connection,
            0,
            request.DeliveryOrderFee ?? settings.DeliveryOrderFee,
            request.ProviderOrderFee ?? settings.ProviderOrderFee,
            request.CancelOrderWhenNoProviderHasAllProducts ?? settings.CancelOrderWhenNoProviderHasAllProducts,
            request.FirstTimeInMinutesToMakeTheCircleBigger ?? settings.FirstTimeInMinutesToMakeTheCircleBigger,
            request.FirstCircleRaduis ?? settings.FirstCircleRaduis,
            request.SecondTimeInMinutesToMakeTheCircleBigger ?? settings.SecondTimeInMinutesToMakeTheCircleBigger,
            request.MaxMinutesToCancelOrderWaitingADelivery ?? settings.MaxMinutesToCancelOrderWaitingADelivery,
            request.MaxMinutesToCancelOrderWaitingAProvider ?? settings.MaxMinutesToCancelOrderWaitingAProvider,
            request.SecondCircleRaduis ?? settings.SecondCircleRaduis,
            request.MinOrderValue??settings.MinOrderValue);

        if (!updated)
        {
            return DataNotFound($"System settings with ID {0} not found or not updated.");
        }

        return Success();
    }
    #endregion
}

public class SystemSettingsUpdateRequest
{
    public double? DeliveryOrderFee { get; set; }
    public double? ProviderOrderFee { get; set; }
    public bool? CancelOrderWhenNoProviderHasAllProducts { get; set; }
    public decimal? SecondCircleRaduis { get; set; }
    public int? SecondTimeInMinutesToMakeTheCircleBigger { get; set; }
    public decimal? FirstCircleRaduis { get; set; }
    public decimal? MinOrderValue { get; set; }
    public int? FirstTimeInMinutesToMakeTheCircleBigger { get; set; }
    public int? MaxMinutesToCancelOrderWaitingAProvider { get; set; }
    public int? MaxMinutesToCancelOrderWaitingADelivery { get; set; }
}