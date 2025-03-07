using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.DateServices;

namespace KiloMart.Commands.Services;

public class CardInsertModel
{
    public string HolderName { get; set; } = null!;
    public string Number { get; set; } = null!;
    public string SecurityCode { get; set; } = null!;
    public DateTime ExpireDate { get; set; }
    public bool IsPrimary { get; set; } = false;

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(HolderName))
            errors.Add("Holder name is required.");

        if (string.IsNullOrWhiteSpace(Number))
            errors.Add("Card number is required.");

        if (string.IsNullOrWhiteSpace(SecurityCode))
            errors.Add("Security code is required.");

        if (ExpireDate < SaudiDateTimeHelper.GetCurrentTime())
            errors.Add("Expire date must be in the future.");


        return (errors.Count == 0, errors.ToArray());
    }
}

public class CardUpdateModel
{
    public int Id { get; set; }
    public string? HolderName { get; set; }
    public string? Number { get; set; }
    public string? SecurityCode { get; set; }
    public DateTime? ExpireDate { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsPrimary { get; set; }

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
    public static async Task<Result<bool>> SetCardAsPrimary(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        int cardId)
    {
        var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            var card = await Db.GetCardByIdAsync(cardId, connection, transaction);
            
            if (card == null)
                return Result<bool>.Fail(["Card not found"]);
                
            if (card.Customer != userPayLoad.Party)
                return Result<bool>.Fail(["You don't have permission to modify this card"]);
            
            await Db.SetAllCardsNonPrimaryAsync(connection, userPayLoad.Party, transaction);

            var success = await Db.SetCardAsPrimaryAsync(connection, cardId, userPayLoad.Party, transaction);
            
            if (!success)
                return Result<bool>.Fail(["Failed to set card as primary"]);

            transaction.Commit();
            return Result<bool>.Ok(true);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return Result<bool>.Fail([e.Message]);
        }
    }
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
        var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {

            if (model.IsPrimary)
            {
                await Db.SetAllCardsNonPrimaryAsync(connection, userPayLoad.Party, transaction);
            }
            var id = await Db.InsertCardAsync(connection,
            model.HolderName, model.Number,
             model.SecurityCode,
             model.ExpireDate,
              userPayLoad.Party,
              model.IsPrimary,
              transaction);

            transaction.Commit();
            var card = new Card
            {
                Id = id,
                HolderName = model.HolderName,
                Number = model.Number,
                SecurityCode = model.SecurityCode,
                ExpireDate = model.ExpireDate,
                Customer = userPayLoad.Party,
                IsActive = true,
                IsPrimary = model.IsPrimary
            };

            return Result<Card>.Ok(card);
        }
        catch (Exception e)
        {
            transaction.Rollback();
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
        var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {

            var existingModel = await Db.GetCardByIdAsync(model.Id, connection, transaction);

            if (existingModel is null)
            {
                return Result<Card>.Fail(["Not Found"]);
            }
            if (existingModel.Customer != userPayLoad.Party)
            {
                return Result<Card>.Fail(["Un Authorized"]);
            }
            if (model.IsPrimary.HasValue && model.IsPrimary.Value == true)
            {
                await Db.SetAllCardsNonPrimaryAsync(connection, userPayLoad.Party, transaction);
            }
            existingModel.ExpireDate = model.ExpireDate ??
                                            existingModel.ExpireDate;
            existingModel.Number = model.Number ??
                                            existingModel.Number;
            existingModel.HolderName = model.HolderName ??
                                            existingModel.HolderName;
            existingModel.IsActive = model.IsActive ??
                                            existingModel.IsActive;
            existingModel.SecurityCode = model.SecurityCode ??
                                            existingModel.SecurityCode;
            existingModel.IsPrimary = model.IsPrimary ??
                                            existingModel.IsPrimary;

            await Db.UpdateCardAsync(connection,
                existingModel.Id,
                existingModel.HolderName,
                existingModel.Number,
                existingModel.SecurityCode,
                existingModel.ExpireDate,
                existingModel.Customer,
                existingModel.IsActive,
                existingModel.IsPrimary,
                transaction);

            transaction.Commit();

            return Result<Card>.Ok(existingModel);

        }
        catch (Exception e)
        {
            transaction.Rollback();
            return Result<Card>.Fail([e.Message]);
        }
    }
}
