using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace KiloMart.Requests.Queries
{
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

}
