using System.Data;
using Dapper;

namespace KiloMart.Requests.Queries;

public partial class Query
{
    public static async Task<ProductWithOffersDetailsResponse[]> GetProductDetailsWithOffers(
        IDbConnection connection, 
        int provider,
        byte language,
        int? categoryId = null) // New parameter for filtering by category
    {
        var query = @"
            SELECT 
                pd.[ProductId],
                pd.[ProductImageUrl],
                pd.[ProductIsActive],
                pd.[ProductMeasurementUnit],
                pd.[ProductDescription],
                pd.[ProductName],
                pd.[ProductCategoryId],
                pd.[ProductCategoryIsActive],
                pd.[ProductCategoryName],
                pd.DealId,
                pd.DealEndDate,
                pd.DealStartDate,
                pd.DealIsActive,
                pd.DealOffPercentage,
                po.Id AS ProductOfferId,
                po.FromDate AS ProductOfferFromDate,
                po.IsActive AS ProductOfferIsActive,
                po.OffPercentage AS ProductOfferOffPercentage,
                po.Price AS ProductOfferPrice,
                po.Product AS ProductOfferProduct,
                po.Provider AS ProductOfferProvider,
                po.Quantity AS ProductOfferQuantity,
                po.ToDate AS ProductOfferToDate
            FROM 
                GetProductDetailsFN(@language) pd
            LEFT JOIN 
                dbo.[ProductOffer] po
            ON 
                pd.ProductId = po.Product AND po.Provider = @provider AND po.IsActive = 1
            WHERE 
                (@categoryId IS NULL OR pd.ProductCategoryId = @categoryId);"; // Filter by category

        var productDetails = await connection.QueryAsync<ProductWithOffersDetailsResponse>(
            query,
            new { provider, language, categoryId });

        return productDetails.ToArray();
    }
    public static async Task<ProductWithOffersDetailsResponse[]> GetPaginatedProducts(
        IDbConnection connection, 
        int provider, 
        byte language, 
        int pageNumber, 
        int pageSize, 
        int? categoryId = null)
    {
        var query = @"
            SELECT 
                pd.[ProductId],
                pd.[ProductImageUrl],
                pd.[ProductIsActive],
                pd.[ProductMeasurementUnit],
                pd.[ProductDescription],
                pd.[ProductName],
                pd.[ProductCategoryId],
                pd.[ProductCategoryIsActive],
                pd.[ProductCategoryName],
                pd.DealId,
                pd.DealEndDate,
                pd.DealStartDate,
                pd.DealIsActive,
                pd.DealOffPercentage,
                po.Id AS ProductOfferId,
                po.FromDate AS ProductOfferFromDate,
                po.IsActive AS ProductOfferIsActive,
                po.OffPercentage AS ProductOfferOffPercentage,
                po.Price AS ProductOfferPrice,
                po.Product AS ProductOfferProduct,
                po.Provider AS ProductOfferProvider,
                po.Quantity AS ProductOfferQuantity,
                po.ToDate AS ProductOfferToDate
            FROM 
                GetProductDetailsFN(@language) pd
            LEFT JOIN 
                dbo.[ProductOffer] po ON 
                    pd.ProductId = po.Product AND po.Provider = @provider AND po.IsActive = 1
            WHERE 
                    (@categoryId IS NULL OR pd.ProductCategoryId = @categoryId)
            ORDER BY 
                    pd.ProductId
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (pageNumber - 1) * pageSize;

        var productDetails = await connection.QueryAsync<ProductWithOffersDetailsResponse>(
           query,
           new { provider, language, categoryId, Offset = offset, PageSize = pageSize });

       return productDetails.ToArray();
    }

    public static async Task<long> GetCountPaginatedProducts(
        IDbConnection connection, 
        int provider, 
        byte language, 
        int? categoryId = null)
    {
        var query = @"
            SELECT 
                COUNT(*)
            FROM 
                GetProductDetailsFN(@language) pd
            LEFT JOIN 
                dbo.[ProductOffer] po ON 
                    pd.ProductId = po.Product AND po.Provider = @provider AND po.IsActive = 1
            WHERE 
                (@categoryId IS NULL OR pd.ProductCategoryId = @categoryId);";


        var count = await connection.QueryFirstOrDefaultAsync<long>(
           query,
           new { provider, language, categoryId});

       return count;
    }
    




    //////////////////////////////////////////////
    //////////////////////////////////////////////
    //////////////////////////////////////////////
    //////////////////////////////////////////////
    //////////////////////////////////////////////
    //////////////////////////////////////////////
    //////////////////////////////////////////////
    //////////////////////////////////////////////
    ///



    public static async Task<ProductWithOffersDetailsResponse[]> GetProviderProductDetailsWithOffers(
        IDbConnection connection, 
        int provider,
        byte language,
        int? categoryId = null) // New parameter for filtering by category
    {
        var query = @"
            SELECT 
                pd.[ProductId],
                pd.[ProductImageUrl],
                pd.[ProductIsActive],
                pd.[ProductMeasurementUnit],
                pd.[ProductDescription],
                pd.[ProductName],
                pd.[ProductCategoryId],
                pd.[ProductCategoryIsActive],
                pd.[ProductCategoryName],
                pd.DealId,
                pd.DealEndDate,
                pd.DealStartDate,
                pd.DealIsActive,
                pd.DealOffPercentage,
                po.Id AS ProductOfferId,
                po.FromDate AS ProductOfferFromDate,
                po.IsActive AS ProductOfferIsActive,
                po.OffPercentage AS ProductOfferOffPercentage,
                po.Price AS ProductOfferPrice,
                po.Product AS ProductOfferProduct,
                po.Provider AS ProductOfferProvider,
                po.Quantity AS ProductOfferQuantity,
                po.ToDate AS ProductOfferToDate
            FROM 
                GetProductDetailsFN(@language) pd
            INNER JOIN 
                dbo.[ProductOffer] po
            ON 
                pd.ProductId = po.Product AND po.Provider = @provider AND po.IsActive = 1
            WHERE 
                (@categoryId IS NULL OR pd.ProductCategoryId = @categoryId);"; // Filter by category

        var productDetails = await connection.QueryAsync<ProductWithOffersDetailsResponse>(
            query,
            new { provider, language, categoryId });

        return productDetails.ToArray();
    }

    public static async Task<ProductWithOffersDetailsResponse[]> GetProviderPaginatedProducts(
        IDbConnection connection, 
        int provider, 
        byte language, 
        int pageNumber, 
        int pageSize, 
        int? categoryId = null)
    {
        var query = @"
            SELECT 
                pd.[ProductId],
                pd.[ProductImageUrl],
                pd.[ProductIsActive],
                pd.[ProductMeasurementUnit],
                pd.[ProductDescription],
                pd.[ProductName],
                pd.[ProductCategoryId],
                pd.[ProductCategoryIsActive],
                pd.[ProductCategoryName],
                pd.DealId,
                pd.DealEndDate,
                pd.DealStartDate,
                pd.DealIsActive,
                pd.DealOffPercentage,
                po.Id AS ProductOfferId,
                po.FromDate AS ProductOfferFromDate,
                po.IsActive AS ProductOfferIsActive,
                po.OffPercentage AS ProductOfferOffPercentage,
                po.Price AS ProductOfferPrice,
                po.Product AS ProductOfferProduct,
                po.Provider AS ProductOfferProvider,
                po.Quantity AS ProductOfferQuantity,
                po.ToDate AS ProductOfferToDate
            FROM 
               GetProductDetailsFN(@language) pd
            INNER JOIN 
               dbo.[ProductOffer] po ON 
                   pd.ProductId = po.Product AND po.Provider = @provider AND po.IsActive = 1
            WHERE 
                    (@categoryId IS NULL OR pd.ProductCategoryId = @categoryId)
            ORDER BY 
                    pd.ProductId
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var offset = (pageNumber - 1) * pageSize;

        var productDetails = await connection.QueryAsync<ProductWithOffersDetailsResponse>(
           query,
           new { provider, language, categoryId, Offset = offset, PageSize = pageSize });

       return productDetails.ToArray();
    }

    public static async Task<long> GetCountProviderPaginatedProducts(
        IDbConnection connection, 
        int provider, 
        byte language, 
        int? categoryId = null)
    {
        var query = @"
            SELECT 
                COUNT(*)
            FROM 
               GetProductDetailsFN(@language) pd
            INNER JOIN 
               dbo.[ProductOffer] po ON 
                   pd.ProductId = po.Product AND po.Provider = @provider AND po.IsActive = 1
            WHERE 
               (@categoryId IS NULL OR pd.ProductCategoryId = @categoryId);";

        var count = await connection.QueryFirstOrDefaultAsync<long>(
           query,
           new { provider, language, categoryId });

       return count;
    }
}

public class ProductWithOffersDetailsResponse
{
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; } = null!;
    public bool ProductIsActive { get; set; }
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public int ProductCategoryId { get; set; }
    public bool ProductCategoryIsActive { get; set; }
    public string ProductCategoryName { get; set; } = null!;
    public int? DealId { get; set; }
    public DateTime? DealEndDate { get; set; }
    public DateTime? DealStartDate { get; set; }
    public bool DealIsActive { get; set; }
    public decimal? DealOffPercentage { get; set; }

    // Offer details
    public int? ProductOfferId { get; set; }
    public DateTime? ProductOfferFromDate { get; set; }
    public bool? ProductOfferIsActive { get; set; }
    public decimal? ProductOfferOffPercentage { get; set; }
    public decimal? ProductOfferPrice { get; set; }
    public int? ProductOfferProduct { get; set; }
    public int? ProductOfferProvider { get; set; }
    public int? ProductOfferQuantity { get; set; }
    public DateTime? ProductOfferToDate { get; set; }
}
