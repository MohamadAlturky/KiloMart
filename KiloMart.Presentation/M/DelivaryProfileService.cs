using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Commands.Services;

public class DelivaryProfileInsertModel
{
    public int Delivary { get; set; }
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string NationalName { get; set; } = null!;
    public string NationalId { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public DateOnly LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; } = null!;
    public DateOnly DrivingLicenseExpiredDate { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(FirstName))
            errors.Add("First name is required.");

        if (string.IsNullOrWhiteSpace(SecondName))
            errors.Add("Second name is required.");

        if (string.IsNullOrWhiteSpace(NationalName))
            errors.Add("National name is required.");

        if (string.IsNullOrWhiteSpace(NationalId))
            errors.Add("National ID is required.");

        if (string.IsNullOrWhiteSpace(LicenseNumber))
            errors.Add("License number is required.");

        if (LicenseExpiredDate < DateOnly.FromDateTime(DateTime.Now))
            errors.Add("License expire date must be in the future.");

        if (string.IsNullOrWhiteSpace(DrivingLicenseNumber))
            errors.Add("Driving license number is required.");

        if (DrivingLicenseExpiredDate < DateOnly.FromDateTime(DateTime.Now))
            errors.Add("Driving license expire date must be in the future.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public class DelivaryProfileUpdateModel
{
    public int Id { get; set; }
    public int? Delivary { get; set; }
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? NationalName { get; set; }
    public string? NationalId { get; set; }
    public string? LicenseNumber { get; set; }
    public DateOnly? LicenseExpiredDate { get; set; }
    public string? DrivingLicenseNumber { get; set; }
    public DateOnly? DrivingLicenseExpiredDate { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Id <= 0)
            errors.Add("Delivary profile ID must be a positive number.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public static class DelivaryProfileService
{
    public static async Task<Result<DelivaryProfile>> Insert(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        DelivaryProfileInsertModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<DelivaryProfile>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var id = await Db.InsertDelivaryProfileAsync(connection, model.Delivary, model.FirstName, model.SecondName, model.NationalName, model.NationalId, model.LicenseNumber, model.LicenseExpiredDate, model.DrivingLicenseNumber, model.DrivingLicenseExpiredDate);
            var delivaryProfile = new DelivaryProfile
            {
                Id = id,
                Delivary = model.Delivary,
                FirstName = model.FirstName,
                SecondName = model.SecondName,
                NationalName = model.NationalName,
                NationalId = model.NationalId,
                LicenseNumber = model.LicenseNumber,
                LicenseExpiredDate = model.LicenseExpiredDate,
                DrivingLicenseNumber = model.DrivingLicenseNumber,
                DrivingLicenseExpiredDate = model.DrivingLicenseExpiredDate
            };

            return Result<DelivaryProfile>.Ok(delivaryProfile);
        }
        catch (Exception e)
        {
            return Result<DelivaryProfile>.Fail(new[] { e.Message });
        }
    }

    public static async Task<Result<DelivaryProfile>> Update(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        DelivaryProfileUpdateModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<DelivaryProfile>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var existingModel = await Db.GetDelivaryProfileByIdAsync(model.Id, connection);

            if (existingModel is null)
            {
                return Result<DelivaryProfile>.Fail(new[] { "Not Found" });
            }
            existingModel.Delivary = model.Delivary ?? existingModel.Delivary;
            existingModel.FirstName = model.FirstName ?? existingModel.FirstName;
            existingModel.SecondName = model.SecondName ?? existingModel.SecondName;
            existingModel.NationalName = model.NationalName ?? existingModel.NationalName;
            existingModel.NationalId = model.NationalId ?? existingModel.NationalId;
            existingModel.LicenseNumber = model.LicenseNumber ?? existingModel.LicenseNumber;
            existingModel.LicenseExpiredDate = model.LicenseExpiredDate ?? existingModel.LicenseExpiredDate;
            existingModel.DrivingLicenseNumber = model.DrivingLicenseNumber ?? existingModel.DrivingLicenseNumber;
            existingModel.DrivingLicenseExpiredDate = model.DrivingLicenseExpiredDate ?? existingModel.DrivingLicenseExpiredDate;

            await Db.UpdateDelivaryProfileAsync(connection, 
                existingModel.Id,
                existingModel.Delivary,
                existingModel.FirstName,
                existingModel.SecondName,
                existingModel.NationalName,
                existingModel.NationalId,
                existingModel.LicenseNumber,
                existingModel.LicenseExpiredDate,
                existingModel.DrivingLicenseNumber,
                existingModel.DrivingLicenseExpiredDate);

            return Result<DelivaryProfile>.Ok(existingModel);
        }
        catch (Exception e)
        {
            return Result<DelivaryProfile>.Fail(new[] { e.Message });
        }
    }
}
