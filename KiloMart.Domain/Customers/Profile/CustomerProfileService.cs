using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.Domain.Customers.Profile.Models;

namespace KiloMart.Domain.Customers.Profile.Services
{
    public static class CustomerProfileService
    {
        public static async Task<Result<CreateCustomerProfileResponse>> InsertAsync(
            IDbFactory dbFactory,
            CreateCustomerProfileRequest customerProfileRequest)
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            try
            {
                // SQL to insert into CustomerProfile and retrieve the new Id
                const string insertCustomerProfileSql = @"
                    INSERT INTO CustomerProfile (Customer, FirstName, SecondName, NationalName, NationalId)
                    VALUES (@Customer, @FirstName, @SecondName, @NationalName, @NationalId);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                // Insert into CustomerProfile and get new ID
                var customerProfileId = await connection.QuerySingleAsync<int>(insertCustomerProfileSql, new
                {
                    customerProfileRequest.Customer,
                    customerProfileRequest.FirstName,
                    customerProfileRequest.SecondName,
                    customerProfileRequest.NationalName,
                    customerProfileRequest.NationalId
                });

                
                return Result<CreateCustomerProfileResponse>.Ok(new CreateCustomerProfileResponse
                {
                    Id = customerProfileId,
                    Customer = customerProfileRequest.Customer,
                    FirstName = customerProfileRequest.FirstName,
                    SecondName = customerProfileRequest.SecondName,
                    NationalName = customerProfileRequest.NationalName,
                    NationalId = customerProfileRequest.NationalId
                });
            }
            catch (Exception e)
            {
                return Result<CreateCustomerProfileResponse>.Fail([e.Message]);
            }
        }
    }
}
