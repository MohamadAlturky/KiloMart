using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;

namespace KiloMart.Requests.Queries;

public static partial class Query
{
    public static async Task<List<OrderJoinParty>> GetOrderByProvider(IDbFactory factory, UserPayLoad payLoad)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();
        const string query =
            @"SELECT p.Id as PartyId, p.DisplayName as PartyDisplayName, p.IsActive as PartyIsActive, 
            o.Id as OrderId, o.TotalPrice, o.TransactionId, o.CustomerLocation, o.ProviderLocation, 
            o.Customer, o.Provider 
            FROM [Order] o inner join Party p on o.Provider=p.Id where o.Provider=@provider";

        var result = await connection.QueryAsync<OrderJoinParty>(query, new
        {
            provider = payLoad.Party,
        });
        return result.ToList();
    }

    public static async Task<List<OrderJoinParty>> GetOrderByProviderAndStatus(IDbFactory factory, UserPayLoad payLoad,
        bool status)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();
        const string query =
            @"SELECT p.Id as PartyId, p.DisplayName as PartyDisplayName, p.IsActive as PartyIsActive, 
            o.Id as OrderId, o.TotalPrice, o.TransactionId, o.CustomerLocation, o.ProviderLocation, 
            o.Customer, o.Provider 
            FROM [Order] o inner join Party p on o.Provider=p.Id where o.Provider=@provider and o.OrderStatus=@status";

        var result = await connection.QueryAsync<OrderJoinParty>(query, new
        {
            provider = payLoad.Party,
            status,
        });
        return result.ToList();
    }

    public static async Task<List<OrderJoinParty>> GetOrderByCustomer(IDbFactory factory, UserPayLoad payLoad)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();
        const string query =
            @"SELECT p.Id as PartyId, p.DisplayName as PartyDisplayName, p.IsActive as PartyIsActive, 
            o.Id as OrderId, o.TotalPrice, o.TransactionId, o.CustomerLocation, o.ProviderLocation, 
            o.Customer, o.Provider 
            FROM [Order] o inner join Party p on o.Customer=p.Id";

        var result = await connection.QueryAsync<OrderJoinParty>(query, new
        {
            customer = payLoad.Party,
        });
        return result.ToList();
    }

    public static async Task<List<OrderJoinParty>> GetOrderByCustomerAndStatus(IDbFactory factory, UserPayLoad payLoad,
        bool status)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();
        const string query =
            @"SELECT p.Id as PartyId, p.DisplayName as PartyDisplayName, p.IsActive as PartyIsActive, 
            o.Id as OrderId, o.TotalPrice, o.TransactionId, o.CustomerLocation, o.ProviderLocation, 
            o.Customer, o.Provider 
            FROM [Order] o inner join Party p on o.Customer=p.Id and o.OrderStatus=@status";

        var result = await connection.QueryAsync<OrderJoinParty>(query, new
        {
            customer = payLoad.Party,
            status,
        });
        return result.ToList();
    }

    public class OrderJoinParty
    {
        public long PartyId { get; set; }
        public string PartyDisplayName { get; set; }
        public bool PartyIsActive { get; set; }
        public byte OrderId { get; set; }
        public int TotalPrice { get; set; }
        public string TransactionId { get; set; }
        public int CustomerLocation { get; set; }
        public int ProviderLocation { get; set; }
        public int Customer { get; set; }
        public int Provider { get; set; }
    }

    public static async Task<List<OrderJoinOrderDiscount>> GetOrderByCustomerWithDiscount(IDbFactory factory,
        UserPayLoad payLoad)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();
        const string query =
            @"SELECT o.Id as OrderId, o.Customer, o.Provider, o.CustomerLocation, o.ProviderLocation, 
            o.TransactionId, o.TotalPrice, od.DiscountCode, o.OrderStatus as orderStatus
            FROM [Order] o inner join OrderDiscount od on o.Id=od.OrderId";

        var result = await connection.QueryAsync<OrderJoinOrderDiscount>(query, new
        {
            customer = payLoad.Party,
        });
        return result.ToList();
    }

    public static async Task<List<OrderJoinOrderDiscount>> GetOrderByCustomerAndStatusWithDiscount(IDbFactory factory,
        UserPayLoad payLoad,
        bool status)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();
        const string query =
            @"SELECT o.Id as OrderId, o.Customer, o.Provider, o.CustomerLocation, o.ProviderLocation, 
            o.TransactionId, o.TotalPrice, od.DiscountCode, o.OrderStatus as orderStatus
            FROM [Order] o inner join OrderDiscount od on o.Id=od.OrderId and o.OrderStatus=@status";

        var result = await connection.QueryAsync<OrderJoinOrderDiscount>(query, new
        {
            customer = payLoad.Party,
            status,
        });
        return result.ToList();
    }

    public class OrderJoinOrderDiscount
    {
        public long OrderId { get; set; }
        public string Customer { get; set; }
        public string Provider { get; set; }
        public string CustomerLocation { get; set; }
        public string ProviderLocation { get; set; }
        public string TransactionId { get; set; }
        public int TotalPrice { get; set; }
        public string DiscountCode { get; set; }
        public bool orderStatus { get; set; }
    }
    public class OrderDetails
    {
        public long OrderId { get; set; }
        public decimal OrderTotalPrice { get; set; }
        public string TransactionId { get; set; }
        public int CustomerLocation { get; set; }
        public int ProviderLocation { get; set; }
        public long OrderItemId { get; set; }
        public float ItemQuantity { get; set; }
        public decimal ItemUnitPrice { get; set; }
        public decimal OfferPrice { get; set; }
        public decimal OffPercentage { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string LocalizedProductName { get; set; }
        public string LocalizedProductDescription { get; set; }
        public string CustomerLocationName { get; set; }
        public string ProviderLocationName { get; set; }
    }
    public static async Task<List<OrderDetails>> GetOrderDetails(IDbFactory factory, long orderId)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();
        const string query = @"
        SELECT 
            o.Id AS OrderId,
            o.TotalPrice AS OrderTotalPrice,
            o.TransactionId,
            o.CustomerLocation,
            o.ProviderLocation,
            oi.Id AS OrderItemId,
            oi.Quantity AS ItemQuantity,
            oi.UnitPrice AS ItemUnitPrice,
            po.Price AS OfferPrice,
            po.OffPercentage AS OfferDiscountPercentage,
            p.Name AS ProductName,
            p.Description AS ProductDescription,
            pl.Name AS LocalizedProductName,
            pl.Description AS LocalizedProductDescription,
            l1.Name AS CustomerLocationName,
            l2.Name AS ProviderLocationName
        FROM [Order] o
        INNER JOIN OrderItem oi ON o.Id = oi.[Order]
        INNER JOIN ProductOffer po ON oi.ProductOffer = po.Id
        INNER JOIN Product p ON po.Product = p.Id	
        LEFT JOIN ProductLocalized pl ON p.Id = pl.Product
        INNER JOIN Location l1 ON o.CustomerLocation = l1.Id
        INNER JOIN Location l2 ON o.ProviderLocation = l2.Id
        WHERE o.Id = @orderId";

        var result = await connection.QueryAsync<OrderDetails>(query, new
        {
            orderId
        });
        return result.ToList();
    }


}