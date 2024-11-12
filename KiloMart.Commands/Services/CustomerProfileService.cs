using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Commands.Services;
public class CustomerProfileInsertModel
{
    public int Customer { get; set; }
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string NationalName { get; set; } = null!;
    public string NationalId { get; set; } = null!;

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Customer <= 0)
        {
            errors.Add("Customer ID must be a positive number.");
        }

        if (string.IsNullOrWhiteSpace(FirstName))
        {
            errors.Add("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(SecondName))
        {
            errors.Add("Second name is required.");
        }

        if (string.IsNullOrWhiteSpace(NationalName))
        {
            errors.Add("National name is required.");
        }

        if (string.IsNullOrWhiteSpace(NationalId))
        {
            errors.Add("National ID is required.");
        }

        return (errors.Count == 0, errors.ToArray());
    }
}

public class CustomerProfileUpdateModel
{
    public int Id { get; set; }
    public int? Customer { get; set; }
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? NationalName { get; set; }
    public string? NationalId { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Id <= 0)
        {
            errors.Add("Customer profile ID must be a positive number.");
        }

        return (errors.Count == 0, errors.ToArray());
    }
}

public static class CustomerProfileService
{
    public static async Task<Result<CustomerProfile>> Insert(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        CustomerProfileInsertModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<CustomerProfile>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var id = await Db.InsertCustomerProfileAsync(connection, model.Customer, model.FirstName, model.SecondName, model.NationalName, model.NationalId);
            var customerProfile = new CustomerProfile
            {
                Id = id,
                Customer = model.Customer,
                FirstName = model.FirstName,
                SecondName = model.SecondName,
                NationalName = model.NationalName,
                NationalId = model.NationalId
            };

            return Result<CustomerProfile>.Ok(customerProfile);
        }
        catch (Exception e)
        {
            return Result<CustomerProfile>.Fail([e.Message]);

        }
    }

    public static async Task<Result<CustomerProfile>> Update(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        CustomerProfileUpdateModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<CustomerProfile>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var existingModel = await Db.GetCustomerProfileByIdAsync(model.Id, connection);

            if (existingModel is null)
            {
                return Result<CustomerProfile>.Fail(["Not Found"]);
            }

            existingModel.Customer = model.Customer ?? existingModel.Customer;
            existingModel.FirstName = model.FirstName ?? existingModel.FirstName;
            existingModel.SecondName = model.SecondName ?? existingModel.SecondName;
            existingModel.NationalName = model.NationalName ?? existingModel.NationalName;
            existingModel.NationalId = model.NationalId ?? existingModel.NationalId;

            await Db.UpdateCustomerProfileAsync(connection,
                existingModel.Id,
                existingModel.Customer,
                existingModel.FirstName,
                existingModel.SecondName,
                existingModel.NationalName,
                existingModel.NationalId);

            return Result<CustomerProfile>.Ok(existingModel);
        }
        catch (Exception e)
        {
            return Result<CustomerProfile>.Fail([e.Message]);

        }
    }
}
