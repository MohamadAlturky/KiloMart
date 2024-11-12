using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Commands.Services;
public static class ProviderProfileService
{
    public static async Task<Result<ProviderProfile>> Insert(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        ProviderProfileInsertModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<ProviderProfile>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var id = await Db.InsertProviderProfileAsync(connection, model.Provider, model.FirstName, model.SecondName, model.OwnerNationalId, model.NationalApprovalId, model.CompanyName, model.OwnerName);
            var providerProfile = new ProviderProfile
            {
                Id = id,
                Provider = model.Provider,
                FirstName = model.FirstName,
                SecondName = model.SecondName,
                OwnerNationalId = model.OwnerNationalId,
                NationalApprovalId = model.NationalApprovalId,
                CompanyName = model.CompanyName,
                OwnerName = model.OwnerName
            };

            return Result<ProviderProfile>.Ok(providerProfile);
        }
        catch (Exception e)
        {
            return Result<ProviderProfile>.Fail([e.Message]);
        }
    }

    public static async Task<Result<ProviderProfile>> Update(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        ProviderProfileUpdateModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<ProviderProfile>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var existingModel = await Db.GetProviderProfileByIdAsync(model.Id, connection);

            if (existingModel is null)
            {
                return Result<ProviderProfile>.Fail(["Not Found"]);
            }
            existingModel.Provider = model.Provider ?? existingModel.Provider;
            existingModel.FirstName = model.FirstName ?? existingModel.FirstName;
            existingModel.SecondName = model.SecondName ?? existingModel.SecondName;
            existingModel.OwnerNationalId = model.OwnerNationalId ?? existingModel.OwnerNationalId;
            existingModel.NationalApprovalId = model.NationalApprovalId ?? existingModel.NationalApprovalId;
            existingModel.CompanyName = model.CompanyName ?? existingModel.CompanyName;
            existingModel.OwnerName = model.OwnerName ?? existingModel.OwnerName;

            await Db.UpdateProviderProfileAsync(connection,
                existingModel.Id,
                existingModel.Provider,
                existingModel.FirstName,
                existingModel.SecondName,
                existingModel.OwnerNationalId,
                existingModel.NationalApprovalId,
                existingModel.CompanyName,
                existingModel.OwnerName);

            return Result<ProviderProfile>.Ok(existingModel);

        }
        catch (Exception e)
        {
            return Result<ProviderProfile>.Fail([e.Message]);
        }
    }
}

public class ProviderProfileInsertModel
{
    public int Provider { get; set; }
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string OwnerNationalId { get; set; } = null!;
    public string NationalApprovalId { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string OwnerName { get; set; } = null!;

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(FirstName))
            errors.Add("First name is required.");

        if (string.IsNullOrWhiteSpace(SecondName))
            errors.Add("Second name is required.");

        if (string.IsNullOrWhiteSpace(OwnerNationalId))
            errors.Add("Owner national ID is required.");

        if (string.IsNullOrWhiteSpace(NationalApprovalId))
            errors.Add("National approval ID is required.");

        if (string.IsNullOrWhiteSpace(CompanyName))
            errors.Add("Company name is required.");

        if (string.IsNullOrWhiteSpace(OwnerName))
            errors.Add("Owner name is required.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public class ProviderProfileUpdateModel
{
    public int Id { get; set; }
    public int? Provider { get; set; }
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? OwnerNationalId { get; set; }
    public string? NationalApprovalId { get; set; }
    public string? CompanyName { get; set; }
    public string? OwnerName { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Id <= 0)
            errors.Add("Provider profile ID must be a positive number.");

        return (errors.Count == 0, errors.ToArray());
    }
}
