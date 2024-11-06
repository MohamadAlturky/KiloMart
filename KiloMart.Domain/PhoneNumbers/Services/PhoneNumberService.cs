using KiloMart.Domain.PhoneNumbers.Models;
using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.DataAccess.Models;

namespace KiloMart.Domain.PhoneNumbers.Services;

public static class PhoneNumberService
{
    public static Result<PhoneNumberDto> Insert(IDbFactory dbFactory, PhoneNumberDto phoneNumber)
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
            phoneNumber.Id = connection.QuerySingle<int>(sql, new
            {
                phoneNumber.Value,
                phoneNumber.Party
            });

            return Result<PhoneNumberDto>.Ok(phoneNumber);
        }
        catch (Exception)
        {
            return Result<PhoneNumberDto>.Fail();
        }
    }
}
