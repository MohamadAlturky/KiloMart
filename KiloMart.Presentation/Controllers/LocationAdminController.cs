using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/admin/location")]
public class LocationAdminController(IDbFactory dbFactory, IUserContext userContext) : AppController(dbFactory, userContext)
{
    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var (Locations, TotalCount) = await Query.GetLocationsWithPartyInfoPaginated(connection, page, pageSize);
        if (Locations is null || Locations.Length == 0)
        {
            return NotFound();
        }
        return Ok(new
        {
            Data = Locations,
            TotalCount = TotalCount
        });
    }
    [HttpGet("list/filter-by-is-deleted")]
    public async Task<IActionResult> ListFiltered([FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool isActive = true)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Query.GetLocationsWithPartyInfoPaginatedFilteredWithIsActive(connection,
            page,
            pageSize,
            isActive);
        if (result.Locations is null || result.Locations.Length == 0)
        {
            return NotFound();
        }
        return Ok(new
        {
            Data = result.Locations,
            TotalCount = result.TotalCount
        });
    }
}

public class Location
{
    public int Id { get; set; }
    public float Longitude { get; set; }
    public float Latitude { get; set; }
    public string Name { get; set; } = null!;
    public int Party { get; set; }
    public bool IsActive { get; set; }
}
