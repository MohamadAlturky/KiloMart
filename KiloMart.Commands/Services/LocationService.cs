// LocationService.cs
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Register.Utils;

namespace KiloMart.Commands.Services;

public class LocationInsertModel
{
    public string Name { get; set; } = null!;
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Location name is required.");

        if (Longitude < -180 || Longitude > 180)
            errors.Add("Longitude must be between -180 and 180.");

        if (Latitude < -90 || Latitude > 90)
            errors.Add("Latitude must be between -90 and 90.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public class LocationUpdateModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public bool? IsActive { get; set; }



    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();
        if (Id <= 0)
        {
            errors.Add("Id is required");
        }
        return (errors.Count == 0, errors.ToArray());
    }
}

public class LocationUpdateWithFullDetailsModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public bool? IsActive { get; set; }




    // details
    public string? LocationDetailsApartmentNumber { get; set; }
    public string? LocationDetailsBuildingNumber { get; set; }
    public string? LocationDetailsBuildingType { get; set; }
    public string? LocationDetailsFloorNumber { get; set; }
    public string? LocationDetailsPhoneNumber { get; set; }
    public string? LocationDetailsStreetNumber { get; set; }



    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();
        if (Id <= 0)
        {
            errors.Add("Id is required");
        }
        return (errors.Count == 0, errors.ToArray());
    }
}
public static class LocationService
{
    public static async Task<Result<Location>> Insert(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        LocationInsertModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<Location>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();

            if (userPayLoad.Role == ((int)Roles.Provider))
            {
                var existingLocation = Db.GetLocationByPartyAsync(userPayLoad.Party,
                connection);

                if (existingLocation is not null)
                {
                    return Result<Location>.Fail(["can't add multiple locations for the provider"]);
                }
            }

            var id = await Db.InsertLocationAsync(connection,
              model.Longitude,
              model.Latitude,
              model.Name,
              userPayLoad.Party);
            var location = new Location
            {
                Id = id,
                Name = model.Name,
                Longitude = model.Longitude,
                Latitude = model.Latitude,
                Party = userPayLoad.Party,
                IsActive = true
            };

            return Result<Location>.Ok(location);
        }
        catch (Exception e)
        {
            return Result<Location>.Fail([e.Message]);
        }
    }

    public static async Task<Result<Location>> Update(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        LocationUpdateModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<Location>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var existingModel = await Db.GetLocationByIdAsync(model.Id, connection);

            if (existingModel is null)
            {
                return Result<Location>.Fail(["Not Found"]);
            }

            if (existingModel.Party != userPayLoad.Party)
            {
                return Result<Location>.Fail(["Un Authorized"]);
            }

            existingModel.Name = model.Name ?? existingModel.Name;
            existingModel.Longitude = model.Longitude ?? existingModel.Longitude;
            existingModel.Latitude = model.Latitude ?? existingModel.Latitude;
            existingModel.IsActive = model.IsActive ?? existingModel.IsActive;

            await Db.UpdateLocationAsync(connection,
                existingModel.Id,
                existingModel.Longitude,
                existingModel.Latitude,
                existingModel.Name,
                existingModel.Party,
                existingModel.IsActive);

            return Result<Location>.Ok(existingModel);
        }
        catch (Exception e)
        {
            return Result<Location>.Fail([e.Message]);
        }
    }

    public static async Task<Result<LocationVw>> UpdateWithFullDetails(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        LocationUpdateWithFullDetailsModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<LocationVw>.Fail(errors);
        }
        var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {

            var existingModel = await Db.GetLocationByIdAsync(
                model.Id,
                connection,
                transaction);

            if (existingModel is null)
            {
                transaction.Rollback();
                return Result<LocationVw>.Fail(["Not Found"]);
            }

            if (existingModel.Party != userPayLoad.Party)
            {
                transaction.Rollback();
                return Result<LocationVw>.Fail(["Un Authorized"]);
            }

            existingModel.Name = model.Name ?? existingModel.Name;
            existingModel.Longitude = model.Longitude ?? existingModel.Longitude;
            existingModel.Latitude = model.Latitude ?? existingModel.Latitude;
            existingModel.IsActive = model.IsActive ?? existingModel.IsActive;

            await Db.UpdateLocationAsync(connection,
                existingModel.Id,
                existingModel.Longitude,
                existingModel.Latitude,
                existingModel.Name,
                existingModel.Party,
                existingModel.IsActive,
                transaction);

            var details = await Db.GetLocationDetailsByLocationIdAsync(
                existingModel.Id, 
                connection,
                transaction);

            if (details is null)
            {
                transaction.Rollback();
                return Result<LocationVw>.Fail(["Details Not Found"]);
            }
            details.StreetNumber = model.LocationDetailsStreetNumber ?? details.StreetNumber;
            details.ApartmentNumber = model.LocationDetailsApartmentNumber ?? details.ApartmentNumber;
            details.PhoneNumber = model.LocationDetailsPhoneNumber ?? details.PhoneNumber;
            details.FloorNumber = model.LocationDetailsFloorNumber ?? details.FloorNumber;
            details.BuildingNumber = model.LocationDetailsBuildingNumber ?? details.BuildingNumber;
            details.BuildingType = model.LocationDetailsBuildingType ?? details.BuildingType;

            await Db.UpdateLocationDetailsAsync(
               connection,
               details.Id,
               details.BuildingType,
               details.BuildingNumber,
               details.FloorNumber,
               details.ApartmentNumber,
               details.StreetNumber,
               details.PhoneNumber,
               details.Location,
               transaction);

            transaction.Commit();
            
            return Result<LocationVw>.Ok(new LocationVw
            {
                LocationId = existingModel.Id,
                LocationIsActive = existingModel.IsActive,
                LocationName = existingModel.Name,
                LocationParty = existingModel.Party,
                LocationLongitude = existingModel.Longitude,
                LocationLatitude = existingModel.Latitude,

                LocationDetailsApartmentNumber = details.ApartmentNumber,
                LocationDetailsBuildingNumber = details.BuildingNumber,
                LocationDetailsBuildingType = details.BuildingType,
                LocationDetailsFloorNumber = details.FloorNumber,
                LocationDetailsId = details.Id,
                LocationDetailsLocation = details.Location,
                LocationDetailsPhoneNumber = details.PhoneNumber,
                LocationDetailsStreetNumber = details.StreetNumber
            });
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return Result<LocationVw>.Fail([e.Message]);
        }
    }
    public static async Task<Result<Location>> DeActivate(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        int id)
    {
        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var existingModel = await Db.GetLocationByIdAsync(id, connection);

            if (existingModel is null)
            {
                return Result<Location>.Fail(["Not Found"]);
            }

            if (existingModel.Party != userPayLoad.Party)
            {
                return Result<Location>.Fail(["Un Authorized"]);
            }


            existingModel.IsActive = false;

            await Db.UpdateLocationAsync(connection,
                existingModel.Id,
                existingModel.Longitude,
                existingModel.Latitude,
                existingModel.Name,
                existingModel.Party,
                existingModel.IsActive);

            return Result<Location>.Ok(existingModel);
        }
        catch (Exception e)
        {
            return Result<Location>.Fail([e.Message]);
        }
    }
}
public class LocationVw
{
    public int LocationId { get; set; }
    public bool LocationIsActive { get; set; }
    public decimal LocationLatitude { get; set; }
    public decimal LocationLongitude { get; set; }
    public string LocationName { get; set; }
    public int LocationParty { get; set; }

    // details
    public string? LocationDetailsApartmentNumber { get; set; }
    public string? LocationDetailsBuildingNumber { get; set; }
    public string? LocationDetailsBuildingType { get; set; }
    public string? LocationDetailsFloorNumber { get; set; }
    public int? LocationDetailsId { get; set; }
    public int? LocationDetailsLocation { get; set; }
    public string? LocationDetailsPhoneNumber { get; set; }
    public string? LocationDetailsStreetNumber { get; set; }
}

