using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Languages.Models;


namespace KiloMart.Commands.Services;

public static class ProductCategoryService
{
    public class ProductCategoryInsertModel
    {
        public string Name { get; set; } = null!;

        public (bool Success, string[] Errors) Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Name is required.");

            return (errors.Count == 0, errors.ToArray());
        }
    }

    public class ProductCategoryUpdateModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }

        public (bool Success, string[] Errors) Validate()
        {
            var errors = new List<string>();

            if (Id <= 0)
                errors.Add("Product category ID must be a positive number.");

            return (errors.Count == 0, errors.ToArray());
        }
    }

    public static async Task<Result<InsertProductCategoryLocalizedResponse>> InsertProductWithLocalization(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        ProductCategoryLocalizedRequest model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<InsertProductCategoryLocalizedResponse>.Fail(errors);
        }
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {

            var id = await Db.InsertProductCategoryAsync(connection, model.Arabic.Name, transaction);
            var productCategory = new ProductCategory
            {
                Id = id,
                Name = model.Arabic.Name,
                IsActive = true
            };
            await Db.InsertProductCategoryLocalizedAsync(
                connection,
                model.Arabic.Name,
                (byte)Language.Arabic,
                id,
                transaction);

            await Db.InsertProductCategoryLocalizedAsync(
                connection,
                model.Endlish.Name,
                (byte)Language.English,
                id,
                transaction);
            
            transaction.Commit();
            return Result<InsertProductCategoryLocalizedResponse>.Ok(new()
            {
                ProductCategory = productCategory,
                ArabicProductCategory = new()
                {
                    Language = (byte)Language.Arabic,
                    Name = model.Arabic.Name,
                    ProductCategory = productCategory.Id
                },
                EnglishProductCategory = new()
                {
                    Language = (byte)Language.English,
                    Name = model.Endlish.Name,
                    ProductCategory = productCategory.Id
                },
            });
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return Result<InsertProductCategoryLocalizedResponse>.Fail([e.Message]);
        }
    }
    public static async Task<Result<ProductCategory>> Insert(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        ProductCategoryInsertModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<ProductCategory>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var id = await Db.InsertProductCategoryAsync(connection, model.Name);
            var productCategory = new ProductCategory
            {
                Id = id,
                Name = model.Name,
                IsActive = true
            };

            return Result<ProductCategory>.Ok(productCategory);
        }
        catch (Exception e)
        {
            return Result<ProductCategory>.Fail([e.Message]);
        }
    }

    public static async Task<Result<ProductCategory>> Update(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        ProductCategoryUpdateModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<ProductCategory>.Fail(errors);
        }

        try
        {
            var connection = dbFactory.CreateDbConnection();
            connection.Open();
            var existingModel = await Db.GetProductCategoryByIdAsync(model.Id, connection);

            if (existingModel is null)
            {
                return Result<ProductCategory>.Fail(["Not Found"]);
            }
            existingModel.Name = model.Name ?? existingModel.Name;
            existingModel.IsActive = model.IsActive ?? existingModel.IsActive;

            await Db.UpdateProductCategoryAsync(connection,
                existingModel.Id,
                existingModel.Name,
                existingModel.IsActive);

            return Result<ProductCategory>.Ok(existingModel);
        }
        catch (Exception e)
        {
            return Result<ProductCategory>.Fail([e.Message]);
        }
    }
}

public class ProductCategoryLocalizedRequest
{
    public ProductCategoryLocalizedRecord Arabic = new();
    public ProductCategoryLocalizedRecord Endlish = new();

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();
        var arabicValidation = Arabic.Validate();
        var englishValidation = Arabic.Validate();
        if (!arabicValidation.Success)
        {
            errors.Add("Error in Arabic Data");
            errors.AddRange(arabicValidation.Errors);
        }
        if (!englishValidation.Success)
        {
            errors.Add("Error in English Data");
            errors.AddRange(englishValidation.Errors);
        }
        return (errors.Count == 0, errors.ToArray());
    }
}

public class ProductCategoryLocalizedRecord
{
    public string Name { get; set; } = null!;
    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Name is required");

        return (errors.Count == 0, errors.ToArray());
    }
}

public class InsertProductCategoryLocalizedResponse
{
    public ProductCategory ProductCategory { get; set; }
    public ProductCategoryLocalized ArabicProductCategory { get; set; }
    public ProductCategoryLocalized EnglishProductCategory { get; set; }
}