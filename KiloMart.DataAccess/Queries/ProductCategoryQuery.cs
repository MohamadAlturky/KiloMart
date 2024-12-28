using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace KiloMart.Requests.Queries
{
    public partial class Query
    {
        public static async Task<(List<ProductCategoryApiResponseDto> Data, int TotalCount)> GetAllLocalizedPaginated(
            IDbConnection connection,
            byte language,
            int page = 1,
            int pageSize = 10,
            bool isActive = true)
        {
            int offset = (page - 1) * pageSize;
            var countSql = "SELECT COUNT(*) FROM ProductCategory WITH (NOLOCK) WHERE IsActive = @isActive";
            int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { isActive });

            string sql = @"
        SELECT 
            ProductCategory.[Id], 
            ProductCategory.[Name], 
            ProductCategory.[IsActive], 
            ProductCategoryLocalized.[Name] AS LocalizedName, 
            ProductCategoryLocalized.[Language]
        FROM ProductCategory WITH (NOLOCK)
        LEFT JOIN ProductCategoryLocalized WITH (NOLOCK)
            ON ProductCategory.Id = ProductCategoryLocalized.ProductCategory 
            AND ProductCategoryLocalized.Language = @language
            WHERE ProductCategory.IsActive = @isActive
        ORDER BY ProductCategory.[Id]
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

            var categories = await connection.QueryAsync<ProductCategoryApiResponse>(
                sql,
                new { language, offset, pageSize, isActive });

            var result = categories.Select(category => new ProductCategoryApiResponseDto
            {
                Id = category.Id,
                Name = category.LocalizedName ?? category.Name,
                IsActive = category.IsActive,
            }).ToList();

            return (result, totalCount);
        }
    }
    public class ProductCategoryApiResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
    public class ProductCategoryApiResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        // Localized properties
        public string? LocalizedName { get; set; }
        public byte? Language { get; set; }
    }
}
