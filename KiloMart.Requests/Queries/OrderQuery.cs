using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;

namespace KiloMart.Requests.Queries;

public static partial class Query
{
    public static async Task<List<Order>> CardById(IDbFactory factory, UserPayLoad payLoad)
    {
        using var connection = factory.CreateDbConnection();
        connection.Open();
        const string query = @"SELECT * FROM dbo.Card where Id=@Id";

        var result = await connection.QueryAsync<Order>(query, new
        {
            Id = 5
        });
        return result.ToList();
    }

    public class Order
    {
        public long Id { get; set; }
        public byte OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public string TransactionId { get; set; } = null!;
        public int CustomerLocation { get; set; }
        public int ProviderLocation { get; set; }
        public int Customer { get; set; }
        public int Provider { get; set; }
    }
}