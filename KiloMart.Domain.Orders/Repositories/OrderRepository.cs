using System.Data;
using Dapper;
using KiloMart.DataAccess.Database;

namespace KiloMart.Domain.Orders.Repositories;

#region Orders Filter
public static partial class OrderRepository
{
    public static async Task<IEnumerable<OrderDetailsDto>> GetOrderDetailsForProviderAsync(IDbConnection connection,
     string whereClause,
     object parameters)
    {
        var sql = $@"
            SELECT 
                o.Id,
                o.OrderStatus,
                o.TotalPrice,
                o.TransactionId,
                o.Date,
                o.PaymentType,
                o.IsPaid,
                o.ItemsPrice,
                o.SystemFee,
                o.DeliveryFee,
                o.SpecialRequest,
                oci.Customer,
                oci.Location AS CustomerLocation,
                oci.Id AS CustomerInformationId,
                opi.Provider,
                opi.Location AS ProviderLocation,
                opi.Id AS ProviderInformationId,
                odi.Delivery,
                odi.Id AS DeliveryInformationId,

                cl.[Name] AS CustomerLocationName,
                cl.[Latitude] AS CustomerLocationLatitude,
                cl.[Longitude] AS CustomerLocationLongitude,
                pl.[Name] AS ProviderLocationName,
                pl.[Latitude] AS ProviderLocationLatitude,
                pl.[Longitude] AS ProviderLocationLongitude,
                               
                mc.DisplayName CustomerDisplayName,
				mp.DisplayName ProviderDisplayName,
				md.DisplayName DeliveryDisplayName
            FROM 
                dbo.[Order] o
            LEFT JOIN 
                dbo.[OrderCustomerInformation] oci ON oci.[Order] = o.Id
            LEFT JOIN 
                dbo.[OrderDeliveryInformation] odi ON odi.[Order] = o.Id
            LEFT JOIN 
                dbo.[OrderProviderInformation] opi ON opi.[Order] = o.Id
			LEFT JOIN dbo.Party mc ON oci.Customer = mc.Id
			LEFT JOIN dbo.Party mp ON opi.Provider = mp.Id
			LEFT JOIN dbo.Party md ON odi.Delivery = md.Id
            LEFT JOIN 
                dbo.[Location] cl ON cl.Id = oci.[Location]
            LEFT JOIN 
                dbo.[Location] pl ON pl.Id = opi.[Location]

            {whereClause}
            ORDER BY 
                o.[Id];";

        return await connection.QueryAsync<OrderDetailsDto>(sql, parameters);
    }

    public static async Task<IEnumerable<OrderDetailsDto>> GetOrderDetailsAsync(IDbConnection connection,
     string whereClause,
     object parameters)
    {
        var sql = $@"
            SELECT 
                o.Id,
                o.OrderStatus,
                o.TotalPrice,
                o.TransactionId,
                o.Date,
                o.PaymentType,
                o.IsPaid,
                o.ItemsPrice,
                o.SystemFee,
                o.DeliveryFee,
                o.SpecialRequest,
                oci.Customer,
                oci.Location AS CustomerLocation,
                oci.Id AS CustomerInformationId,
                opi.Provider,
                opi.Location AS ProviderLocation,
                opi.Id AS ProviderInformationId,
                odi.Delivery,
                odi.Id AS DeliveryInformationId,

                cl.[Name] AS CustomerLocationName,
                cl.[Latitude] AS CustomerLocationLatitude,
                cl.[Longitude] AS CustomerLocationLongitude,
                pl.[Name] AS ProviderLocationName,
                pl.[Latitude] AS ProviderLocationLatitude,
                pl.[Longitude] AS ProviderLocationLongitude,
                               
                mc.DisplayName CustomerDisplayName,
				mp.DisplayName ProviderDisplayName,
				md.DisplayName DeliveryDisplayName
            FROM 
                dbo.[Order] o
            LEFT JOIN 
                dbo.[OrderCustomerInformation] oci ON oci.[Order] = o.Id
            LEFT JOIN 
                dbo.[OrderDeliveryInformation] odi ON odi.[Order] = o.Id
            LEFT JOIN 
                dbo.[OrderProviderInformation] opi ON opi.[Order] = o.Id
			LEFT JOIN dbo.Party mc ON oci.Customer = mc.Id
			LEFT JOIN dbo.Party mp ON opi.Provider = mp.Id
			LEFT JOIN dbo.Party md ON odi.Delivery = md.Id
            LEFT JOIN 
                dbo.[Location] cl ON cl.Id = oci.[Location]
            LEFT JOIN 
                dbo.[Location] pl ON pl.Id = opi.[Location]

            {whereClause}
            ORDER BY 
                o.[Id];";

        return await connection.QueryAsync<OrderDetailsDto>(sql, parameters);
    }

    public static async Task<OrderDetailsDto?> GetOrderDetailsFirstOrDefaultAsync(IDbConnection connection,
     string whereClause,
     object parameters)
    {
        var sql = $@"
            SELECT 
                o.Id,
                o.OrderStatus,
                o.TotalPrice,
                o.TransactionId,
                o.Date,
                o.PaymentType,
                o.IsPaid,
                o.ItemsPrice,
                o.SystemFee,
                o.DeliveryFee,
                o.SpecialRequest,
                oci.Customer,
                oci.Location AS CustomerLocation,
                oci.Id AS CustomerInformationId,
                opi.Provider,
                opi.Location AS ProviderLocation,
                opi.Id AS ProviderInformationId,
                odi.Delivery,
                odi.Id AS DeliveryInformationId,

                cl.[Name] AS CustomerLocationName,
                cl.[Latitude] AS CustomerLocationLatitude,
                cl.[Longitude] AS CustomerLocationLongitude,
                pl.[Name] AS ProviderLocationName,
                pl.[Latitude] AS ProviderLocationLatitude,
                pl.[Longitude] AS ProviderLocationLongitude,

                mc.DisplayName CustomerDisplayName,
				mp.DisplayName ProviderDisplayName,
				md.DisplayName DeliveryDisplayName
            FROM 
                dbo.[Order] o
            LEFT JOIN 
                dbo.[OrderCustomerInformation] oci ON oci.[Order] = o.Id
            LEFT JOIN 
                dbo.[OrderDeliveryInformation] odi ON odi.[Order] = o.Id
            LEFT JOIN 
                dbo.[OrderProviderInformation] opi ON opi.[Order] = o.Id
			LEFT JOIN dbo.Party mc ON oci.Customer = mc.Id
			LEFT JOIN dbo.Party mp ON opi.Provider = mp.Id
			LEFT JOIN dbo.Party md ON odi.Delivery = md.Id
            LEFT JOIN 
                dbo.[Location] cl ON cl.Id = oci.[Location]
            LEFT JOIN 
                dbo.[Location] pl ON pl.Id = opi.[Location]

            {whereClause}
            ORDER BY 
                o.[Id];";

        return await connection.QueryFirstOrDefaultAsync<OrderDetailsDto>(sql, parameters);
    }
    public static async Task<IEnumerable<OrderDetailsDto>> GetOrderDetailsByProductIdAsync(IDbConnection connection, int productId)
    {
        var sql = @"
        SELECT 
            o.Id,
            o.OrderStatus,
            o.TotalPrice,
            o.TransactionId,
            o.Date,
            o.PaymentType,
            o.IsPaid,
            o.ItemsPrice,
            o.SystemFee,
            o.DeliveryFee,
            o.SpecialRequest,
            oci.Customer,
            oci.Location AS CustomerLocation,
            oci.Id AS CustomerInformationId,
            opi.Provider,
            opi.Location AS ProviderLocation,
            opi.Id AS ProviderInformationId,
            odi.Delivery,
            odi.Id AS DeliveryInformationId,

            cl.[Name] AS CustomerLocationName,
            cl.[Latitude] AS CustomerLocationLatitude,
            cl.[Longitude] AS CustomerLocationLongitude,
            pl.[Name] AS ProviderLocationName,
            pl.[Latitude] AS ProviderLocationLatitude,
            pl.[Longitude] AS ProviderLocationLongitude,

            mc.DisplayName CustomerDisplayName,
            mp.DisplayName ProviderDisplayName,
            md.DisplayName DeliveryDisplayName
        FROM 
            dbo.[Order] o
        LEFT JOIN 
            dbo.[OrderCustomerInformation] oci ON oci.[Order] = o.Id
        LEFT JOIN 
            dbo.[OrderDeliveryInformation] odi ON odi.[Order] = o.Id
        LEFT JOIN 
            dbo.[OrderProviderInformation] opi ON opi.[Order] = o.Id
        LEFT JOIN 
            dbo.Party mc ON oci.Customer = mc.Id
        LEFT JOIN 
            dbo.Party mp ON opi.Provider = mp.Id
        LEFT JOIN 
            dbo.Party md ON odi.Delivery = md.Id
        LEFT JOIN 
            dbo.[Location] cl ON cl.Id = oci.[Location]
        LEFT JOIN 
            dbo.[Location] pl ON pl.Id = opi.[Location]
        INNER JOIN 
            dbo.[OrderProduct] op ON o.Id = op.[Order]
        WHERE 
            op.Product = @productId
        GROUP BY
            o.Id,
            o.OrderStatus,
            o.TotalPrice,
            o.TransactionId,
            o.Date,
            o.PaymentType,
            o.IsPaid,
            o.ItemsPrice,
            o.SystemFee,
            o.DeliveryFee,
            o.SpecialRequest,
            oci.Customer,
            oci.Location,
            oci.Id,
            opi.Provider,
            opi.Location,
            opi.Id,
            odi.Delivery,
            odi.Id,
            cl.[Name],
            cl.[Latitude],
            cl.[Longitude],
            pl.[Name],
            pl.[Latitude],
            pl.[Longitude],
            mc.DisplayName,
            mp.DisplayName,
            md.DisplayName
        ORDER BY 
            o.[Id];";

        return await connection.QueryAsync<OrderDetailsDto>(sql, new { productId });
    }


    public static async Task<IEnumerable<OrderDetailsDto>> GetOrderDetailsByProductOfferIdAsync(IDbConnection connection, long productId)
    {
        var sql = @"
        SELECT 
            o.Id,
            o.OrderStatus,
            o.TotalPrice,
            o.TransactionId,
            o.Date,
            o.PaymentType,
            o.IsPaid,
            o.ItemsPrice,
            o.SystemFee,
            o.DeliveryFee,
            o.SpecialRequest,
            oci.Customer,
            oci.Location AS CustomerLocation,
            oci.Id AS CustomerInformationId,
            opi.Provider,
            opi.Location AS ProviderLocation,
            opi.Id AS ProviderInformationId,
            odi.Delivery,
            odi.Id AS DeliveryInformationId,

            cl.[Name] AS CustomerLocationName,
            cl.[Latitude] AS CustomerLocationLatitude,
            cl.[Longitude] AS CustomerLocationLongitude,
            pl.[Name] AS ProviderLocationName,
            pl.[Latitude] AS ProviderLocationLatitude,
            pl.[Longitude] AS ProviderLocationLongitude,

            mc.DisplayName CustomerDisplayName,
            mp.DisplayName ProviderDisplayName,
            md.DisplayName DeliveryDisplayName
        FROM 
            dbo.[Order] o
        LEFT JOIN 
            dbo.[OrderCustomerInformation] oci ON oci.[Order] = o.Id
        LEFT JOIN 
            dbo.[OrderDeliveryInformation] odi ON odi.[Order] = o.Id
        LEFT JOIN 
            dbo.[OrderProviderInformation] opi ON opi.[Order] = o.Id
        LEFT JOIN 
            dbo.Party mc ON oci.Customer = mc.Id
        LEFT JOIN 
            dbo.Party mp ON opi.Provider = mp.Id
        LEFT JOIN 
            dbo.Party md ON odi.Delivery = md.Id
        LEFT JOIN 
            dbo.[Location] cl ON cl.Id = oci.[Location]
        LEFT JOIN 
            dbo.[Location] pl ON pl.Id = opi.[Location]
        INNER JOIN 
            dbo.[OrderProductOffer] op ON o.Id = op.[Order]
        WHERE 
            op.ProductOffer = @productId
        GROUP BY
            o.Id,
            o.OrderStatus,
            o.TotalPrice,
            o.TransactionId,
            o.Date,
            o.PaymentType,
            o.IsPaid,
            o.ItemsPrice,
            o.SystemFee,
            o.DeliveryFee,
            o.SpecialRequest,
            oci.Customer,
            oci.Location,
            oci.Id,
            opi.Provider,
            opi.Location,
            opi.Id,
            odi.Delivery,
            odi.Id,
            cl.[Name],
            cl.[Latitude],
            cl.[Longitude],
            pl.[Name],
            pl.[Latitude],
            pl.[Longitude],
            mc.DisplayName,
            mp.DisplayName,
            md.DisplayName
        ORDER BY 
            o.[Id];";

        return await connection.QueryAsync<OrderDetailsDto>(sql, new { productId });
    }
}
public class OrderDetailsDto
{
    public long Id { get; set; }
    public byte OrderStatus { get; set; }
    public decimal TotalPrice { get; set; }
    public string TransactionId { get; set; } = null!;
    public DateTime Date { get; set; }
    public byte PaymentType { get; set; }
    public bool IsPaid { get; set; }
    public decimal ItemsPrice { get; set; }
    public decimal SystemFee { get; set; }
    public decimal DeliveryFee { get; set; }
    public string? SpecialRequest { get; set; }

    public int? Customer { get; set; }
    public int? CustomerLocation { get; set; }
    public int? CustomerInformationId { get; set; }

    public int? Provider { get; set; }
    public int? ProviderLocation { get; set; }
    public int? ProviderInformationId { get; set; }

    public int? Delivery { get; set; }
    public int? DeliveryInformationId { get; set; }

    public string? CustomerLocationName { get; set; }
    public decimal? CustomerLocationLatitude { get; set; }
    public decimal? CustomerLocationLongitude { get; set; }
    public string? ProviderLocationName { get; set; }
    public decimal? ProviderLocationLatitude { get; set; }
    public decimal? ProviderLocationLongitude { get; set; }


    public string? CustomerDisplayName { get; set; }
    public string? ProviderDisplayName { get; set; }
    public string? DeliveryDisplayName { get; set; }
}
#endregion

#region Orders Activity
public static partial class OrderRepository
{
    public static async Task<IEnumerable<OrderActivityDetailsDto>> GetOrderActivitiesAsync(IDbConnection connection, long orderId)
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

        return await connection.QueryAsync<OrderActivityDetailsDto>(sql, parameters);
    }
}

public class OrderActivityDetailsDto
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
public class OrderProductDetailsDto
{
    public int ItemId { get; set; }
    public long ItemOrder { get; set; }
    public int ItemQuantity { get; set; }

    public decimal? ActualUnitPrice { get; set; } = null;
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
    public bool? DealIsActive { get; set; }
    public decimal? DealOffPercentage { get; set; }
}
public static partial class OrderRepository
{
    public static async Task<IEnumerable<OrderProductDetailsDto>> GetOrderProductsAsync(IDbConnection connection,
    long orderId,
    byte language)
    {
        var sql = @"
                SELECT 
                op.Id AS ItemId,
                op.[Order] AS ItemOrder,
                op.Quantity AS ItemQuantity,
                [ProductId],
                [ProductImageUrl],
                [ProductIsActive],
                [ProductMeasurementUnit],
                [ProductDescription],
                [ProductName],
                [ProductCategoryId],
                [ProductCategoryIsActive],
                [ProductCategoryName],
                [DealId],
                [DealEndDate],
                [DealStartDate],
                [DealIsActive],
                [DealOffPercentage]
            FROM dbo.GetProductDetailsFN(@Language) pd
            INNER JOIN OrderProduct op ON op.Product = pd.ProductId
            WHERE [Order] = @OrderId";

        var parameters = new { OrderId = orderId, Language = language };

        return await connection.QueryAsync<OrderProductDetailsDto>(sql, parameters);
    }
}
#endregion

#region Orders Products IN Clause
public static partial class OrderRepository
{
    public static async Task<IEnumerable<OrderProductDetailsDto>> GetOrderProductsByIdsAsync(IDbConnection connection,
        IEnumerable<long> orderIds,
        byte language)
    {
        var sql = @"
                SELECT 
                    op.Id AS ItemId,
                    op.[Order] AS ItemOrder,
                    op.Quantity AS ItemQuantity,
                    [ProductId],
                    [ProductImageUrl],
                    [ProductIsActive],
                    [ProductMeasurementUnit],
                    [ProductDescription],
                    [ProductName],
                    [ProductCategoryId],
                    [ProductCategoryIsActive],
                    [ProductCategoryName],
                    [DealId],
                    [DealEndDate],
                    [DealStartDate],
                    [DealIsActive],
                    [DealOffPercentage]
            FROM dbo.GetProductDetailsFN(@Language) pd
            INNER JOIN OrderProduct op ON op.Product = pd.ProductId
                WHERE 
                    op.[Order] IN @OrderIds;";

        var parameters = new DynamicParameters();
        parameters.Add("Language", language);
        parameters.Add("OrderIds", orderIds.ToArray());

        return await connection.QueryAsync<OrderProductDetailsDto>(sql, parameters);
    }
}
#endregion

#region Order Product Offers
public static partial class OrderRepository
{
    public static async Task<IEnumerable<OrderProductOfferDetailsDto>> GetOrderProductOffersAsync(IDbConnection connection,
        long orderId,
        byte language)
    {
        var sql = @"
                SELECT 
                op.[Id],
                op.[Order],
                op.[ProductOffer], 
                op.[Quantity],
                op.UnitPrice,
                [ProductId],
                [ProductImageUrl],
                [ProductIsActive],
                [ProductMeasurementUnit],
                [ProductDescription],
                [ProductName],
                [ProductCategoryId],
                [ProductCategoryIsActive],
                [ProductCategoryName],
                [DealId],
                [DealEndDate],
                [DealStartDate],
                [DealIsActive],
                [DealOffPercentage]
            FROM 
                OrderProductOffer op
            INNER JOIN 
                ProductOffer po ON po.Id = op.ProductOffer 
            INNER JOIN 
                dbo.GetProductDetailsFN(@language) pd ON po.Product = pd.ProductId
            WHERE 
                op.[Order] = @OrderId;";
        // var sql = @"
        // SELECT 
        // op.[Id],
        // op.[Order],
        // op.[ProductOffer], 
        // op.[Quantity],
        // op.UnitPrice,
        // pd.ProductDescription,
        // pd.ProductImageUrl,
        // pd.ProductProductCategory,
        // pd.ProductMeasurementUnit,
        // pd.ProductName,
        // cd.ProductCategoryName
        // FROM 
        // OrderProductOffer op
        // INNER JOIN 
        // ProductOffer po ON po.Id = op.ProductOffer 
        // INNER JOIN 
        // GetProductDetailsByLanguageFN(@language) pd ON po.Product = pd.ProductId
        // INNER JOIN 
        // GetProductCategoryDetailsByLanguageFN(@language) cd ON cd.ProductCategoryId = pd.ProductProductCategory
        // WHERE 
        // op.[Order] = @OrderId;";

        var parameters = new { OrderId = orderId, Language = language };

        return await connection.QueryAsync<OrderProductOfferDetailsDto>(sql, parameters);
    }
}
public class OrderProductOfferDetailsDto
{
    public long Id { get; set; }
    public long Order { get; set; }
    public int ProductOffer { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    // public string ProductDescription { get; set; } = null!;
    // public string ProductImageUrl { get; set; } = null!;
    // public int ProductProductCategory { get; set; }
    // public string ProductMeasurementUnit { get; set; } = null!;
    // public string ProductName { get; set; } = null!;
    // public string ProductCategoryName { get; set; } = null!;
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
    public bool? DealIsActive { get; set; }
    public decimal? DealOffPercentage { get; set; }
}
#endregion

#region Order Product Offer In Clause
public static partial class OrderRepository
{
    public static async Task<IEnumerable<OrderProductOfferDetailsDto>> GetOrderProductOffersByIdsAsync(IDbConnection connection,
        IEnumerable<long> orderIds,
        int language)
    {
        var sql = @"
                SELECT 
                op.[Id],
                op.[Order],
                op.[ProductOffer], 
                op.[Quantity],
                op.UnitPrice,
                [ProductId],
                [ProductImageUrl],
                [ProductIsActive],
                [ProductMeasurementUnit],
                [ProductDescription],
                [ProductName],
                [ProductCategoryId],
                [ProductCategoryIsActive],
                [ProductCategoryName],
                [DealId],
                [DealEndDate],
                [DealStartDate],
                [DealIsActive],
                [DealOffPercentage]
            FROM 
                OrderProductOffer op
            INNER JOIN 
                ProductOffer po ON po.Id = op.ProductOffer 
            INNER JOIN 
                dbo.GetProductDetailsFN(@language) pd ON po.Product = pd.ProductId
                WHERE 
                    op.[Order] IN @OrderIds;";

        var parameters = new DynamicParameters();
        parameters.Add("language", language);
        parameters.Add("OrderIds", orderIds.ToArray());

        return await connection.QueryAsync<OrderProductOfferDetailsDto>(sql, parameters);
    }
}

#endregion


public static partial class OrderRepository
{
    public static async Task<IEnumerable<OrderDetailsForDeliveryDto>>
    GetOrderDetailsForDeliveryAsync(
        IDbConnection connection,
        int delivery,
        byte status)
    {
        var sql = $@"
        SELECT 
            o.Id,
            o.OrderStatus,
            o.TotalPrice,
            o.TransactionId,
            o.Date,
            o.PaymentType,
            o.IsPaid,
            o.ItemsPrice,
            o.SystemFee,
            o.DeliveryFee,
            o.SpecialRequest,
            oci.Customer,
            oci.Location AS CustomerLocation,
            oci.Id AS CustomerInformationId,
            opi.Provider,
            opi.Location AS ProviderLocation,
            opi.Id AS ProviderInformationId,
            odi.Delivery,
            odi.Id AS DeliveryInformationId,

            cl.[Name] AS CustomerLocationName,
            cl.[Latitude] AS CustomerLocationLatitude,
            cl.[Longitude] AS CustomerLocationLongitude,
            pl.[Name] AS ProviderLocationName,
            pl.[Latitude] AS ProviderLocationLatitude,
            pl.[Longitude] AS ProviderLocationLongitude,

            pfp.DisplayName AS ProviderName,
            pfc.DisplayName AS CustomerName
        FROM 
            dbo.[Order] o
        LEFT JOIN 
            dbo.[OrderCustomerInformation] oci ON oci.[Order] = o.Id
        LEFT JOIN 
            dbo.[OrderDeliveryInformation] odi ON odi.[Order] = o.Id
        LEFT JOIN 
            dbo.[OrderProviderInformation] opi ON opi.[Order] = o.Id

        LEFT JOIN 
            dbo.[Location] cl ON cl.Id = oci.[Location]
        LEFT JOIN 
            dbo.[Location] pl ON pl.Id = opi.[Location]
        INNER JOIN 
            dbo.[Party] pfc ON pfc.Id = oci.Customer
        INNER JOIN 
            dbo.[Party] pfp ON pfp.Id = opi.Provider
        WHERE 
            odi.Delivery = @delivery AND o.OrderStatus = @status
        ORDER BY 
            o.[Id];";

        return await connection.QueryAsync<OrderDetailsForDeliveryDto>(sql,
            new
            {
                delivery,
                status
            });
    }
}
public class OrderDetailsForDeliveryDto
{
    public long Id { get; set; }
    public byte OrderStatus { get; set; }
    public decimal TotalPrice { get; set; }
    public string TransactionId { get; set; } = null!;
    public DateTime Date { get; set; }
    public byte PaymentType { get; set; }
    public bool IsPaid { get; set; }
    public decimal ItemsPrice { get; set; }
    public decimal SystemFee { get; set; }
    public decimal DeliveryFee { get; set; }
    public string? SpecialRequest { get; set; }
    public int? Customer { get; set; }
    public int? CustomerLocation { get; set; }
    public int? CustomerInformationId { get; set; }

    public int? Provider { get; set; }
    public int? ProviderLocation { get; set; }
    public int? ProviderInformationId { get; set; }

    public int? Delivery { get; set; }
    public int? DeliveryInformationId { get; set; }

    public string? CustomerLocationName { get; set; }
    public decimal? CustomerLocationLatitude { get; set; }
    public decimal? CustomerLocationLongitude { get; set; }
    public string? ProviderLocationName { get; set; }
    public decimal? ProviderLocationLatitude { get; set; }
    public decimal? ProviderLocationLongitude { get; set; }

    // New fields for Party information
    public string ProviderName { get; set; }
    public string CustomerName { get; set; }
}


#region get offers
public static partial class OrderRepository
{
    public static async Task<IEnumerable<OrderProductOfferDto>> GetAllOrderProductOffersByOrderIdAsync(IDbConnection connection,
        long orderId)
    {
        var sql = @"
            SELECT 
                op.[Id],
                op.[Order],
                op.[ProductOffer], 
                op.[Quantity],
                op.UnitPrice
            FROM 
                OrderProductOffer op
            WHERE 
                op.[Order] = @OrderId;";

        return await connection.QueryAsync<OrderProductOfferDto>(sql, new
        {
            OrderId = orderId
        });
    }
}
public class OrderProductOfferDto
{
    public long Id { get; set; }
    public long Order { get; set; }
    public int ProductOffer { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
#endregion