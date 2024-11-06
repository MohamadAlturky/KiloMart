
using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.DataAccess.Models;
using KiloMart.Domain.ProductCategories.Models;

namespace KiloMart.Domain.ProductCategories.Services;

public static class ProductCategoryService
{
    public static Result<ProductCategoryDto> Insert(IDbFactory dbFactory, ProductCategoryDto category)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            using var transaction = connection.BeginTransaction();
            connection.Open();

            // Insert into ProductCategory table
            const string sql = @"
                INSERT INTO ProductCategory (IsActive, Name)
                VALUES (@IsActive, @Name);
                SELECT CAST(SCOPE_IDENTITY() as int);"; // Retrieve the generated Id

            // Execute the insert and retrieve the new Id
            category.Id = connection.QuerySingle<int>(sql, new { category.IsActive, category.Name }, transaction);

            // Insert localizations if any
            if (category.Localizations != null && category.Localizations.Count > 0)
            {
                foreach (var localization in category.Localizations)
                {
                    localization.ProductCategory = category.Id;
                    var localizationResult = ProductCategoryLocalizedService.Insert(dbFactory, localization);
                    
                    // If any localization insert fails, return a failure result
                    if (!localizationResult.Success)
                    {
                        return Result<ProductCategoryDto>.Fail();
                    }
                }
            }

            return Result<ProductCategoryDto>.Ok(category);
        }
        catch (Exception)
        {
            return Result<ProductCategoryDto>.Fail();
        }
    }
}
