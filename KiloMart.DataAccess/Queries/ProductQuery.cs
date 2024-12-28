using System.Data;
using Dapper;

namespace KiloMart.Requests.Queries;

public partial  class Query
{
    public static async Task<(List<ProductApiResponseDto> Data, int TotalCount)> GetProductsPaginated(
    IDbConnection connection,
    byte language = 1,
    int page = 1,
    int pageSize = 10,
    bool isActive = true)
{
    int offset = (page - 1) * pageSize;

    var countSql = "SELECT COUNT(*) FROM Product WITH (NOLOCK) WHERE IsActive = @isActive";
    int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { isActive });

    string sql = @"
        SELECT
            p.[Id]
            , p.[ImageUrl]
            , p.[ProductCategory]
            , p.[IsActive]
            , p.[MeasurementUnit]
            , p.[Description]
            , p.[Name]
            , pl.[Language]
            , pl.[Product]
            , pl.[MeasurementUnit] AS LocalizedMeasurementUnit
            , pl.[Description] AS LocalizedDescription
            , pl.[Name] AS LocalizedName
        FROM Product p WITH (NOLOCK)
        LEFT JOIN ProductLocalized pl WITH (NOLOCK)
            ON p.Id = pl.Product AND pl.Language = @language
        WHERE p.IsActive = @isActive
        ORDER BY p.[Id]
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

    var products = await connection.QueryAsync<ProductApiResponse>(
        sql,
        new { language, offset, pageSize, isActive }
    );

    var result = products.Select(p => new ProductApiResponseDto
    {
        Id = p.Id,
        Name = p.LocalizedName ?? p.Name,
        IsActive = p.IsActive,
        ImageUrl = p.ImageUrl
    }).ToList();

    return (result, totalCount);
}

public static async Task<(List<ProductApiResponseDto> Data, int TotalCount)> GetProductsPaginatedByCategory(
    IDbConnection connection,
    int category,
    byte language = 1,
    int page = 1,
    int pageSize = 10,
    bool isActive = true)
{
    int offset = (page - 1) * pageSize;

    var countSql = "SELECT COUNT(*) FROM Product WITH (NOLOCK) WHERE IsActive = @isActive AND ProductCategory = @category";
    int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { isActive, category });

    string sql = @"
        SELECT
            p.[Id]
            , p.[ImageUrl]
            , p.[ProductCategory]
            , p.[IsActive]
            , p.[MeasurementUnit]
            , p.[Description]
            , p.[Name]
            , pl.[Language]
            , pl.[Product]
            , pl.[MeasurementUnit] AS LocalizedMeasurementUnit
            , pl.[Description] AS LocalizedDescription
            , pl.[Name] AS LocalizedName
        FROM Product p WITH (NOLOCK)
        LEFT JOIN ProductLocalized pl WITH (NOLOCK)
            ON p.Id = pl.Product AND pl.Language = @language
        WHERE p.IsActive = @isActive AND p.ProductCategory = @category
        ORDER BY p.[Id]
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

    var products = await connection.QueryAsync<ProductApiResponse>(
        sql,
        new { language, offset, pageSize, isActive, category }
    );

    var result = products.Select(p => new ProductApiResponseDto
    {
        Id = p.Id,
        Name = p.LocalizedName ?? p.Name,
        IsActive = p.IsActive,
        ImageUrl = p.ImageUrl
    }).ToList();

    return (result, totalCount);
}

public static async Task<(List<ProductApiResponseWithOfferDto> Data, int TotalCount)> GetProductsWithOfferPaginated(
    IDbConnection connection,
    byte language = 1,
    int page = 1,
    int pageSize = 10,
    bool isActive = true)
{
    int offset = (page - 1) * pageSize;

    var countSql = "SELECT COUNT(*) FROM Product WITH (NOLOCK) WHERE IsActive = @isActive";
    int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { isActive });

    string sql = @"
        SELECT
            p.[Id]
            , p.[ImageUrl]
            , p.[ProductCategory]
            , p.[IsActive]
            , p.[MeasurementUnit]
            , p.[Description]
            , p.[Name]
            , pl.[Language]
            , pl.[Product]
            , pl.[MeasurementUnit] AS LocalizedMeasurementUnit
            , pl.[Description] AS LocalizedDescription
            , pl.[Name] AS LocalizedName
            , po.MaxPrice
            , po.MinPrice
        FROM Product p WITH (NOLOCK)
        LEFT JOIN ProductLocalized pl WITH (NOLOCK)
            ON p.Id = pl.Product AND pl.Language = @language
        INNER JOIN (
            SELECT Product, MAX(Price) AS MaxPrice, MIN(Price) AS MinPrice
            FROM ProductOffer
            GROUP BY Product
        ) po ON p.Id = po.Product
        WHERE p.IsActive = @isActive
        ORDER BY p.[Id]
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

    var products = await connection.QueryAsync<ProductApiResponseWithOffer>(
        sql,
        new { language, offset, pageSize, isActive }
    );

    var result = products.Select(p => new ProductApiResponseWithOfferDto
    {
        Id = p.Id,
        Name = p.LocalizedName ?? p.Name,
        IsActive = p.IsActive,
        ImageUrl = p.ImageUrl,
        MinPrice = p.MinPrice,
        MaxPrice = p.MaxPrice
    }).ToList();

    return (result, totalCount);
}

public static async Task<(List<ProductApiResponseWithOfferDto> Data, int TotalCount)> GetProductsWithOfferPaginatedByCategory(
    IDbConnection connection,
    int category,
    byte language = 1,
    int page = 1,
    int pageSize = 10,
    bool isActive = true)
{
    int offset = (page - 1) * pageSize;

    var countSql = "SELECT COUNT(*) FROM Product WITH (NOLOCK) WHERE IsActive = @isActive AND ProductCategory = @category";
    int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { isActive, category });

    string sql = @"
        SELECT
            p.[Id]
            , p.[ImageUrl]
            , p.[ProductCategory]
            , p.[IsActive]
            , p.[MeasurementUnit]
            , p.[Description]
            , p.[Name]
            , pl.[Language]
            , pl.[Product]
            , pl.[MeasurementUnit] AS LocalizedMeasurementUnit
            , pl.[Description] AS LocalizedDescription
            , pl.[Name] AS LocalizedName
            , po.MaxPrice
            , po.MinPrice
        FROM Product p WITH (NOLOCK)
        LEFT JOIN ProductLocalized pl WITH (NOLOCK)
            ON p.Id = pl.Product AND pl.Language = @language
        INNER JOIN (
            SELECT Product, MAX(Price) AS MaxPrice, MIN(Price) AS MinPrice
            FROM ProductOffer
            GROUP BY Product
        ) po ON p.Id = po.Product
        WHERE p.IsActive = @isActive AND ProductCategory = @category
        ORDER BY p.[Id]
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

    var products = await connection.QueryAsync<ProductApiResponseWithOffer>(
        sql,
        new { language, offset, pageSize, isActive, category }
    );

    var result = products.Select(p => new ProductApiResponseWithOfferDto
    {
        Id = p.Id,
        Name = p.LocalizedName ?? p.Name,
        IsActive = p.IsActive,
        ImageUrl = p.ImageUrl,
        MinPrice = p.MinPrice,
        MaxPrice = p.MaxPrice
    }).ToList();

    return (result, totalCount);
}
}

public class ProductApiResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
}

public class ProductApiResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    // Localized properties
    public byte? Language { get; set; }
    public int? Product { get; set; }
    public string? LocalizedMeasurementUnit { get; set; }
    public string? LocalizedDescription { get; set; }
    public string? LocalizedName { get; set; }
}

public class ProductApiResponseWithOffer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    public byte? Language { get; set; }
    public int? Product { get; set; }
    public string? LocalizedMeasurementUnit { get; set; }
    public string? LocalizedDescription { get; set; }
    public string? LocalizedName { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}

public class ProductApiResponseWithOfferDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}



////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////
///////////////////////////////////////////////////////
///////////////////////////////////////////////////////
///////////////////////////////////////////////////////
///////////////////////////////////////////////////////
///////////////////////////////////////////////////////
///////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////
#warning Buggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg 
public partial class Query
{
    public static async Task<(ProductOfferWithDetails[] ProductOffers, int TotalCount)> GetProductOffersPaginated(
        IDbConnection connection,
        int providerId,
        byte languageId,
        int page = 1,
        int pageSize = 10)
    {
        int skip = (page - 1) * pageSize;

        // Query to fetch paginated product offers with details
        var dataQuery = @"
        SELECT 
            -- Columns from ProductOffer
            po.Id AS ProductOfferId,
            po.Product AS ProductOfferProduct,
            po.Price AS ProductOfferPrice,
            po.OffPercentage AS ProductOfferOffPercentage,
            po.FromDate AS ProductOfferFromDate,
            po.ToDate AS ProductOfferToDate,
            po.Quantity AS ProductOfferQuantity,
            po.Provider AS ProductOfferProvider,
            po.IsActive AS ProductOfferIsActive,

            -- Columns from Product
            p.Id AS ProductId,
            p.ImageUrl AS ProductImageUrl,
            p.ProductCategory AS ProductProductCategory,
            p.IsActive AS ProductIsActive,
            p.MeasurementUnit AS ProductMeasurementUnit,
            p.Description AS ProductDescription,
            p.Name AS ProductName,

            -- Columns from ProductLocalized
            pl.Language AS ProductLocalizedLanguage,
            pl.Product AS ProductLocalizedProduct,
            pl.MeasurementUnit AS ProductLocalizedMeasurementUnit,
            pl.Description AS ProductLocalizedDescription,
            pl.Name AS ProductLocalizedName,
            coalesce(pcl.Name,pc.Name) AS ProductCategoryName
        FROM 
            [dbo].[ProductOffer] po
        INNER JOIN 
            [dbo].[Product] p 
            ON p.Id = po.Product
        INNER JOIN 
	        dbo.ProductCategory pc 
	        ON pc.Id = p.ProductCategory
        LEFT JOIN 
            [dbo].[ProductLocalized] pl
            ON pl.Product = p.Id AND pl.[Language] = @languageId
        LEFT JOIN
            dbo.ProductCategoryLocalized pcl
            ON pcl.ProductCategory = pc.Id AND pcl.Language = @languageId
        WHERE 
            po.IsActive = 1 AND po.[Provider] = @providerId
        ORDER BY 
            po.Id
        OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY;";

        // Query to fetch the total count of product offers
        var countQuery = @"
        SELECT COUNT(*)
        FROM 
            [ProductOffer] po
        WHERE 
            po.IsActive = 1 AND po.[Provider] = @providerId;";

        // Execute both queries
        var productOffers = await connection.QueryAsync<ProductOfferWithDetails>(dataQuery, new { providerId, languageId, skip, pageSize });
        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { providerId });

        return (productOffers.ToArray(), totalCount);
    }
}

public class ProductOfferWithDetails
{
    // ProductOffer properties
    public int ProductOfferId { get; set; }
    public int ProductOfferProduct { get; set; }
    public decimal ProductOfferPrice { get; set; }
    public decimal ProductOfferOffPercentage { get; set; }
    public DateTime ProductOfferFromDate { get; set; }
    public DateTime? ProductOfferToDate { get; set; }
    public decimal ProductOfferQuantity { get; set; }
    public int ProductOfferProvider { get; set; }
    public bool ProductOfferIsActive { get; set; }

    // Product properties
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; } = null!;
    public int ProductProductCategory { get; set; }
    public bool ProductIsActive { get; set; }
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public string ProductName { get; set; } = null!;

    // ProductLocalized properties
    public int? ProductLocalizedLanguage { get; set; }
    public int? ProductLocalizedProduct { get; set; }
    public string? ProductLocalizedMeasurementUnit { get; set; }
    public string? ProductLocalizedDescription { get; set; }
    public string? ProductLocalizedName { get; set; }


    // ProductCategoryName
    public string ProductCategoryName { get; set; } = null!;
}
