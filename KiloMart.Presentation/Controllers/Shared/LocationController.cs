using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/customer/location")]
public class LocationController(IDbFactory dbFactory, IUserContext userContext)
    : AppController(dbFactory, userContext)
{

    [HttpPost("add")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> Insert(LocationInsertModel model)
    {
        var result = await LocationService.Insert(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            return Fail(result.Errors);
        }
    }

    [HttpPut("{id}")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> Update(int id, LocationUpdateModel model)
    {
        model.Id = id;

        var result = await LocationService.Update(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return DataNotFound();
            }
            else
            {
                return Fail(result.Errors);
            }
        }
    }

    [HttpPut("edit-with-full-details/{id}")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> UpdateWithFullDetails(int id, LocationUpdateWithFullDetailsModel model)
    {
        model.Id = id;

        var result = await LocationService.UpdateWithFullDetails(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return DataNotFound();
            }
            else
            {
                return Fail(result.Errors);
            }
        }
    }

    [HttpDelete("{id}")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await LocationService.DeActivate(_dbFactory, _userContext.Get(), id);

        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return DataNotFound();
            }
            else
            {
                return Fail(result.Errors);
            }
        }
    }
    [HttpGet("customer-and-provider/mine")]
    [Guard([Roles.Customer, Roles.Provider])]
    public async Task<IActionResult> GetMine()
    {
        var party = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var query = @"
        SELECT 
            l.Id LocationId,
            l.IsActive LocationIsActive,
            l.Latitude LocationLatitude,
            l.Longitude LocationLongitude,
            l.Name LocationName,
            l.Party LocationParty,
            ld.ApartmentNumber LocationDetailsApartmentNumber,
            ld.BuildingNumber LocationDetailsBuildingNumber,
            ld.BuildingType LocationDetailsBuildingType,
            ld.FloorNumber LocationDetailsFloorNumber,
            ld.Id LocationDetailsId,
            ld.Location LocationDetailsLocation,
            ld.PhoneNumber LocationDetailsPhoneNumber,
            ld.StreetNumber LocationDetailsStreetNumber
        FROM [dbo].[Location] l 
        LEFT JOIN [dbo].[LocationDetails] ld
        ON ld.[Location] = l.Id 
        WHERE l.[Party] = @Party 
        AND l.IsActive = 1;";
        var result = await connection.QueryAsync<LocationVw>(query, new { Party = party });

        return Success(result.ToList());
    }
    
    [HttpPost("create-with-full-details")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> Create(LocationInsertWithDetailsModel model)
    {
        var result = await LocationService.Insert(_dbFactory, _userContext.Get(), new LocationInsertModel()
        {
            Latitude = model.Latitude,
            Longitude = model.Longitude,
            Name = model.Name
        });

        if (result.Success)
        {
            var detailsResult = await LocationDetailsService.Insert(
                _dbFactory,
                _userContext.Get(),
                new LocationDetailsInsertModel()
                {
                    ApartmentNumber = model.ApartmentNumber,
                    BuildingNumber = model.BuildingNumber,
                    BuildingType = model.BuildingType,
                    FloorNumber = model.FloorNumber,
                    Location = result.Data.Id,
                    PhoneNumber = model.PhoneNumber,
                    StreetNumber = model.StreetNumber
                }
            );

            if (detailsResult.Success)
            {
                return Success(new { Location = result.Data, Details = detailsResult.Data });
            }
            else
            {
                return Fail(result.Errors);
            }

        }
        else
        {
            return Fail(result.Errors);
        }
    }
}


public class LocationInsertWithDetailsModel
{
    public string Name { get; set; } = null!;
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }

    public string BuildingType { get; set; } = null!;
    public string BuildingNumber { get; set; } = null!;
    public string FloorNumber { get; set; } = null!;
    public string ApartmentNumber { get; set; } = null!;
    public string StreetNumber { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Location name is required.");

        if (Longitude < -180 || Longitude > 180)
            errors.Add("Longitude must be between -180 and 180.");

        if (Latitude < -90 || Latitude > 90)
            errors.Add("Latitude must be between -90 and 90.");
        if (string.IsNullOrWhiteSpace(BuildingType))
            errors.Add("Building type is required.");

        if (string.IsNullOrWhiteSpace(BuildingNumber))
            errors.Add("Building number is required.");

        if (string.IsNullOrWhiteSpace(FloorNumber))
            errors.Add("Floor number is required.");

        if (string.IsNullOrWhiteSpace(ApartmentNumber))
            errors.Add("Apartment number is required.");

        if (string.IsNullOrWhiteSpace(StreetNumber))
            errors.Add("Street number is required.");

        if (string.IsNullOrWhiteSpace(PhoneNumber))
            errors.Add("Phone number is required.");

        return (errors.Count == 0, errors.ToArray());
    }
}