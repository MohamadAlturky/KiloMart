using System.Data;
using Dapper;

namespace KiloMart.Requests.Queries;

public partial class Query
{
    public static async Task<IEnumerable<ProductDetailDto>> 
        GetProductDetailsByLanguageAsync(IDbConnection connection, string language)
    {
        const string query = @"
        SELECT 
            pd.ProductId,
            pd.ProductImageUrl,
            pd.ProductProductCategory,
            pd.ProductIsActive,
            pd.ProductMeasurementUnit,
            pd.ProductDescription,
            pd.ProductName,
            pc.ProductCategoryId,
            pc.ProductCategoryName,
            pc.ProductCategoryIsActive
        FROM GetProductDetailsByLanguageFN(@language) pd
        INNER JOIN GetProductCategoryDetailsByLanguageFN(@language) pc
        ON pc.ProductCategoryId = pd.ProductProductCategory";

        var parameters = new { language };

        return await connection.QueryAsync<ProductDetailDto>(query, parameters);
    }
    public static async Task<IEnumerable<ProductDetailDto>> 
        GetActiveProductDetailsByLanguageAsync(IDbConnection connection, string language)
    {
        const string query = @"
        SELECT 
            pd.ProductId,
            pd.ProductImageUrl,
            pd.ProductProductCategory,
            pd.ProductIsActive,
            pd.ProductMeasurementUnit,
            pd.ProductDescription,
            pd.ProductName,
            pc.ProductCategoryId,
            pc.ProductCategoryName,
            pc.ProductCategoryIsActive
        FROM GetProductDetailsByLanguageFN(@language) pd
        INNER JOIN GetProductCategoryDetailsByLanguageFN(@language) pc
        ON pc.ProductCategoryId = pd.ProductProductCategory
        WHERE pd.ProductIsActive = 1;";

        var parameters = new { language };

        return await connection.QueryAsync<ProductDetailDto>(query, parameters);
    }
    public static async Task<IEnumerable<ProductDetailDto>> 
        GetActiveProductDetailsByLanguageAndCategoryAsync(IDbConnection connection,
         string language,
         int category)
    {
        const string query = @"
        SELECT 
            pd.ProductId,
            pd.ProductImageUrl,
            pd.ProductProductCategory,
            pd.ProductIsActive,
            pd.ProductMeasurementUnit,
            pd.ProductDescription,
            pd.ProductName,
            pc.ProductCategoryId,
            pc.ProductCategoryName,
            pc.ProductCategoryIsActive
        FROM GetProductDetailsByLanguageFN(@language) pd
        INNER JOIN GetProductCategoryDetailsByLanguageFN(@language) pc
        ON pc.ProductCategoryId = pd.ProductProductCategory
        WHERE pd.ProductIsActive = 1 AND pd.ProductProductCategory = @category;";

        var parameters = new { language, category };

        return await connection.QueryAsync<ProductDetailDto>(query, parameters);
    }
}

public class ProductDetailDto
{
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; } = null!;
    public int ProductProductCategory { get; set; }
    public bool ProductIsActive { get; set; }
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int ProductCategoryId { get; set; }
    public string ProductCategoryName { get; set; } = null!;
    public bool ProductCategoryIsActive { get; set; }
}
