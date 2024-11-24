using System.Data;
using Dapper;

namespace KiloMart.Requests.Queries;

public partial class ProductQuery
{
    public static async Task<(List<CustomerProductApiResponseWithOfferDto> Products, int TotalCount)> GetProductsWithOfferPaginated(
        IDbConnection connection,
        int category,
        byte language = 1,
        int page = 1,
        int pageSize = 10)
    {
        bool isActive = true;
        int offset = (page - 1) * pageSize;

        // Count total products matching the criteria
        var countSql = @"
                SELECT COUNT(*) 
                FROM Product p WITH (NOLOCK)
                INNER JOIN (
                    SELECT Product, MAX(Price) AS MaxPrice, MIN(Price) AS MinPrice, Sum(Quantity) AS QuantitySum
                    FROM ProductOffer
                    WHERE IsActive = 1
                    GROUP BY Product
                ) po ON p.Id = po.Product
                WHERE p.IsActive = @isActive AND ProductCategory = @category AND QuantitySum != 0";

        int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { isActive, category });

        // SQL query with pagination
        string sql = @"
                SELECT
                    p.[Id],
                    p.[ImageUrl],
                    p.[ProductCategory],
                    p.[IsActive],
                    p.[MeasurementUnit],
                    p.[Description],
                    p.[Name],
                    pl.[Language],
                    pl.[Product],
                    pl.[MeasurementUnit] AS LocalizedMeasurementUnit,
                    pl.[Description] AS LocalizedDescription,
                    pl.[Name] AS LocalizedName,
                    po.MaxPrice,
                    po.MinPrice
                FROM Product p WITH (NOLOCK)
                LEFT JOIN ProductLocalized pl WITH (NOLOCK)
                    ON p.Id = pl.Product AND pl.Language = @language
                INNER JOIN (
                    SELECT Product, MAX(Price) AS MaxPrice, MIN(Price) AS MinPrice, Sum(Quantity) AS QuantitySum
                    FROM ProductOffer
                    WHERE IsActive = 1
                    GROUP BY Product
                ) po ON p.Id = po.Product
                WHERE p.IsActive = @isActive AND ProductCategory = @category AND QuantitySum != 0
                ORDER BY p.[Id]
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        // Execute query and transform results into DTOs
        var products = await connection.QueryAsync<CustomerProductApiResponseWithOffer>(sql, new { language, offset, pageSize, isActive, category });

        var result = products.Select(product => new CustomerProductApiResponseWithOfferDto
        {
            Id = product.Id,
            Name = product.LocalizedName ?? product.Name,
            IsActive = product.IsActive,
            ImageUrl = product.ImageUrl,
            MinPrice = product.MinPrice,
            MaxPrice = product.MaxPrice,
            MeasurementUnit = product.LocalizedMeasurementUnit ?? product.MeasurementUnit,
            Description = product.LocalizedDescription ?? product.Description,
        }).ToList();

        return (result, totalCount);
    }


    public static async Task<List<BestDealsByOffDto>> GetBestDealsByOffPercentage(
        IDbConnection connection,
        byte language)
    {
        const string sql = @"
            SELECT
                pd.ProductId,
                pd.ProductImageUrl,
                pd.ProductProductCategory,
                pd.ProductIsActive,
                pd.ProductMeasurementUnit,
                pd.ProductDescription,
                pd.ProductName,
                MAX(po.OffPercentage) AS OffPercentage
            FROM 
                GetProductDetailsByLanguageFN(@Language) pd 
            INNER JOIN 
                ProductOffer po ON po.Product = pd.ProductId AND po.IsActive = 1 AND po.Quantity > 0
            GROUP BY 
                pd.ProductId,
                pd.ProductImageUrl,
                pd.ProductProductCategory,
                pd.ProductIsActive,
                pd.ProductMeasurementUnit,
                pd.ProductDescription,
                pd.ProductName";

        var parameters = new { Language = language };

        try
        {
            var result = await connection.QueryAsync<BestDealsByOffDto>(sql, parameters);
            return result.AsList();
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception("Error occurred while fetching best deals", ex);
        }
    }
    public static async Task<List<BestDealsByMultiplyDto>> GetBestDealsByMultiply(
        IDbConnection connection,
        byte language)
    {
        const string sql = @"
            SELECT
                pd.ProductId,
                pd.ProductImageUrl,
                pd.ProductProductCategory,
                pd.ProductIsActive,
                pd.ProductMeasurementUnit,
                pd.ProductDescription,
                pd.ProductName,
                MIN(po.Price * po.OffPercentage) AS PriceMultiOffPercentage
            FROM 
                GetProductDetailsByLanguageFN(@Language) pd 
            INNER JOIN 
                ProductOffer po ON po.Product = pd.ProductId AND po.IsActive = 1 AND po.Quantity > 0
            GROUP BY 
                pd.ProductId,
                pd.ProductImageUrl,
                pd.ProductProductCategory,
                pd.ProductIsActive,
                pd.ProductMeasurementUnit,
                pd.ProductDescription,
                pd.ProductName";

        var parameters = new { Language = language };

        try
        {
            var result = await connection.QueryAsync<BestDealsByMultiplyDto>(sql, parameters);
            return result.AsList();
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception("Error occurred while fetching best deals", ex);
        }
    }
    public static async Task<List<ProductDetail>> GetByIsActiveProductDetailsAsync(IDbConnection connection, int languageId, bool isActive)
    {
        const string sql = @"
        SELECT 
            p.ProductDescription,
            p.ProductId,
            p.ProductImageUrl,
            p.ProductMeasurementUnit,
            p.ProductName,
            p.ProductProductCategory,
            p.ProductIsActive
        FROM GetProductDetailsByLanguageFN(@languageId) p 
        WHERE p.ProductIsActive = @isActive";

        var parameters = new { languageId, isActive };
        var result = await connection.QueryAsync<ProductDetail>(sql, parameters);
        return result.ToList();
    }
    public static async Task<List<ProductDetailWithCategory>> GetByIsActiveProductDetailsWithCategoryAsync
    (IDbConnection connection, int languageId, bool isActive)
    {
        const string sql = @"
            SELECT 
                p.ProductDescription,
                p.ProductId,
                p.ProductImageUrl,
                p.ProductMeasurementUnit,
                p.ProductName,
                p.ProductProductCategory,
                p.ProductIsActive,
                c.ProductCategoryName

            FROM GetProductDetailsByLanguageFN(@languageId) p 
                INNER JOIN GetProductCategoryDetailsByLanguageFN(@languageId) c
            ON c.ProductCategoryId = p.ProductProductCategory
                WHERE p.ProductIsActive = @isActive";

        var parameters = new { languageId, isActive };
        var result = await connection.QueryAsync<ProductDetailWithCategory>(sql, parameters);
        return result.ToList();
    }
    
}

public class ProductDetail
{
    public string ProductDescription { get; set; } = null!;
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; } = null!;
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int ProductProductCategory { get; set; }
    public bool ProductIsActive { get; set; }
}
public class ProductDetailWithCategory
{
    public string ProductDescription { get; set; } = null!;
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; } = null!;
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public bool ProductIsActive { get; set; }
    public int ProductProductCategory { get; set; }
    public string ProductCategoryName { get; set; } = null!;
}
public class BestDealsByOffDto
{
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; } = null!;
    public string ProductProductCategory { get; set; } = null!;
    public bool ProductIsActive { get; set; }
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal OffPercentage { get; set; }
}

public class BestDealsByMultiplyDto
{
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; } = null!;
    public string ProductProductCategory { get; set; } = null!;
    public bool ProductIsActive { get; set; }
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal PriceMultiOffPercentage { get; set; }
}


public class CustomerProductApiResponseWithOffer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MeasurementUnit { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    // Localized properties
    public byte? Language { get; set; }
    public int? Product { get; set; }
    public string? LocalizedMeasurementUnit { get; set; }
    public string? LocalizedDescription { get; set; }
    public string? LocalizedName { get; set; }
    // ProductOffer properties
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}

public class CustomerProductApiResponseWithOfferDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MeasurementUnit { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}