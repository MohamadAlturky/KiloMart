﻿using Dapper;
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
            provider = payLoad.Party, status,
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
            customer = payLoad.Party, status,
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
    
}