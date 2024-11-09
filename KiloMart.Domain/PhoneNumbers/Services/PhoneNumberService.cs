using KiloMart.Domain.PhoneNumbers.Models;
using Dapper;
using KiloMart.Core.Models;
using KiloMart.Core.Contracts;

namespace KiloMart.Domain.PhoneNumbers.Services;

public static class PhoneNumberService
{
    public static async Task<Result<CreatePhoneNumberResponse>> Insert(IDbFactory dbFactory, CreatePhoneNumberRequest phoneNumber)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            // SQL query to insert into PhoneNumber table
            const string sql = @"
                INSERT INTO PhoneNumber (Value, Party,IsActive)
                VALUES (@Value, @Party, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int);"; // Retrieve the generated Id

            // Execute the insert and retrieve the new Id
            CreatePhoneNumberResponse response = new();
            response.Id = await connection.QuerySingleAsync<int>(sql, new
            {
                Value = phoneNumber.Value,
                Party = phoneNumber.Party,
                IsActive = true
            });
            response.Value = phoneNumber.Value;
            response.Party = phoneNumber.Party;

            return Result<CreatePhoneNumberResponse>.Ok(response);
        }
        catch (Exception e)
        {
            return Result<CreatePhoneNumberResponse>.Fail([e.Message]);
        }
    }
}
