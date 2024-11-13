using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.Domain.Products.Add.Models;

namespace KiloMart.Domain.Products.Add.Services;

public static class ProductService
{
   public static Result<ProductDto> Insert(IDbFactory dbFactory, ProductDto product){
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {


            // Insert into Product table
            const string sql = @"
                INSERT INTO Product (ImageUrl, ProductCategory, IsActive, MeasurementUnit, Name, Description)
                VALUES (@ImageUrl, @ProductCategory, @IsActive, @MeasurementUnit, @Name, @Description);
                SELECT CAST(SCOPE_IDENTITY() as int);"; // Retrieve the generated Id

            // Execute the insert and retrieve the new Id
            product.Id = connection.QuerySingle<int>(sql, 
            new { 
                ImageUrl = product.ImageUrl,
                ProductCategory = product.CategoryId,
                IsActive = product.IsActive, 
                MeasurementUnit = product.MeasurementUnit, 
                Name = product.Name, 
                Description = product.Description 
                }, transaction);
            // Insert localizations if any
            if (product.Localizations != null && product.Localizations.Count > 0)
            {
                foreach (var localization in product.Localizations)
                {
                    localization.Product = product.Id;
                    var localizationResult = ProductLocalizedService.Insert(connection, transaction, localization);
                    if (!localizationResult.Success)
                    {
                        transaction.Rollback();
                        return Result<ProductDto>.Fail(localizationResult.Errors);
                    }
                }
            }
            transaction.Commit();

            return Result<ProductDto>.Ok(product);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return Result<ProductDto>.Fail(new[] { e.Message });
        }
   }
 
}