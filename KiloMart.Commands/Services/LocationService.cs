// LocationService.cs
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Commands.Services;

public class LocationInsertModel
{
    public string Name { get; set; } = null!;
    public float Longitude { get; set; }
    public float Latitude { get; set; }

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
    public int Id  { get; set; }
    public string? Name { get; set; }
    public float? Longitude { get; set; }
    public float? Latitude { get; set; }
    public bool? IsActive { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();
        if(Id <= 0)
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

            if(existingModel.Party != userPayLoad.Party)
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
}
