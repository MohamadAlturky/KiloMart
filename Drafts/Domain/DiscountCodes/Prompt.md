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


------------------------------------------------
namespace KiloMart.Domain.PhoneNumbers.Models;

public class PhoneNumberDto
{
    public int Id { get; set; }

    public string Value { get; set; } = string.Empty;

    public int Party { get; set; }


    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(Value))
            errors.Add("Value is required");
        if (Party <= 0)
            errors.Add("Party is required");

        return (errors.Count == 0, errors.ToArray());
    }
}

-------------------------------------------------

as this code write the service and the model for 


