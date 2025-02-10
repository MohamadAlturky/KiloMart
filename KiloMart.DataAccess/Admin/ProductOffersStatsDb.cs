using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Admin;

public static partial class Stats
{

    public static async Task<int> GetActiveProductOffersCount(IDbConnection connection)
    {
        const string query = "SELECT COUNT([Id]) AS TotalCount FROM dbo.[ProductOffer] WHERE IsActive = 1";
        return await connection.ExecuteScalarAsync<int>(query);
    }

    public class ProductOfferProviderDto
    {
        public int Id { get; set; }
        public int Product { get; set; }
        public decimal Price { get; set; }
        public decimal OffPercentage { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public int ProviderId { get; set; }
        public string ProviderDisplayName { get; set; }
        public bool ProviderIsActive { get; set; }
    }

    public static async Task<IEnumerable<ProductOfferProviderDto>> GetActiveProductOffersByProductAsync(
        IDbConnection connection,
        int productId)
    {
        const string sql = @"
        SELECT 
            po.[Id],
            po.[Product],
            po.[Price],
            po.[OffPercentage],
            po.[FromDate],
            po.[ToDate],
            po.[Quantity],
            po.[IsActive],
            p.Id AS ProviderId,
            p.DisplayName AS ProviderDisplayName,
            p.IsActive AS ProviderIsActive
        FROM [dbo].[ProductOffer] po
        INNER JOIN [dbo].[Party] p ON po.[Provider] = p.Id
        WHERE po.IsActive = 1 AND po.[Product] = @productId";

        return await connection.QueryAsync<ProductOfferProviderDto>(sql, new { productId });
    }

    public class ProductDetailsDto
    {
        public int ProductId { get; set; }
        public string ProductImageUrl { get; set; }
        public bool ProductIsActive { get; set; }
        public string ProductMeasurementUnit { get; set; }
        public string ProductDescription { get; set; }
        public string ProductName { get; set; }
        public int ProductCategoryId { get; set; }
        public bool ProductCategoryIsActive { get; set; }
        public string ProductCategoryName { get; set; }
        public int DealId { get; set; }
        public DateTime DealEndDate { get; set; }
        public DateTime DealStartDate { get; set; }
        public bool DealIsActive { get; set; }
        public decimal DealOffPercentage { get; set; }
    }

    public static async Task<IEnumerable<ProductDetailsDto>> GetProductDetailsAsync(
        IDbConnection connection,
        byte languageId,
        int? categoryId,
        bool? isActive,
        int pageSize,
        int page)
    {
        const string sql = @"
        SELECT 
            [p].[Id] AS [ProductId],
            [p].[ImageUrl] AS [ProductImageUrl],
            [p].[IsActive] AS [ProductIsActive],
            COALESCE([pl].[MeasurementUnit], [p].[MeasurementUnit]) AS [ProductMeasurementUnit],
            COALESCE([pl].[Description], [p].[Description]) AS [ProductDescription],
            COALESCE([pl].[Name], [p].[Name]) AS [ProductName],
            [pc].[Id] AS [ProductCategoryId],
            [pc].[IsActive] AS [ProductCategoryIsActive],
            COALESCE([pcl].[Name], [pc].[Name]) AS [ProductCategoryName],
            d.[Id] AS DealId,
            d.[EndDate] AS DealEndDate,
            d.[StartDate] AS DealStartDate,
            d.[IsActive] AS DealIsActive,
            d.[OffPercentage] AS DealOffPercentage
        FROM 
            [dbo].[Product] AS [p]
        INNER JOIN
            [dbo].[ProductCategory] AS [pc] 
                ON [pc].[Id] = [p].[ProductCategory]
        LEFT JOIN 
            [dbo].[ProductLocalized] AS [pl] 
                ON [p].[Id] = [pl].[Product] AND [pl].[Language] = @languageId
        LEFT JOIN
            [dbo].[ProductCategoryLocalized] AS [pcl] 
                ON [pc].[Id] = [pcl].[ProductCategory] AND [pcl].[Language] = @languageId
        LEFT JOIN 
            [dbo].[Deal] AS d 
                ON d.[Product] = p.[Id] 
                AND d.[IsActive] = 1 
                AND d.[StartDate] <= GETDATE() 
                AND d.[EndDate] >= GETDATE()
        WHERE 
            (@categoryId IS NULL OR [pc].[Id] = @categoryId)
            AND (@isActive IS NULL OR [p].[IsActive] = @isActive)
        ORDER BY [p].[Id]
        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY";

        int offset = (page - 1) * pageSize;

        var parameters = new
        {
            languageId,
            categoryId,
            isActive,
            offset,
            pageSize
        };

        return await connection.QueryAsync<ProductDetailsDto>(sql, parameters);
    }
    public static async Task<int> GetProductDetailsCountAsync(
    IDbConnection connection,
    byte languageId,
    int? categoryId,
    bool? isActive)
    {
        const string sql = @"
        SELECT 
            COUNT(*)
        FROM 
            [dbo].[Product] AS [p]
        INNER JOIN
            [dbo].[ProductCategory] AS [pc] 
                ON [pc].[Id] = [p].[ProductCategory]
        LEFT JOIN 
            [dbo].[ProductLocalized] AS [pl] 
                ON [p].[Id] = [pl].[Product] AND [pl].[Language] = @languageId
        LEFT JOIN
            [dbo].[ProductCategoryLocalized] AS [pcl] 
                ON [pc].[Id] = [pcl].[ProductCategory] AND [pcl].[Language] = @languageId
        LEFT JOIN 
            [dbo].[Deal] AS d 
                ON d.[Product] = p.[Id] 
                AND d.[IsActive] = 1 
                AND d.[StartDate] <= GETDATE() 
                AND d.[EndDate] >= GETDATE()
        WHERE 
            (@categoryId IS NULL OR [pc].[Id] = @categoryId)
            AND (@isActive IS NULL OR [p].[IsActive] = @isActive)";

        var parameters = new
        {
            languageId,
            categoryId,
            isActive
        };

        return await connection.ExecuteScalarAsync<int>(sql, parameters);
    }


    public class ProductDetailsWithOffersDto
    {
        public int ProductId { get; set; }
        public string ProductImageUrl { get; set; }
        public bool ProductIsActive { get; set; }
        public string ProductMeasurementUnit { get; set; }
        public string ProductDescription { get; set; }
        public string ProductName { get; set; }
        public int ProductCategoryId { get; set; }
        public bool ProductCategoryIsActive { get; set; }
        public string ProductCategoryName { get; set; }
        public int? DealId { get; set; }
        public DateTime? DealEndDate { get; set; }
        public DateTime? DealStartDate { get; set; }
        public bool? DealIsActive { get; set; }
        public decimal? DealOffPercentage { get; set; }
        public DateTime ProductOfferFromDate { get; set; }
        public long ProductOfferId { get; set; }
        public bool ProductOfferIsActive { get; set; }
        public decimal ProductOfferOffPercentage { get; set; }
        public decimal ProductOfferPrice { get; set; }
        public int ProductOfferProvider { get; set; }
        public int ProductOfferQuantity { get; set; }
        public DateTime ProductOfferToDate { get; set; }
        public string PartyDisplayName { get; set; }
        public bool PartyIsActive { get; set; }
    }

    public static async Task<ProductDetailsWithOffersDto?> GetProductDetailsWithOffersAsync(
        IDbConnection connection,
        long offerId,
        byte languageId)
    {
        const string sql = @"
        SELECT 
            [p].[Id] AS [ProductId],
            [p].[ImageUrl] AS [ProductImageUrl],
            [p].[IsActive] AS [ProductIsActive],
            COALESCE([pl].[MeasurementUnit], [p].[MeasurementUnit]) AS [ProductMeasurementUnit],
            COALESCE([pl].[Description], [p].[Description]) AS [ProductDescription],
            COALESCE([pl].[Name], [p].[Name]) AS [ProductName],
            [pc].[Id] AS [ProductCategoryId],
            [pc].[IsActive] AS [ProductCategoryIsActive],
            COALESCE([pcl].[Name], [pc].[Name]) AS [ProductCategoryName],
            d.[Id] AS DealId,
            d.[EndDate] AS DealEndDate,
            d.[StartDate] AS DealStartDate,
            d.[IsActive] AS DealIsActive,
            d.[OffPercentage] AS DealOffPercentage,
            po.[FromDate] AS ProductOfferFromDate,
            po.[Id] AS ProductOfferId,
            po.[IsActive] AS ProductOfferIsActive,
            po.[OffPercentage] AS ProductOfferOffPercentage,
            po.[Price] AS ProductOfferPrice,
            po.[Provider] AS ProductOfferProvider,
            po.[Quantity] AS ProductOfferQuantity,
            po.[ToDate] AS ProductOfferToDate,
            party.[DisplayName] AS PartyDisplayName,
            party.[IsActive] AS PartyIsActive
        FROM 
            [dbo].[Product] AS [p]
        INNER JOIN
            [dbo].[ProductCategory] AS [pc] 
                ON [pc].[Id] = [p].[ProductCategory]
        LEFT JOIN 
            [dbo].[ProductLocalized] AS [pl] 
                ON [p].[Id] = [pl].[Product] AND [pl].[Language] = @languageId
        LEFT JOIN
            [dbo].[ProductCategoryLocalized] AS [pcl] 
                ON [pc].[Id] = [pcl].[ProductCategory] AND [pcl].[Language] = @languageId
        LEFT JOIN 
            [dbo].[Deal] AS d 
                ON d.[Product] = p.[Id] 
                AND d.[IsActive] = 1 
                AND d.[StartDate] <= GETDATE() 
                AND d.[EndDate] >= GETDATE()
        INNER JOIN 
            dbo.[ProductOffer] po ON po.Product = p.Id
        INNER JOIN 
            dbo.[Party] party ON po.Provider = party.Id
            WHERE po.[Id] = @Offer";

        var parameters = new { languageId, Offer = offerId };

        return await connection.QueryFirstOrDefaultAsync<ProductDetailsWithOffersDto>(sql, parameters);
    }

    public class ProductRequestDto
    {
        public int ProductRequestId { get; set; }
        public int Provider { get; set; }
        public DateTime Date { get; set; }
        public string ImageUrl { get; set; }
        public int ProductCategory { get; set; }
        public decimal Price { get; set; }
        public decimal OffPercentage { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public string PartyName { get; set; }
        public string StatusName { get; set; }
        public string Language { get; set; }
        public string LocalizedName { get; set; }
        public string Description { get; set; }
        public string MeasurementUnit { get; set; }
    }

   public class ProductRequestResult
{
    public IEnumerable<ProductRequestDto> ProductRequests { get; set; }
    public int TotalCount { get; set; }
}

public static async Task<ProductRequestResult> GetProductRequestsAsync(
    IDbConnection connection,
    int pageNumber,
    int pageSize,
    int? providerId = null,
    int? statusId = null)
{
    // Query to fetch paginated data
    const string sql = @"
        SELECT 
            PR.[Id] AS ProductRequestId,
            PR.[Provider],
            PR.[Date],
            PR.[ImageUrl],
            PR.[ProductCategory],
            PR.[Price],
            PR.[OffPercentage],
            PR.[Quantity],
            PR.[Status],
            p.[DisplayName] AS PartyName,
            PRS.[Name] AS StatusName,
            PRDL.[Language],
            PRDL.[Name] AS LocalizedName,
            PRDL.[Description],
            PRDL.[MeasurementUnit]
        FROM 
            [dbo].[ProductRequest] PR
        INNER JOIN 
            [dbo].[ProductRequestStatus] PRS ON PR.[Status] = PRS.[Id]
        INNER JOIN 
            [dbo].[ProductRequestDataLocalized] PRDL ON PR.[Id] = PRDL.[ProductRequest]
        INNER JOIN 
            [dbo].[Party] p ON p.[Id] = PR.[Provider]
        WHERE 
            (@ProviderId IS NULL OR PR.[Provider] = @ProviderId)
            AND (@StatusId IS NULL OR PR.[Status] = @StatusId)
        ORDER BY 
            PR.[Id]
        OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;";

    // Query to fetch total count
    const string countSql = @"
        SELECT 
            COUNT(*)
        FROM 
            [dbo].[ProductRequest] PR
        INNER JOIN 
            [dbo].[ProductRequestStatus] PRS ON PR.[Status] = PRS.[Id]
        INNER JOIN 
            [dbo].[ProductRequestDataLocalized] PRDL ON PR.[Id] = PRDL.[ProductRequest]
        INNER JOIN 
            [dbo].[Party] p ON p.[Id] = PR.[Provider]
        WHERE 
            (@ProviderId IS NULL OR PR.[Provider] = @ProviderId)
            AND (@StatusId IS NULL OR PR.[Status] = @StatusId);";

    var parameters = new
    {
        PageNumber = pageNumber,
        PageSize = pageSize,
        ProviderId = providerId,
        StatusId = statusId
    };

    // Execute both queries
    var productRequests = await connection.QueryAsync<ProductRequestDto>(sql, parameters);
    var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

    return new ProductRequestResult
    {
        ProductRequests = productRequests,
        TotalCount = totalCount
    };
}
}
