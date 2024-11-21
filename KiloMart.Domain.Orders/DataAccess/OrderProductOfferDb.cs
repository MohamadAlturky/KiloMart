using Dapper;
using System.Data;

namespace KiloMart.Domain.Orders.DataAccess;

/// <summary>
/// CREATE TABLE[dbo].[OrderProductOffer]
/// (
///     [Id][bigint] IDENTITY(1,1) NOT NULL,
///     [Order] [bigint] NOT NULL,
///     [ProductOffer] [int] NOT NULL,
///     [UnitPrice] [money] NOT NULL,
///     [Quantity] [float] NOT NULL
/// );
/// </summary>
public static partial class OrdersDb
{
    public static async Task<long> InsertOrderProductOfferAsync(IDbConnection connection,
        long orderId,
        int productOfferId,
        decimal unitPrice,
        double quantity,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[OrderProductOffer]
                            ([Order], [ProductOffer], [UnitPrice], [Quantity])
                            VALUES (@Order, @ProductOffer, @UnitPrice, @Quantity)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Order = orderId,
            ProductOffer = productOfferId,
            UnitPrice = unitPrice,
            Quantity = quantity
        }, transaction);
    }

    public static async Task<bool> UpdateOrderProductOfferAsync(IDbConnection connection,
        long id,
        long orderId,
        int productOfferId,
        decimal unitPrice,
        double quantity,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[OrderProductOffer]
                                SET 
                                [Order] = @Order,
                                [ProductOffer] = @ProductOffer,
                                [UnitPrice] = @UnitPrice,
                                [Quantity] = @Quantity
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Order = orderId,
            ProductOffer = productOfferId,
            UnitPrice = unitPrice,
            Quantity = quantity
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteOrderProductOfferAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[OrderProductOffer]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<OrderProductOffer?> GetOrderProductOfferByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Order], 
                            [ProductOffer], 
                            [UnitPrice], 
                            [Quantity]
                            FROM [dbo].[OrderProductOffer]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<OrderProductOffer>(query, new
        {
            Id = id
        });
    }
}

public class OrderProductOffer
{
    public long Id { get; set; }
    public long Order { get; set; }
    public int ProductOffer { get; set; }
    public decimal UnitPrice { get; set; }
    public double Quantity { get; set; }
}