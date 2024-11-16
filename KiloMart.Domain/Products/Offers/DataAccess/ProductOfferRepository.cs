using Dapper;
using KiloMart.Core.Contracts;

namespace KiloMart.Domain.Products.Offers.DataAccess;

public static class ProductOfferRepository
{
    public static async Task InsertProductOffer(ProductOffer model, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var query = @"
        INSERT INTO [dbo].[ProductOffer] ([Product], [Price], [OffPercentage], [FromDate], [ToDate], [Quantity], [Provider], [IsActive])
            VALUES (@Product, @Price, @OffPercentage, @FromDate, @ToDate, @Quantity, @Provider, @IsActive)";
        await connection.ExecuteAsync(query, model);
    }

    public static async Task UpdateProductOffer(ProductOffer model, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var query = @"
        UPDATE [dbo].[ProductOffer]
        SET [Product] = @Product, [Price] = @Price, [OffPercentage] = @OffPercentage, [FromDate] = @FromDate, [ToDate] = @ToDate, [Quantity] = @Quantity, [Provider] = @Provider, [IsActive] = @IsActive
        WHERE [Id] = @Id";
        await connection.ExecuteAsync(query, model);
    }

    public static async Task<ProductOffer?> GetProductOfferById(int id, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var query = "SELECT * FROM [dbo].[ProductOffer] WHERE [Id] = @Id";
        var result = await connection.QueryFirstOrDefaultAsync<ProductOffer>(query, new { Id = id });
        return result;
    }
}


public class ProductOffer
{
    public int Id { get; set; }
    public int Product { get; set; }
    public decimal Price { get; set; }
    public decimal OffPercentage { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public float Quantity { get; set; }
    public int Provider { get; set; }
    public bool IsActive { get; set; }
}
