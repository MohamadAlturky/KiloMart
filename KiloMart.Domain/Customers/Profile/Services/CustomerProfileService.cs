using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.DataAccess.Models;
using KiloMart.Domain.Customers.Profile.Models;

namespace KiloMart.Domain.Customers.Profile.Services
{
    public static class CustomerProfileService
    {
        public static async Task<Result<CreateCustomerProfileResponse>> InsertAsync(
            IDbFactory dbFactory,
            CreateCustomerProfileRequest customerProfileRequest,
            CreateCustomerProfileLocalizedRequest localizedRequest)
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

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
                }, transaction);

                // SQL to insert into CustomerProfileLocalized using the newly created CustomerProfile Id
                const string insertCustomerProfileLocalizedSql = @"
                    INSERT INTO CustomerProfileLocalized (CustomerProfile, Language, FirstName, SecondName, NationalName)
                    VALUES (@CustomerProfile, @Language, @FirstName, @SecondName, @NationalName);";

                // Execute the insert into CustomerProfileLocalized
                await connection.ExecuteAsync(insertCustomerProfileLocalizedSql, new
                {
                    CustomerProfile = customerProfileId,
                    localizedRequest.Language,
                    localizedRequest.FirstName,
                    localizedRequest.SecondName,
                    localizedRequest.NationalName
                }, transaction);

                // Commit transaction if both inserts are successful
                transaction.Commit();

                // Prepare response
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
                transaction.Rollback();
                return Result<CreateCustomerProfileResponse>.Fail([e.Message]);
            }
        }
    }
}
