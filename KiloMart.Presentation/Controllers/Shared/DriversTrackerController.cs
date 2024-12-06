// DriversTrackerController.cs
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Controllers;
using KiloMart.Presentation.Tracking;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("api/tracking/drivers")]
public class DriversTrackerController : AppController
{
    private readonly DriversTrackerService _driversTrackerService;

    public DriversTrackerController(
        IDbFactory dbFactory,
        IUserContext userContext,
        DriversTrackerService driversTrackerService)
            : base(dbFactory,
             userContext)
    {
        _driversTrackerService = driversTrackerService;
    }

    [HttpPost("{userId}")]
    public IActionResult CreateOrUpdate(int userId, [FromBody] CreateDriverLocationRequest location)
    {
        _driversTrackerService.CreateOrUpdate(userId, location.Latitude, location.Longitude);
        return Success();
    }

    [HttpGet("{userId}")]
    public IActionResult GetByKey(int userId)
    {
        var location = _driversTrackerService.GetByKey(userId);
        if (location != null)
        {
            return Success(new { location });
        }
        return DataNotFound();
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var allLocations = _driversTrackerService.GetAll();
        return Success(new { allLocations });
    }

    [HttpGet("keys")]
    public IActionResult GetKeys()
    {
        var keys = _driversTrackerService.GetKeys();
        return Success(new { keys });
    }
}

public class CreateDriverLocationRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}