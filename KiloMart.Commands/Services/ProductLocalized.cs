// c:\New folder\Workers\KiloMart\KiloMart.Commands\Services\ProductLocalizedService.cs
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Commands.Services;

public class ProductLocalizedInsertModel
{
    public int Language { get; set; }
    public int Product { get; set; }
    public string MeasurementUnit { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Name { get; set; } = null!;

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Language <= 0)
            errors.Add("Language ID must be a positive number.");

        if (Product <= 0)
            errors.Add("Product ID must be a positive number.");

        if (string.IsNullOrWhiteSpace(MeasurementUnit))
            errors.Add("Measurement unit is required.");

        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("Description is required.");

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Name is required.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public class ProductLocalizedUpdateModel
{
    public int Id { get; set; }
    public int? Language { get; set; }
    public int? Product { get; set; }
    public string? MeasurementUnit { get; set; }
    public string? Description { get; set; }
    public string? Name { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Id <= 0)
            errors.Add("Product localized ID must be a positive number.");

        if (Language.HasValue && Language <= 0)
            errors.Add("Language ID must be a positive number.");

        if (Product.HasValue && Product <= 0)
            errors.Add("Product ID must be a positive number.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public static class ProductLocalizedService
{
    public static async Task<Result<ProductLocalized>> Insert(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        ProductLocalizedInsertModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<ProductLocalized>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            await Db.InsertProductLocalizedAsync(connection, model.Language, model.Product, model.MeasurementUnit, model.Description, model.Name);

            var productLocalized = await Db.GetProductLocalizedByLanguageAndProductAsync(model.Language, model.Product, connection);

            return Result<ProductLocalized>.Ok(productLocalized);
        }
        catch (Exception e)
        {
            return Result<ProductLocalized>.Fail([e.Message]);
        }
    }

    public static async Task<Result<ProductLocalized>> Update(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        ProductLocalizedUpdateModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<ProductLocalized>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();

            var existingModel = await Db.GetProductLocalizedByLanguageAndProductAsync(model.Language ?? 0, model.Product ?? 0, connection);

            if (existingModel is null)
            {
                return Result<ProductLocalized>.Fail(["Not Found"]);
            }

            existingModel.MeasurementUnit = model.MeasurementUnit ?? existingModel.MeasurementUnit;
            existingModel.Description = model.Description ?? existingModel.Description;
            existingModel.Name = model.Name ?? existingModel.Name;

            if (model.Language.HasValue)
                existingModel.Language = model.Language.Value;

            if (model.Product.HasValue)
                existingModel.Product = model.Product.Value;

            await Db.UpdateProductLocalizedAsync(connection, existingModel.Language, existingModel.Product, existingModel.MeasurementUnit, existingModel.Description, existingModel.Name);

            return Result<ProductLocalized>.Ok(existingModel);
        }
        catch (Exception e)
        {
            return Result<ProductLocalized>.Fail([e.Message]);
        }
    }
}
