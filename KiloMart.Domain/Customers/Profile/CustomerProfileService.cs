using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Domain.Customers.Profile;

public static class CustomerProfileService
{
    public static async Task<Result<CreateCustomerProfileResponse>> InsertAsync(
        IDbFactory dbFactory,
        CreateCustomerProfileRequest customerProfileRequest)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();

        var existingProfile = await Db.GetCustomerProfileByCustomerIdAsync(customerProfileRequest.Customer, connection);
        if (existingProfile is not null)
        {
            return Result<CreateCustomerProfileResponse>.Fail(["Profile Already exist"]);
        }
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

    public static async Task<Result<UpdateCustomerProfileResponse>> UpdateAsync(
            IDbFactory dbFactory,
            UserPayLoad userPayLoad,
            UpdateCustomerProfileRequest customerProfileRequest)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var existingProfile = await Db.GetCustomerProfileByCustomerIdAsync(userPayLoad.Party, connection);
        if(existingProfile is null)
        {
            return Result<UpdateCustomerProfileResponse>.Fail(["Not Found"]);
        }
        if (existingProfile.Customer != userPayLoad.Party)
        {
            return Result<UpdateCustomerProfileResponse>.Fail(["Un Authorized"]);
        }

        try
        {
            existingProfile.NationalId = customerProfileRequest.NationalId??existingProfile.NationalId;
            existingProfile.NationalName = customerProfileRequest.NationalName??existingProfile.NationalName;
            existingProfile.FirstName = customerProfileRequest.FirstName??existingProfile.FirstName;
            existingProfile.SecondName = customerProfileRequest.SecondName??existingProfile.SecondName;

            // SQL to update the CustomerProfile
            const string updateCustomerProfileSql = @"
                    UPDATE CustomerProfile
                    SET FirstName = @FirstName,
                        SecondName = @SecondName,
                        NationalName = @NationalName,
                        NationalId = @NationalId
                    WHERE Customer = @Customer;";

            // Execute the update query
            var affectedRows = await connection.ExecuteAsync(updateCustomerProfileSql, new
            {
                customerProfileRequest.FirstName,
                customerProfileRequest.SecondName,
                customerProfileRequest.NationalName,
                customerProfileRequest.NationalId,
                Customer = userPayLoad.Party
            });

            // Check if the update was successful
            if (affectedRows > 0)
            {
                return Result<UpdateCustomerProfileResponse>.Ok(new UpdateCustomerProfileResponse
                {
                    FirstName = existingProfile.FirstName,
                    SecondName = existingProfile.SecondName,
                    NationalName = existingProfile.NationalName,
                    NationalId = existingProfile.NationalId
                });
            }
            else
            {
                return Result<UpdateCustomerProfileResponse>.Fail(new[] { "Update failed, no rows affected." });
            }
        }
        catch (Exception e)
        {
            return Result<UpdateCustomerProfileResponse>.Fail(new[] { e.Message });
        }
    }
}
