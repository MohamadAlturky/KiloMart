using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Commands.Services;

public class CardInsertModel
{
    public string HolderName { get; set; } = null!;
    public string Number { get; set; } = null!;
    public string SecurityCode { get; set; } = null!;
    public DateOnly ExpireDate { get; set; }
    public int Customer { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(HolderName))
            errors.Add("Holder name is required.");

        if (string.IsNullOrWhiteSpace(Number) || Number.Length != 16)
            errors.Add("Card number must be 16 digits.");

        if (string.IsNullOrWhiteSpace(SecurityCode) || SecurityCode.Length != 3)
            errors.Add("Security code must be 3 digits.");

        if (ExpireDate < DateOnly.FromDateTime(DateTime.Now))
            errors.Add("Expire date must be in the future.");

        if (Customer <= 0)
            errors.Add("Customer ID must be a positive number.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public class CardUpdateModel
{
    public int Id { get; set; }
    public string? HolderName { get; set; } 
    public string? Number { get; set; } 
    public string? SecurityCode { get; set; }
    public DateOnly? ExpireDate { get; set; }
    public bool? IsActive { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Id <= 0)
            errors.Add("Card ID must be a positive number.");

        return (errors.Count == 0, errors.ToArray());
    }
}
public static class CardService
{
    public static async Task<Result<Card>> Insert(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        CardInsertModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<Card>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var id = await Db.InsertCardAsync(connection, model.HolderName, model.Number, model.SecurityCode, model.ExpireDate, model.Customer);
            var card = new Card
            {
                Id = id,
                HolderName = model.HolderName,
                Number = model.Number,
                SecurityCode = model.SecurityCode,
                ExpireDate = model.ExpireDate,
                Customer = model.Customer,
                IsActive = true
            };

            return Result<Card>.Ok(card);
        }
        catch (Exception e)
        {
            return Result<Card>.Fail([e.Message]);

        }
    }

    public static async Task<Result<Card>> Update(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        CardUpdateModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<Card>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var existingModel = await Db.GetCardByIdAsync(model.Id, connection);
            
            if (existingModel is null)
            {
                return Result<Card>.Fail(["Not Found"]);
            }
            existingModel.ExpireDate = model.ExpireDate??
                                            existingModel.ExpireDate;
            existingModel.Number = model.Number??
                                            existingModel.Number;
            existingModel.HolderName = model.HolderName??
                                            existingModel.HolderName;
            existingModel.IsActive = model.IsActive??
                                            existingModel.IsActive;
            existingModel.SecurityCode = model.SecurityCode??
                                            existingModel.SecurityCode;

            await Db.UpdateCardAsync(connection, 
                existingModel.Id,
                existingModel.HolderName,
                existingModel.Number,
                existingModel.SecurityCode,
                existingModel.ExpireDate,
                existingModel.Customer,
                existingModel.IsActive);

            return Result<Card>.Ok(existingModel);

        }
        catch (Exception e)
        {
            return Result<Card>.Fail([e.Message]);

        }
    }
}
