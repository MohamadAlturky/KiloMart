using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;

namespace KiloMart.Requests.Queries;

public static partial class Query
{
    public static async Task<List<OrderJoinParty>> GetOrderByProvider(IDbFactory factory, UserPayLoad payLoad,
        int provider)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();
        const string query =
            @"SELECT p.Id,p.DisplayName,p.IsActive oid FROM [Order] o inner join Party p  on o.Provider=p.Id where o.Provider=@provider";

        var result = await connection.QueryAsync<OrderJoinParty>(query, new
        {
            provider = provider,
        });
        return result.ToList();
    }

    public static async Task<List<OrderJoinParty>> GetOrderByProviderAndStatus(IDbFactory factory, UserPayLoad payLoad,
        int provider, bool status)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();
        const string query =
            @"SELECT p.Id,p.DisplayName,p.IsActive oid FROM [Order] o inner join Party p  on o.Provider=p.Id where o.Provider=@provider and o.OrderStatus=@status";

        var result = await connection.QueryAsync<OrderJoinParty>(query, new
        {
            provider = provider,
            status = status,
        });
        return result.ToList();
    }

    public static async Task<List<OrderJoinParty>> GetOrderByCustomer(IDbFactory factory, UserPayLoad payLoad,
        int customer)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();
        const string query =
            @"SELECT p.Id,p.DisplayName,p.IsActive oid FROM [Order] o inner join Party p  on o.Customer=p.Id";

        var result = await connection.QueryAsync<OrderJoinParty>(query, new
        {
            customer = customer,
        });
        return result.ToList();
    }

    public static async Task<List<OrderJoinParty>> GetOrderByCustomerAndStatus(IDbFactory factory, UserPayLoad payLoad,
        int customer, bool status)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();
        const string query =
            @"SELECT p.Id,p.DisplayName,p.IsActive oid FROM [Order] o inner join Party p  on o.Customer=p.Id and o.OrderStatus=@status";

        var result = await connection.QueryAsync<OrderJoinParty>(query, new
        {
            customer = customer,
            status = status,
        });
        return result.ToList();
    }

    public class OrderJoinParty
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
    }
}