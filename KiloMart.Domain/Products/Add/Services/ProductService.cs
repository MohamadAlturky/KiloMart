namespace KiloMart.Domain.Products.Add.Services;

public static class ProductService
{
   public static Result<ProductDto> Insert(IDbFactory dbFactory, ProductDto product){
   
   try
        {
            using var connection = dbFactory.CreateDbConnection();
            using var transaction = connection.BeginTransaction();
            connection.Open();

            // Insert into Product table
            const string sql = @"
                INSERT INTO Product (ImageUrl, CategoryId, IsActive, MeasurementUnit, Name, Description)
                VALUES (@ImageUrl, @CategoryId, @IsActive, @MeasurementUnit, @Name, @Description);
                SELECT CAST(SCOPE_IDENTITY() as int);"; // Retrieve the generated Id

            // Execute the insert and retrieve the new Id
            product.Id = connection.QuerySingle<int>(sql, new { product.ImageUrl, product.CategoryId, product.IsActive, product.MeasurementUnit, product.Name, product.Description }, transaction);
            // Insert localizations if any
            if (product.Localizations != null && product.Localizations.Count > 0)
            {
                foreach (var localization in product.Localizations)
                {
                    localization.ProductId = product.Id;
                    var localizationResult = ProductLocalizedService.Insert(dbFactory, localization);
                    if (!localizationResult.Success)
                    {
                        return Result<ProductDto>.Fail();
                    }
                }
            }
            

            return Result<ProductDto>.Ok(product);
        }
        catch (Exception)
        {
            return Result<ProductDto>.Fail();
        }
   }
 
}