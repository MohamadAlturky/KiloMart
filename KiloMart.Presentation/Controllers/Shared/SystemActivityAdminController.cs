using Microsoft.AspNetCore.Mvc;
using KiloMart.DataAccess.Database;
using KiloMart.Core.Contracts;
using KiloMart.Core.Authentication;

namespace KiloMart.Presentation.Controllers.Shared;
[ApiController]
[Route("api/admin/system-activity")]
public class SystemActivityAdminController : AppController
{
    public SystemActivityAdminController(IDbFactory dbFactory, IUserContext userContext)
        : base(dbFactory, userContext) { }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var activity = await Db.GetSystemActivityByIdAsync(id, connection);
        if (activity is null)
        {
            return NotFound();
        }

        return Ok(activity);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var activities = await Db.GetAllSystemActivitiesAsync(connection);


        return Ok(new
        {
            Data = activities,
            TotalCount = activities.Count()
        });
    }

    [HttpGet("sum")]
    public async Task<IActionResult> GetSum([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        float totalValue = await Db.GetSumOfActivityValuesBetweenDatesAsync(startDate, endDate, connection);

        return Ok(new
        {
            TotalValue = totalValue
        });
    }
}

public class SystemActivityDto
{
    public DateTime Date { get; set; }
    public float Value { get; set; }
    public long Order { get; set; }
}