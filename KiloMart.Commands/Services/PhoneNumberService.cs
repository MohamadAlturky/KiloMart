using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Commands.Services;

public class PhoneNumberInsertModel
{
    public string Value { get; set; } = null!;

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Value))
            errors.Add("Value is required.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public class PhoneNumberUpdateModel
{
    public int Id { get; set; }
    public string? Value { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Id <= 0)
            errors.Add("Id must be a positive number.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public static class PhoneNumberService
{
    public static async Task<Result<PhoneNumber>> Insert(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        PhoneNumberInsertModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<PhoneNumber>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var id = await Db.InsertPhoneNumberAsync(connection, model.Value, userPayLoad.Party);
            var phoneNumber = new PhoneNumber
            {
                Id = id,
                Value = model.Value,
                Party = userPayLoad.Party,
                IsActive = true
            };

            return Result<PhoneNumber>.Ok(phoneNumber);
        }
        catch (Exception e)
        {
            return Result<PhoneNumber>.Fail([e.Message]);
        }
    }

    public static async Task<Result<PhoneNumber>> Update(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        PhoneNumberUpdateModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<PhoneNumber>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var existingModel = await Db.GetPhoneNumberByIdAsync(model.Id, connection);

            if (existingModel is null)
            {
                return Result<PhoneNumber>.Fail(["Not Found"]);
            }

            if(existingModel.Party != userPayLoad.Party)
            {
                return Result<PhoneNumber>.Fail(["Un Authorized"]);
            }

            existingModel.Value = model.Value ?? existingModel.Value;

            await Db.UpdatePhoneNumberAsync(connection,
                existingModel.Id,
                existingModel.Value,
                existingModel.Party,
                existingModel.IsActive);

            return Result<PhoneNumber>.Ok(existingModel);

        }
        catch (Exception e)
        {
            return Result<PhoneNumber>.Fail([e.Message]);
        }
    }

    public static async Task<Result<PhoneNumber>> Delete(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        int id)
    {
        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var existingModel = await Db.GetPhoneNumberByIdAsync(id, connection);

            if (existingModel is null)
            {
                return Result<PhoneNumber>.Fail(["Not Found"]);
            }

            await Db.DeletePhoneNumberAsync(connection, id);

            return Result<PhoneNumber>.Ok(existingModel);

        }
        catch (Exception e)
        {
            return Result<PhoneNumber>.Fail([e.Message]);
        }
    }
}
