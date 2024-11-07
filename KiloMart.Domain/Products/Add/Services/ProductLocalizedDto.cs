namespace KiloMart.Domain.Products.Add.Services;

public static class ProductLocalizedDto
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
                INSERT INTO ProductLocalized (Language, ProductId, MeasurementUnit, Description, Name)
                VALUES (@Language, @ProductId, @MeasurementUnit, @Description, @Name);";

            connection.Execute(sql, new { localization.Language, localization.ProductId, localization.MeasurementUnit, localization.Description, localization.Name });

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
                INSERT INTO ProductLocalized (Language, ProductId, MeasurementUnit, Description, Name)
                VALUES (@Language, @ProductId, @MeasurementUnit, @Description, @Name);";

            connection.Execute(sql, new { 
                localization.Language, 
                localization.ProductId, 
                localization.MeasurementUnit, 
                localization.Description, 
                localization.Name }, transaction);

            return Result<ProductLocalizedDto>.Ok(localization);
        }
        catch (Exception)
        {
            return Result<ProductLocalizedDto>.Fail();
        }
    }
}