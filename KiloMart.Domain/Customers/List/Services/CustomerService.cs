using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.CustomerDtos.List.Models;
using KiloMart.Domain.Languages.Models;

namespace KiloMart.Domain.Customers.List.Services;

public static class CustomerService
{
    public static async Task<List<CustomerDto>> List(IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var customers = await connection.QueryAsync<CustomerDto>(
            "SELECT * FROM Customer;"
        );
        return customers.ToList();
    }
    public static async Task<List<CustomerDto>> ListWithDetails(IDbFactory dbFactory, Language language)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        // join the customer table with the customer profile table
        var customers = await connection.QueryAsync<CustomerDto>(
            "SELECT * FROM Customer LEFT JOIN CustomerProfile ON Customer.Party = CustomerProfile.Customer;"
        );
        return customers.ToList();
    }
}
