using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Controllers;
using KiloMart.DataAccess.Database;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/system-settings")]
public class SystemSettingsAdminController : AppController
{
    public SystemSettingsAdminController(IDbFactory dbFactory, IUserContext userContext) 
        : base(dbFactory, userContext)
    {
    }

    [HttpGet("")]
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

    [HttpPut("")]
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
            request.DeliveryOrderFee??settings.DeliveryOrderFee,
            request.ProviderOrderFee??settings.ProviderOrderFee,
            request.CancelOrderWhenNoProviderHasAllProducts??settings.CancelOrderWhenNoProviderHasAllProducts);
        
        if (!updated)
        {
            return DataNotFound($"System settings with ID {0} not found or not updated.");
        }

        return Success();
    }
}

public class SystemSettingsUpdateRequest
{
    public double? DeliveryOrderFee { get; set; }
    public double? ProviderOrderFee { get; set; }
    public bool? CancelOrderWhenNoProviderHasAllProducts { get; set; }
}