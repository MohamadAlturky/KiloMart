using Dapper;
using KiloMart.Core.Contracts;

namespace KiloMart.Requests.Queries;

public partial class Query
{
    public static async Task<List<ProductOfferDetails>> GetProductOffersByProvider(IDbFactory factory, long providerId, byte language)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();

        const string query = @"
        SELECT 
            pd.ProductId AS ProductId,
            pd.ProductImageUrl AS ProductImageUrl,
            pd.ProductProductCategory AS ProductCategory,
            pd.ProductIsActive AS ProductIsActive,
            pd.ProductMeasurementUnit AS ProductMeasurementUnit,
            pd.ProductName AS ProductName,
            pd.ProductDescription AS ProductDescription,
            po.Id AS OfferId,
            po.Price AS OfferPrice,
            po.OffPercentage AS OfferOffPercentage,
            po.FromDate AS OfferFromDate,
            po.Quantity AS OfferQuantity,
            po.Provider AS OfferProvider,
            po.IsActive AS OfferIsActive,
            po.ToDate AS OfferToDate,
		    pc.ProductCategoryName
        FROM dbo.GetProductDetailsByLanguageFN(@language) pd
        INNER JOIN ProductOffer po ON pd.ProductId = po.Product AND po.IsActive = 1
        INNER JOIN GetProductCategoryDetailsByLanguageFN(@language) pc ON pc.ProductCategoryId = pd.ProductProductCategory 
        WHERE po.Provider = @provider
        ORDER BY po.Id DESC";

        var result = await connection.QueryAsync<ProductOfferDetails>(query, new { provider = providerId, language });

        return result.ToList();
    }
    public static async Task<List<ProductOfferDetails>> GetProductOffersByProviderAndCategory(IDbFactory factory,
    long providerId,
     byte language,
     int category)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();

        const string query = @"
        SELECT 
            pd.ProductId AS ProductId,
            pd.ProductImageUrl AS ProductImageUrl,
            pd.ProductProductCategory AS ProductCategory,
            pd.ProductIsActive AS ProductIsActive,
            pd.ProductMeasurementUnit AS ProductMeasurementUnit,
            pd.ProductName AS ProductName,
            pd.ProductDescription AS ProductDescription,
            po.Id AS OfferId,
            po.Price AS OfferPrice,
            po.OffPercentage AS OfferOffPercentage,
            po.FromDate AS OfferFromDate,
            po.Quantity AS OfferQuantity,
            po.Provider AS OfferProvider,
            po.IsActive AS OfferIsActive,
            po.ToDate AS OfferToDate
        FROM dbo.GetProductDetailsByLanguageFN(@language) pd
        INNER JOIN ProductOffer po ON pd.ProductId = po.Product AND po.IsActive = 1  AND pd.ProductProductCategory = @category
        WHERE po.Provider = @provider
        ORDER BY po.Id DESC";

        var result = await connection.QueryAsync<ProductOfferDetails>(query, new { provider = providerId, language,category });

        return result.ToList();
    }

    public static async Task<(List<ProductOfferDetails> Offers, int TotalCount)> GetProductOffersByProvider(IDbFactory factory, long providerId, int pageNumber, int pageSize, byte language)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();

        const string query = @"
        SELECT 
            pd.ProductId AS ProductId,
            pd.ProductImageUrl AS ProductImageUrl,
            pd.ProductProductCategory AS ProductCategory,
            pd.ProductIsActive AS ProductIsActive,
            pd.ProductMeasurementUnit AS ProductMeasurementUnit,
            pd.ProductName AS ProductName,
            pd.ProductDescription AS ProductDescription,
            po.Id AS OfferId,
            po.Price AS OfferPrice,
            po.OffPercentage AS OfferOffPercentage,
            po.FromDate AS OfferFromDate,
            po.Quantity AS OfferQuantity,
            po.Provider AS OfferProvider,
            po.IsActive AS OfferIsActive,
            po.ToDate AS OfferToDate,
		    pc.ProductCategoryName
        FROM dbo.GetProductDetailsByLanguageFN(@language) pd
        INNER JOIN ProductOffer po ON pd.ProductId = po.Product AND po.IsActive = 1
        INNER JOIN GetProductCategoryDetailsByLanguageFN(@language) pc ON pc.ProductCategoryId = pd.ProductProductCategory 
        WHERE po.Provider = @provider
        ORDER BY po.Id DESC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(*) FROM ProductOffer WHERE Provider = @provider AND IsActive = 1;";

        // Calculate the offset based on the current page and page size
        var offset = (pageNumber - 1) * pageSize;

        using var multi = await connection.QueryMultipleAsync(query, new { provider = providerId, Offset = offset, PageSize = pageSize, language });

        var offers = multi.Read<ProductOfferDetails>().ToList();
        var totalCount = multi.ReadSingle<int>();

        return (offers, totalCount);
    }
}

public class ProductOfferDetails
{
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; } = null!;
    public int ProductCategory { get; set; }
    public string ProductCategoryName { get; set; } = null!;
    public bool ProductIsActive { get; set; }
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public int OfferId { get; set; }
    public decimal OfferPrice { get; set; }
    public decimal OfferOffPercentage { get; set; }
    public DateTime OfferFromDate { get; set; }
    public DateTime OfferToDate { get; set; }
    public int OfferQuantity { get; set; }
    public int OfferProvider { get; set; }
    public bool OfferIsActive { get; set; }
}