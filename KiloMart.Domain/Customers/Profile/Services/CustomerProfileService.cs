using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.DataAccess.Models;
using KiloMart.Domain.Customers.Profile.Models;

namespace KiloMart.Domain.Customers.Profile.Services;

public static class CustomerProfileService
{
    public static Result<CustomerProfileDto> Insert(IDbFactory dbFactory, CustomerProfileDto customerProfile)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            // SQL query to insert into CustomerProfile table
            const string sql = @"
                INSERT INTO CustomerProfile (Customer, FirstName, SecondName, NationalName, NationalId)
                VALUES (@Customer, @FirstName, @SecondName, @NationalName, @NationalId);
                SELECT CAST(SCOPE_IDENTITY() as int);"; // Retrieve the generated Id

            // Execute the insert and retrieve the new Id
            customerProfile.Id = connection.QuerySingle<int>(sql, new
            {
                Customer = customerProfile.Customer,
                FirstName = customerProfile.FirstName,
                SecondName = customerProfile.SecondName,
                NationalName = customerProfile.NationalName,
                NationalId = customerProfile.NationalId
            });

            return Result<CustomerProfileDto>.Ok(customerProfile);
        }
        catch (Exception e)
        {
            return Result<CustomerProfileDto>.Fail([e.Message]);
        }
    }
}
