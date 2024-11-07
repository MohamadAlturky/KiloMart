using System.Data;
using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.DataAccess.Models;
using KiloMart.Domain.ProductCategories.Models;

namespace KiloMart.Domain.ProductCategories.Services;

public static class ProductCategoryLocalizedService
{
    public static Result<ProductCategoryLocalizedDto> Insert(IDbFactory dbFactory, ProductCategoryLocalizedDto localization)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            // Insert into ProductCategoryLocalized table
            const string sql = @"
                INSERT INTO ProductCategoryLocalized (Name, Language, ProductCategory)
                VALUES (@Name, @Language, @ProductCategory);";

            connection.Execute(sql, new { localization.Name, localization.Language, localization.ProductCategory });

            return Result<ProductCategoryLocalizedDto>.Ok(localization);
        }
        catch (Exception e)
        {
            return Result<ProductCategoryLocalizedDto>.Fail(new[] { e.Message });
        }
    }

    public static Result<ProductCategoryLocalizedDto> Insert(IDbConnection connection,
     IDbTransaction transaction,
     ProductCategoryLocalizedDto localization)
    {
        try
        {
            // Insert into ProductCategoryLocalized table
            const string sql = @"
                INSERT INTO ProductCategoryLocalized (Name, Language, ProductCategory)
                VALUES (@Name, @Language, @ProductCategory);";

            connection.Execute(sql, 
            new { 
                localization.Name,
                localization.Language,
                localization.ProductCategory
            }, transaction);

            return Result<ProductCategoryLocalizedDto>.Ok(localization);
        }
        catch (Exception e)
        {
            return Result<ProductCategoryLocalizedDto>.Fail([e.Message]);
        }
    }
}
