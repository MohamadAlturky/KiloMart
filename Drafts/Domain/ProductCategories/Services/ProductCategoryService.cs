
using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.Domain.ProductCategories.Models;

namespace KiloMart.Domain.ProductCategories.Services;

public static class ProductCategoryService
{
    public static Result<ProductCategoryDto> Insert(IDbFactory dbFactory, ProductCategoryDto category)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {


            // Insert into ProductCategory table
            const string sql = @"
                INSERT INTO ProductCategory (IsActive, Name)
                VALUES (@IsActive, @Name);
                SELECT CAST(SCOPE_IDENTITY() as int);"; // Retrieve the generated Id

            // Execute the insert and retrieve the new Id
            category.Id = connection.QuerySingle<int>(sql, new { category.IsActive, category.Name }, transaction);

            // Insert localizations if any
            if (category.Localizations is not null && category.Localizations.Count > 0)
            {
                foreach (var localization in category.Localizations)
                {
                    localization.ProductCategory = category.Id;
                    var localizationResult = ProductCategoryLocalizedService.Insert(connection, transaction, localization);
                    
                    // If any localization insert fails, return a failure result
                    if (!localizationResult.Success)
                    {
                        transaction.Rollback();
                        return Result<ProductCategoryDto>.Fail(localizationResult.Errors);
                    }
                }
            }
            transaction.Commit();
            return Result<ProductCategoryDto>.Ok(category);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return Result<ProductCategoryDto>.Fail([e.Message]);
        }
    }
}
