using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;


public static partial class Db
{
    public static async Task<IEnumerable<OrdersWithAvailableStockModel>> GetOrdersWithAvailableStockAsync(
        IDbConnection connection,
        int providerId,
        byte orderStatus = 1)
    {
        const string query = @"
            SELECT g.[Order] FROM
            (
                SELECT d.[Order], MIN(QuantityDifference) Mininum FROM (
                    SELECT
                        orderProduct.[Order],
                        productOffer.Quantity - orderProduct.Quantity AS QuantityDifference
                    FROM OrderProduct orderProduct
                    INNER JOIN [Order] o ON orderProduct.[Order] = o.Id
                    LEFT JOIN ProductOffer productOffer
                        ON orderProduct.Product = productOffer.Product 
                        AND productOffer.[IsActive] = 1 
                        AND productOffer.[Provider] = @ProviderId
                    WHERE o.OrderStatus = @OrderStatus
                ) d
                GROUP BY d.[Order]
            ) g
            WHERE g.Mininum >= 0";

        return await connection.QueryAsync<OrdersWithAvailableStockModel>(
            query,
            new
            {
                ProviderId = providerId,
                OrderStatus = orderStatus
            });
    }
}

public class OrdersWithAvailableStockModel
{
    public long Order { get; set; }
}