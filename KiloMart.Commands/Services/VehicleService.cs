using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Commands.Services;

public static class VehicleService
{
    public static async Task<Result<Vehicle>> Insert(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        VehicleInsertModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<Vehicle>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var id = await Db.InsertVehicleAsync(connection, model.Number, model.Model, model.Type, model.Year, model.Delivary);
            var vehicle = new Vehicle
            {
                Id = id,
                Number = model.Number,
                Model = model.Model,
                Type = model.Type,
                Year = model.Year,
                Delivary = model.Delivary
            };

            return Result<Vehicle>.Ok(vehicle);
        }
        catch (Exception e)
        {
            return Result<Vehicle>.Fail([e.Message]);
        }
    }

    public static async Task<Result<Vehicle>> Update(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        VehicleUpdateModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<Vehicle>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var existingModel = await Db.GetVehicleByIdAsync(model.Id, connection);

            if (existingModel is null)
            {
                return Result<Vehicle>.Fail(["Not Found"]);
            }
            existingModel.Number = model.Number ?? existingModel.Number;
            existingModel.Model = model.Model ?? existingModel.Model;
            existingModel.Type = model.Type ?? existingModel.Type;
            existingModel.Year = model.Year ?? existingModel.Year;
            existingModel.Delivary = model.Delivary ?? existingModel.Delivary;

            await Db.UpdateVehicleAsync(connection,
                existingModel.Id,
                existingModel.Number,
                existingModel.Model,
                existingModel.Type,
                existingModel.Year,
                existingModel.Delivary);

            return Result<Vehicle>.Ok(existingModel);

        }
        catch (Exception e)
        {
            return Result<Vehicle>.Fail([e.Message]);
        }
    }
}

public class VehicleInsertModel
{
    public string Number { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Year { get; set; } = null!;
    public int Delivary { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Number))
            errors.Add("Vehicle number is required.");

        if (string.IsNullOrWhiteSpace(Model))
            errors.Add("Vehicle model is required.");

        if (string.IsNullOrWhiteSpace(Type))
            errors.Add("Vehicle type is required.");

        if (string.IsNullOrWhiteSpace(Year))
            errors.Add("Vehicle year is required.");

        if (Delivary <= 0)
            errors.Add("Delivary must be a positive number.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public class VehicleUpdateModel
{
    public int Id { get; set; }
    public string? Number { get; set; }
    public string? Model { get; set; }
    public string? Type { get; set; }
    public string? Year { get; set; }
    public int? Delivary { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Id <= 0)
            errors.Add("Vehicle ID must be a positive number.");

        return (errors.Count == 0, errors.ToArray());
    }
}
