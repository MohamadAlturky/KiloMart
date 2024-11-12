using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Commands.Services;

public class ProductCategoryLocalizedInsertModel
{
    public string Name { get; set; } = null!;
    public byte Language { get; set; }
    public int ProductCategory { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Name is required.");

        if (Language <= 0)
            errors.Add("Language must be a positive number.");

        if (ProductCategory <= 0)
            errors.Add("ProductCategory must be a positive number.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public class ProductCategoryLocalizedUpdateModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public byte Language { get; set; }
    public int ProductCategory { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Id <= 0)
            errors.Add("Id must be a positive number.");

        return (errors.Count == 0, errors.ToArray());
    }
}

public static class ProductCategoryLocalizedService
{
    public static async Task<Result<ProductCategoryLocalized>> Insert(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        ProductCategoryLocalizedInsertModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<ProductCategoryLocalized>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var id = await Db.InsertProductCategoryLocalizedAsync(connection, model.Name, model.Language, model.ProductCategory);
            var productCategoryLocalized = new ProductCategoryLocalized
            {
                Name = model.Name,
                Language = model.Language,
                ProductCategory = model.ProductCategory
            };

            return Result<ProductCategoryLocalized>.Ok(productCategoryLocalized);
        }
        catch (Exception e)
        {
            return Result<ProductCategoryLocalized>.Fail([e.Message]);
        }
    }

    public static async Task<Result<ProductCategoryLocalized>> Update(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        ProductCategoryLocalizedUpdateModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<ProductCategoryLocalized>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var existingModel = await Db.GetProductCategoryLocalizedAsync(model.ProductCategory, model.Language, connection);

            if (existingModel is null)
            {
                return Result<ProductCategoryLocalized>.Fail(["Not Found"]);
            }

            existingModel.Name = model.Name ?? existingModel.Name;
            existingModel.Language = model.Language;

            await Db.UpdateProductCategoryLocalizedAsync(connection,
                existingModel.Name,
                existingModel.Language,
                existingModel.ProductCategory);

            return Result<ProductCategoryLocalized>.Ok(existingModel);
        }
        catch (Exception e)
        {
            return Result<ProductCategoryLocalized>.Fail([e.Message]);
        }
    }
}
