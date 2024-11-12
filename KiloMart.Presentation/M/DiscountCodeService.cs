using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Commands.Services;

public class DiscountCodeInsertModel
{
    public string Code { get; set; } = null!;
    public decimal Value { get; set; }
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public byte DiscountType { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Code))
            errors.Add("Code is required.");

        if (Value <= 0)
            errors.Add("Value must be a positive number.");

        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("Description is required.");

        if (StartDate < DateTime.Now)
            errors.Add("Start date must be in the future.");

        if (EndDate.HasValue && EndDate < StartDate)
            errors.Add("End date must be after start date.");

        if (DiscountType < 1 || DiscountType > 2)
            errors.Add("DiscountType must be 1 or 2.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public class DiscountCodeUpdateModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public decimal? Value { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public byte? DiscountType { get; set; }
    public bool? IsActive { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Id <= 0)
            errors.Add("DiscountCode ID must be a positive number.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public static class DiscountCodeService
{
    public static async Task<Result<DiscountCode>> Insert(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        DiscountCodeInsertModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<DiscountCode>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var id = await Db.InsertDiscountCodeAsync(connection, model.Code, model.Value, model.Description, model.StartDate, model.EndDate, model.DiscountType);
            var discountCode = new DiscountCode
            {
                Id = id,
                Code = model.Code,
                Value = model.Value,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DiscountType = model.DiscountType,
                IsActive = true
            };

            return Result<DiscountCode>.Ok(discountCode);
        }
        catch (Exception e)
        {
            return Result<DiscountCode>.Fail(new[] { e.Message });
        }
    }

    public static async Task<Result<DiscountCode>> Update(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        DiscountCodeUpdateModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<DiscountCode>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var existingModel = await Db.GetDiscountCodeByIdAsync(model.Id, connection);

            if (existingModel is null)
            {
                return Result<DiscountCode>.Fail(new[] { "Not Found" });
            }

            existingModel.Code = model.Code ?? existingModel.Code;
            existingModel.Value = model.Value ?? existingModel.Value;
            existingModel.Description = model.Description ?? existingModel.Description;
            existingModel.StartDate = model.StartDate ?? existingModel.StartDate;
            existingModel.EndDate = model.EndDate ?? existingModel.EndDate;
            existingModel.DiscountType = model.DiscountType ?? existingModel.DiscountType;
            existingModel.IsActive = model.IsActive ?? existingModel.IsActive;

            await Db.UpdateDiscountCodeAsync(connection,
                existingModel.Id,
                existingModel.Code,
                existingModel.Value,
                existingModel.Description,
                existingModel.StartDate,
                existingModel.EndDate,
                existingModel.DiscountType,
                existingModel.IsActive);

            return Result<DiscountCode>.Ok(existingModel);
        }
        catch (Exception e)
        {
            return Result<DiscountCode>.Fail(new[] { e.Message });
        }
    }
}
