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
    public class LocationController : AppController
    {
        public LocationController(IDbFactory dbFactory, IUserContext userContext)
            : base(dbFactory, userContext)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Insert(LocationInsertModel model)
        {
            var result = await LocationService.Insert(_dbFactory, _userContext.Get(), model);

            if (result.Success)
            {
                return CreatedAtAction(nameof(Insert), new { id = result.Data.Id }, result.Data);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, LocationUpdateModel model)
        {
            model.Id = id;

            var result = await LocationService.Update(_dbFactory, _userContext.Get(), model);

            if (result.Success)
            {
                return Ok(result.Data);
            }
            else
            {
                if (result.Errors.Contains("Not Found"))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await LocationService.DeActivate(_dbFactory, _userContext.Get(), id);

            if (result.Success)
            {
                return Ok(result.Data);
            }
            else
            {
                if (result.Errors.Contains("Not Found"))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
        }
        [HttpGet("mine")]
        [Guard([Roles.Customer])]
        public async Task<IActionResult> GetMine()
        {
            var party = _userContext.Get().Party;
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            var query = "SELECT * FROM [dbo].[Location] WHERE [Party] = @Party AND IsActive = 1;";
            var result = await connection.QueryAsync<Location>(query, new { Party = party });
            return Ok(result.ToList());
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
