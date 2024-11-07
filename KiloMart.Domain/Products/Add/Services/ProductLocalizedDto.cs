using System.Data;
using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.DataAccess.Models;
using KiloMart.Domain.Products.Add.Models;

namespace KiloMart.Domain.Products.Add.Services;

public static class ProductLocalizedService
{
    //write the static method Insert
    public static Result<ProductLocalizedDto> Insert(IDbFactory dbFactory, ProductLocalizedDto localization)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();
            
            // Insert into ProductLocalized table
            const string sql = @"
                INSERT INTO ProductLocalized (Language, Product, MeasurementUnit, Description, Name)
                VALUES (@Language, @Product, @MeasurementUnit, @Description, @Name);";

            connection.Execute(sql, new {
                Language = localization.Language, 
                Product = localization.Product,
                MeasurementUnit = localization.MeasurementUnit,
                Description = localization.Description,
                Name = localization.Name
            });

            return Result<ProductLocalizedDto>.Ok(localization);
        }
        catch (Exception)
        {
            return Result<ProductLocalizedDto>.Fail();
        }
    }
    public static Result<ProductLocalizedDto> Insert(IDbConnection connection,
     IDbTransaction transaction,
     ProductLocalizedDto localization)
    {
        try
        {
            // Insert into ProductLocalized table
            const string sql = @"
                INSERT INTO ProductLocalized (Language, Product, MeasurementUnit, Description, Name)
                VALUES (@Language, @Product, @MeasurementUnit, @Description, @Name);";

            connection.Execute(sql, new { 
                Language = localization.Language, 
                Product = localization.Product, 
                MeasurementUnit = localization.MeasurementUnit, 
                Description = localization.Description, 
                Name = localization.Name }, transaction);

            return Result<ProductLocalizedDto>.Ok(localization);
        }
        catch (Exception)
        {
            return Result<ProductLocalizedDto>.Fail();
        }
    }
}