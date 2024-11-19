using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers
{
    [ApiController]
    [Route("api/location")]
    public class LocationAdminController : AppController
    {
        public LocationAdminController(IDbFactory dbFactory, IUserContext userContext)
            : base(dbFactory, userContext)
        {
        }


        [HttpGet("list")]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            var result = await Query.GetLocationsWithPartyInfoPaginated(connection, page, pageSize);
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
