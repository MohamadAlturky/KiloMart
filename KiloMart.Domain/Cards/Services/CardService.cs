using KiloMart.Domain.Cards.Models;
using Dapper;
using KiloMart.Core.Models;
using KiloMart.Core.Contracts;

namespace KiloMart.Domain.Cards.Services;

public static class CardService
{
    public static Result<int> Insert(IDbFactory dbFactory, CardDto card,int party)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            // SQL query to insert into Card table
            const string sql = @"
                INSERT INTO Card (HolderName, Number, SecurityCode, ExpireDate, Customer,IsActive)
                VALUES (@HolderName, @Number, @SecurityCode, @ExpireDate, @Customer,@IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int);"; // Retrieve the generated Id

            // Execute the insert and retrieve the new Id
            var id = connection.QuerySingle<int>(sql, new
            {
                HolderName = card.HolderName,
                Number = card.Number,
                SecurityCode = card.SecurityCode,
                ExpireDate = card.ExpireDate,
                Customer = party,
                IsActive = true
            });

            return Result<int>.Ok(id);
        }
        catch (Exception e)
        {
            return Result<int>.Fail([e.Message]);
        }
    }
}

