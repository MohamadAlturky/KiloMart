using KiloMart.Domain.Cards.Models;
using Dapper;
using KiloMart.DataAccess.Models;
using KiloMart.DataAccess.Contracts;

namespace KiloMart.Domain.Cards.Services;

public static class CardService
{
    public static Result<CardDto> Insert(IDbFactory dbFactory, CardDto card)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            // SQL query to insert into Card table
            const string sql = @"
                INSERT INTO Card (HolderName, Number, SecurityCode, ExpireDate, Customer)
                VALUES (@HolderName, @Number, @SecurityCode, @ExpireDate, @Customer);
                SELECT CAST(SCOPE_IDENTITY() as int);"; // Retrieve the generated Id

            // Execute the insert and retrieve the new Id
            card.Id = connection.QuerySingle<int>(sql, new
            {
                card.HolderName,
                card.Number,
                card.SecurityCode,
                card.ExpireDate,
                card.Customer
            });

            return Result<CardDto>.Ok(card);
        }
        catch (Exception)
        {
            return Result<CardDto>.Fail();
        }
    }
}
