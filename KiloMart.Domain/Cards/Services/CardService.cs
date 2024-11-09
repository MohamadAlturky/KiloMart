using KiloMart.Domain.Cards.Models;
using Dapper;
using KiloMart.Core.Models;
using KiloMart.Core.Contracts;

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
                INSERT INTO Card (HolderName, Number, SecurityCode, ExpireDate, Customer,IsActive)
                VALUES (@HolderName, @Number, @SecurityCode, @ExpireDate, @Customer,@IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int);"; // Retrieve the generated Id

            // Execute the insert and retrieve the new Id
            card.Id = connection.QuerySingle<int>(sql, new
            {
                HolderName = card.HolderName,
                Number = card.Number,
                SecurityCode = card.SecurityCode,
                ExpireDate = card.ExpireDate,
                Customer = card.Customer,
                IsActive = true
            });

            return Result<CardDto>.Ok(card);
        }
        catch (Exception e)
        {
            return Result<CardDto>.Fail([e.Message]);
        }
    }
}
