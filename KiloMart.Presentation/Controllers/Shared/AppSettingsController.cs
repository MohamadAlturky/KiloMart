using Microsoft.AspNetCore.Mvc;
using KiloMart.Core.Settings;
using Microsoft.AspNetCore.SignalR;
using KiloMart.Presentation.RealTime;
using KiloMart.Core.Contracts;
using KiloMart.Core.Authentication;
using KiloMart.DataAccess.Database;
using KiloMart.Presentation.Authorization;
using KiloMart.Domain.Register.Utils;
using KiloMart.Domain.DateServices;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/admin")]
public class AppSettingsController : AppController
{
    // private readonly IAppSettingsProvider _settingsProvider;
    private readonly IHubContext<NotificationHub> _hubContext;

    public AppSettingsController(
        IDbFactory dbFactory,
        // IAppSettingsProvider settingsProvider,
        IHubContext<NotificationHub> hubContext,
        IUserContext userContext)
            : base(dbFactory, userContext)
    {
        // _settingsProvider = settingsProvider;
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
    public class NotificationDto
    {
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
    [HttpPost("notify/admin")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> Notify([FromBody] NotificationDto dto, [FromQuery] int userId)
    {
        //await _hubContext.SendChatMessage(userId,message);
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        await Db.InsertNotificationAsync(
            connection,
            dto.Title,
            dto.Message,
            SaudiDateTimeHelper.GetCurrentTime(),
            userId,
            "");
        foreach (var connectionId in NotificationHub._connections.GetConnections(userId))
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification",
             new { Message = $"this message is just for you user Id = {userId}, the title = {dto.Title}, the message :\n {dto.Message}" });
        }
        return Success();
    }
    [HttpPost("notify/all-users")]
    [Guard([Roles.Customer, Roles.Delivery, Roles.Provider])]

    public async Task<IActionResult> NotifyForAll([FromBody] NotificationDto dto, [FromQuery] int userId)
    {
        //await _hubContext.SendChatMessage(userId,message);
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        await Db.InsertNotificationAsync(
            connection,
            dto.Title,
            dto.Message,
            SaudiDateTimeHelper.GetCurrentTime(),
            userId,
            "");
        foreach (var connectionId in NotificationHub._connections.GetConnections(userId))
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification",
             new { Message = $"this message is just for you user Id = {userId}, the title = {dto.Title}, the message :\n {dto.Message}" });
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
    [Guard([Roles.Admin])]
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
            request.SystemOrderFee ?? settings.SystemOrderFee,
            request.CancelOrderWhenNoProviderHasAllProducts ?? settings.CancelOrderWhenNoProviderHasAllProducts,
            request.TimeInMinutesToMakeTheCircleBigger ?? settings.TimeInMinutesToMakeTheCircleBigger,
            request.CircleRaduis ?? settings.CircleRaduis,
            request.MaxMinutesToCancelOrderWaitingAProvider ?? settings.MaxMinutesToCancelOrderWaitingAProvider,
            request.MinOrderValue ?? settings.MinOrderValue,
            request.DistanceToAdd ?? settings.DistanceToAdd,
            request.MaxDistanceToAdd ?? settings.MaxDistanceToAdd,
            request.RaduisForGetProducts ?? settings.RaduisForGetProducts);

        if (!updated)
        {
            return DataNotFound($"System settings with ID {0} not found or not updated.");
        }

        return Success();
    }
    #endregion


    #region mobile-app-configuration
    [HttpGet("mobile-app-configuration")]
    public async Task<IActionResult> GetMobileAppConfiguration()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var config = await Db.GetMobileAppConfigurationByIdAsync(0, connection);
        if (config is null)
        {
            return DataNotFound($"Mobile app configuration with ID {0} not found.");
        }

        return Success(config);
    }

    [HttpPut("mobile-app-configuration")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> UpdateMobileAppConfiguration([FromBody] MobileAppConfigurationUpdateRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request.");
        }

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var existingConfig = await Db.GetMobileAppConfigurationByIdAsync(0, connection);
        if (existingConfig is null)
        {
            return DataNotFound($"Mobile app configuration with ID {0} not found.");
        }

        var updated = await Db.UpdateMobileAppConfigurationAsync(
            connection,
            0,
            request.CustomerAppMinimumBuildNumberAndroid ?? existingConfig.CustomerAppMinimumBuildNumberAndroid,
            request.CustomerAppMinimumBuildNumberIos ?? existingConfig.CustomerAppMinimumBuildNumberIos,
            request.CustomerAppUrlAndroid ?? existingConfig.CustomerAppUrlAndroid,
            request.CustomerAppUrlIos ?? existingConfig.CustomerAppUrlIos,
            request.ProviderAppMinimumBuildNumberAndroid ?? existingConfig.ProviderAppMinimumBuildNumberAndroid,
            request.ProviderAppMinimumBuildNumberIos ?? existingConfig.ProviderAppMinimumBuildNumberIos,
            request.ProviderAppUrlAndroid ?? existingConfig.ProviderAppUrlAndroid,
            request.ProviderAppUrlIos ?? existingConfig.ProviderAppUrlIos,
            request.DeliveryAppMinimumBuildNumberAndroid ?? existingConfig.DeliveryAppMinimumBuildNumberAndroid,
            request.DeliveryAppMinimumBuildNumberIos ?? existingConfig.DeliveryAppMinimumBuildNumberIos,
            request.DeliveryAppUrlAndroid ?? existingConfig.DeliveryAppUrlAndroid,
            request.DeliveryAppUrlIos ?? existingConfig.DeliveryAppUrlIos
        );

        if (!updated)
        {
            return DataNotFound($"Mobile app configuration with ID {0} not found or not updated.");
        }

        return Success();
    }
    #endregion


}

public class SystemSettingsUpdateRequest
{
    public decimal? DeliveryOrderFee { get; set; }
    public decimal? SystemOrderFee { get; set; }
    public bool? CancelOrderWhenNoProviderHasAllProducts { get; set; }
    public int? TimeInMinutesToMakeTheCircleBigger { get; set; }
    public decimal? CircleRaduis { get; set; }
    public int? MaxMinutesToCancelOrderWaitingAProvider { get; set; }
    public decimal? MinOrderValue { get; set; }
    public decimal? DistanceToAdd { get; set; }
    public decimal? MaxDistanceToAdd { get; set; }
    public decimal? RaduisForGetProducts { get; set; }
}

public class MobileAppConfigurationUpdateRequest
{
    public float? CustomerAppMinimumBuildNumberAndroid { get; set; }
    public float? CustomerAppMinimumBuildNumberIos { get; set; }
    public string? CustomerAppUrlAndroid { get; set; }
    public string? CustomerAppUrlIos { get; set; }
    public float? ProviderAppMinimumBuildNumberAndroid { get; set; }
    public float? ProviderAppMinimumBuildNumberIos { get; set; }
    public string? ProviderAppUrlAndroid { get; set; }
    public string? ProviderAppUrlIos { get; set; }
    public float? DeliveryAppMinimumBuildNumberAndroid { get; set; }
    public float? DeliveryAppMinimumBuildNumberIos { get; set; }
    public string? DeliveryAppUrlAndroid { get; set; }
    public string? DeliveryAppUrlIos { get; set; }
}