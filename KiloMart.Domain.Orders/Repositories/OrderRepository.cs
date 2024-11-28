using System.Data;
using Dapper;

namespace KiloMart.Domain.Orders.Repositories;

#region Orders Filter
public static partial class OrderRepository
{
    public static async Task<IEnumerable<OrderDetails>> GetOrderDetailsAsync(IDbConnection connection,
     string whereClause,
     object parameters)
    {
        var sql = $@"
            SELECT 
                o.Id,
                o.OrderStatus,
                o.TotalPrice,
                o.TransactionId,
                oci.Customer,
                oci.Location AS CustomerLocation,
                oci.Id AS CustomerInformationId,
                opi.Provider,
                opi.Location AS ProviderLocation,
                opi.Id AS ProviderInformationId,
                odi.Delivery,
                odi.Id AS DeliveryInformationId
            FROM 
                dbo.[Order] o
            LEFT JOIN 
                dbo.[OrderCustomerInformation] oci ON oci.[Order] = o.Id
            LEFT JOIN 
                dbo.[OrderDeliveryInformation] odi ON odi.[Order] = o.Id
            LEFT JOIN 
                dbo.[OrderProviderInformation] opi ON opi.[Order] = o.Id
            {whereClause}
            ORDER BY 
                o.[Id];";

        return await connection.QueryAsync<OrderDetails>(sql, parameters);
    }
}
public class OrderDetails
{
    public long Id { get; set; }
    public byte OrderStatus { get; set; }
    public decimal TotalPrice { get; set; }
    public string TransactionId { get; set; } = null!;

    public int? Customer { get; set; }
    public int? CustomerLocation { get; set; }
    public int? CustomerInformationId { get; set; }

    public int? Provider { get; set; }
    public int? ProviderLocation { get; set; }
    public int? ProviderInformationId { get; set; }

    public int? Delivery { get; set; }
    public int? DeliveryInformationId { get; set; }
}
#endregion


#region Orders Activity
public static partial class OrderRepository
{
    public static async Task<IEnumerable<OrderActivityDetails>> GetOrderActivitiesAsync(IDbConnection connection, int orderId)
    {
        var sql = @"
                SELECT 
                    oa.[Id],
                    oa.[Order],
                    oa.[Date],
                    oa.[OrderActivityType],
                    oat.[Name] AS ActivityTypeName,
                    oa.[OperatedBy], 
                    p.[DisplayName] AS OperatedByDisplayName
                FROM 
                    dbo.[OrderActivity] oa
                INNER JOIN 
                    Party p ON oa.OperatedBy = p.Id
                INNER JOIN 
                    OrderActivityType oat ON oat.Id = oa.OrderActivityType
                WHERE 
                    oa.[Order] = @OrderId;";

        var parameters = new { OrderId = orderId };

        return await connection.QueryAsync<OrderActivityDetails>(sql, parameters);
    }
}

public class OrderActivityDetails
{
    public long Id { get; set; }
    public long Order { get; set; }
    public DateTime Date { get; set; }
    public byte OrderActivityType { get; set; }
    public string ActivityTypeName { get; set; } = null!;
    public int OperatedBy { get; set; }
    public string OperatedByDisplayName { get; set; } = null!;
}
#endregion


#region Orders Products
public class OrderProductDetails
{
    public long Id { get; set; }
    public long Order { get; set; }
    public int Product { get; set; }
    public decimal Quantity { get; set; }
    public string ProductDescription { get; set; } = null!;
    public string ProductImageUrl { get; set; } = null!;
    public int ProductProductCategory { get; set; }
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string ProductCategoryName { get; set; } = null!;
}
public static partial class OrderRepository
{
    public static async Task<IEnumerable<OrderProductDetails>> GetOrderProductsAsync(IDbConnection connection,
    long orderId,
    byte language)
    {
        var sql = @"
                SELECT 
                    op.[Id],
                    op.[Order],
                    op.[Product], 
                    op.[Quantity],
                    pd.ProductDescription,
                    pd.ProductImageUrl,
                    pd.ProductProductCategory,
                    pd.ProductMeasurementUnit,
                    pd.ProductName,
                    cd.ProductCategoryName
                FROM 
                    OrderProduct op
                INNER JOIN 
                    GetProductDetailsByLanguageFN(@language) pd ON op.Product = pd.ProductId
                INNER JOIN 
                    GetProductCategoryDetailsByLanguageFN(@language) cd ON cd.ProductCategoryId = pd.ProductProductCategory
                WHERE 
                    op.[Order] = @OrderId;";

        var parameters = new { OrderId = orderId, language };

        return await connection.QueryAsync<OrderProductDetails>(sql, parameters);
    }
}
#endregion

#region Order Product Offers
public static partial class OrderRepository
{
    public static async Task<IEnumerable<OrderProductOfferDetails>> GetOrderProductOffersAsync(IDbConnection connection,
        long orderId,
        byte language)
    {
        var sql = @"
                SELECT 
                    op.[Id],
                    op.[Order],
                    op.[Product], 
                    op.[Quantity],
                    op.UnitPrice,
                    pd.ProductDescription,
                    pd.ProductImageUrl,
                    pd.ProductProductCategory,
                    pd.ProductMeasurementUnit,
                    pd.ProductName,
                    cd.ProductCategoryName
                FROM 
                    OrderProductOffer op
                INNER JOIN 
                    ProductOffer po ON po.Id = op.ProductOffer 
                INNER JOIN 
                    GetProductDetailsByLanguageFN(@language) pd ON po.Product = pd.ProductId
                INNER JOIN 
                    GetProductCategoryDetailsByLanguageFN(@language) cd ON cd.ProductCategoryId = pd.ProductProductCategory
                WHERE 
                    op.[Order] = @OrderId;";

        var parameters = new { OrderId = orderId, Language = language };

        return await connection.QueryAsync<OrderProductOfferDetails>(sql, parameters);
    }
}
public class OrderProductOfferDetails
{
    public long Id { get; set; }
    public long Order { get; set; }
    public int Product { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string ProductDescription { get; set; } = null!;
    public string ProductImageUrl { get; set; } = null!;
    public int ProductProductCategory { get; set; }
    public string ProductMeasurementUnit { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string ProductCategoryName { get; set; } = null!;
}
#endregion