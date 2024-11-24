using Dapper;
using System.Data;
using System.Data.SqlClient;
namespace KiloMart.Domain.Orders.Queries;



public static partial class OrdersQuery
{


    public async static Task<List<Order>> GetOrdersByStatus(byte orderStatus, IDbConnection connection)
    {
        var sql = @"
            SELECT 
                o.Id, o.OrderStatus, o.TotalPrice, o.TransactionId,
                opi.Id AS ProviderInfoId, opi.Provider, opi.Location AS ProviderLocation,
                oci.Id AS CustomerInfoId, oci.Customer, oci.Location AS CustomerLocation,
                oa.Id AS ActivityId, oa.Date, oa.OrderActivityType, oa.OperatedBy,
                opo.Id AS OrderProductOfferId, opo.ProductOffer, opo.UnitPrice, opo.Quantity AS ProductOfferQuantity,
                op.Id AS OrderProductId, op.Product, op.Quantity AS ProductQuantity
            FROM [Order] o
            LEFT JOIN OrderProviderInformation opi ON o.Id = opi.[Order]
            LEFT JOIN OrderCustomerInformation oci ON o.Id = oci.[Order]
            LEFT JOIN OrderActivity oa ON o.Id = oa.[Order]
            LEFT JOIN OrderProductOffer opo ON o.Id = opo.[Order]
            LEFT JOIN OrderProduct op ON o.Id = op.[Order]
            WHERE o.OrderStatus = @OrderStatus;
        ";

        var orderDictionary = new Dictionary<long, Order>();

        var orders = await connection.QueryAsync<Order, OrderProviderInformation?, OrderCustomerInformation?, OrderActivity?, OrderProductOffer?, OrderProduct?, Order>(
            sql,
            (order, providerInfo, customerInfo, activity, productOffer, product) =>
            {
                if (!orderDictionary.TryGetValue(order.Id, out var currentOrder))
                {
                    currentOrder = order;
                    currentOrder.ProviderInformation = providerInfo;
                    currentOrder.CustomerInformation = customerInfo;
                    currentOrder.Activities = [];
                    currentOrder.ProductOffers = [];
                    currentOrder.Products = [];
                    orderDictionary.Add(currentOrder.Id, currentOrder);
                }

                if (activity is not null && !currentOrder.Activities.Any(a => a.ActivityId == activity.ActivityId))
                {
                    currentOrder.Activities.Add(activity);
                }

                if (productOffer is not null && !currentOrder.ProductOffers.Any(po => po.OrderProductOfferId == productOffer.OrderProductOfferId))
                {
                    currentOrder.ProductOffers.Add(productOffer);
                }

                if (product is not null && !currentOrder.Products.Any(p => p.OrderProductId == product.OrderProductId))
                {
                    currentOrder.Products.Add(product);
                }

                return currentOrder;
            },
            new { OrderStatus = orderStatus },
            splitOn: "ProviderInfoId,CustomerInfoId,ActivityId,OrderProductOfferId,OrderProductId"
        );

        return orders.Distinct().ToList();
    }

    public static async Task<OrderMinPrice?> GetCheapestOrderByCustomerAndStatusAsync(IDbConnection connection, int customerId)
    {
        const string sql = @"
        SELECT Top(1)
            o.Id,
            o.TotalPrice
        FROM [Order] o
        LEFT JOIN OrderCustomerInformation oci ON o.Id = oci.[Order]
        WHERE oci.Customer = @customer
        ORDER BY o.TotalPrice ASC";

        var parameters = new { customer = customerId };

        return await connection.QueryFirstOrDefaultAsync<OrderMinPrice>(sql, parameters);
    }
    public static async Task<List<OrderProductDetail>> GetOrderProductDetailsAsync(IDbConnection connection,
     long orderId, byte language)
    {
        const string sql = @"
        SELECT 
            op.Id AS ItemId,
            op.[Order] AS ItemOrder,
            op.Quantity AS ItemQuantity,
            pd.ProductId,
            pd.ProductImageUrl,
            pd.ProductProductCategory,
            pd.ProductIsActive,
            pd.ProductMeasurementUnit,
            pd.ProductDescription,
            pd.ProductName
        FROM GetProductDetailsByLanguageFN(@language) pd
        INNER JOIN OrderProduct op ON op.Product = pd.ProductId
        WHERE [Order] = @orderId";

        var parameters = new { orderId, language };
        var result = await connection.QueryAsync<OrderProductDetail>(sql, parameters);

        return result.ToList();
    }

}
public class OrderProductDetail
{
    public int ItemId { get; set; }
    public long ItemOrder { get; set; }
    public int ItemQuantity { get; set; }
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; }
    public int ProductProductCategory { get; set; }
    public bool ProductIsActive { get; set; }
    public string ProductMeasurementUnit { get; set; }
    public string ProductDescription { get; set; }
    public string ProductName { get; set; }
}
public class OrderMinPrice
{
    public long Id { get; set; }
    public decimal TotalPrice { get; set; }
}

public class Order
{
    public long Id { get; set; }
    public byte OrderStatus { get; set; }
    public decimal TotalPrice { get; set; }
    public string TransactionId { get; set; } = null!;
    public OrderProviderInformation? ProviderInformation { get; set; }
    public OrderCustomerInformation? CustomerInformation { get; set; }
    public List<OrderActivity> Activities { get; set; } = [];
    public List<OrderProductOffer> ProductOffers { get; set; } = [];
    public List<OrderProduct> Products { get; set; } = [];
}

public class OrderProviderInformation
{
    public long ProviderInfoId { get; set; }
    public int Provider { get; set; }
    public int ProviderLocation { get; set; }
}

public class OrderCustomerInformation
{
    public long CustomerInfoId { get; set; }
    public int Customer { get; set; }
    public int CustomerLocation { get; set; }
}

public class OrderActivity
{
    public long ActivityId { get; set; }
    public DateTime Date { get; set; }
    public byte OrderActivityType { get; set; }
    public int OperatedBy { get; set; }
}

public class OrderProductOffer
{
    public long OrderProductOfferId { get; set; }
    public long ProductOffer { get; set; }
    public decimal UnitPrice { get; set; }
    public float ProductOfferQuantity { get; set; }
}

public class OrderProduct
{
    public int OrderProductId { get; set; }
    public int Product { get; set; }
    public float ProductQuantity { get; set; }
}
