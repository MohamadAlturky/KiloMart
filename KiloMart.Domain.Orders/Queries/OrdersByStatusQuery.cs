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

}

public class Order
{
    public long Id { get; set; }
    public byte OrderStatus { get; set; }
    public decimal TotalPrice { get; set; }
    public string TransactionId { get; set; }
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
