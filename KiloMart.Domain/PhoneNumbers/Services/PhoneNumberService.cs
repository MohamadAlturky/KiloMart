using KiloMart.Domain.PhoneNumbers.Models;
using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.DataAccess.Models;

namespace KiloMart.Domain.PhoneNumbers.Services;

public static class PhoneNumberService
{
    public static Result<CreatePhoneNumberResponse> Insert(IDbFactory dbFactory, CreatePhoneNumberRequest phoneNumber)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            // SQL query to insert into PhoneNumber table
            const string sql = @"
                INSERT INTO PhoneNumber (Value, Party)
                VALUES (@Value, @Party);
                SELECT CAST(SCOPE_IDENTITY() as int);"; // Retrieve the generated Id

            // Execute the insert and retrieve the new Id
            CreatePhoneNumberResponse response = new();
            response.Id = connection.QuerySingle<int>(sql, new
            {
                phoneNumber.Value,
                phoneNumber.Party
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
