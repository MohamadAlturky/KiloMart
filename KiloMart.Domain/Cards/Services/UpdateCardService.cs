using Dapper;
using KiloMart.Core.Models;
using KiloMart.Core.Contracts;

namespace KiloMart.Domain.Cards.Services
{
    public static class UpdateCardService
    {
        
        public static async Task<Result> Update(IDbFactory dbFactory, UpdateCardDto dto, int id, int party)
        {
            try
            {
                using var connection = dbFactory.CreateDbConnection();
                connection.Open();
                
                const string getCardSql = @"
                    SELECT 
                        Id,
                        HolderName,
                        Number,
                        SecurityCode,
                        ExpireDate,
                        IsActive,
                        Customer
                    FROM Card
                    WHERE Id = @Id";

                var parameters = new
                {
                    Id = id
                };

                var existingCard = await connection.QuerySingleAsync<CardReadModel?>(getCardSql, parameters);
                if (existingCard is null)
                {
                    return Result.Fail(["can't find the card"]);
                }
                
                if(existingCard.Customer != party)
                {
                    return Result.Fail(["don't has access to another customers cards"]);
                }

                if(dto.ExpireDate.HasValue)
                {
                    existingCard.ExpireDate = dto.ExpireDate.Value;
                }
                if(!string.IsNullOrEmpty(dto.SecurityCode))
                {
                    existingCard.SecurityCode = dto.SecurityCode;
                }
                if(!string.IsNullOrEmpty(dto.HolderName))
                {
                    existingCard.HolderName = dto.HolderName;
                }
                if(!string.IsNullOrEmpty(dto.Number))
                {
                    existingCard.Number = dto.Number;
                }
                if(dto.IsActive.HasValue)
                {
                    existingCard.IsActive = dto.IsActive.Value;
                }
                
                // SQL query to update the Card table
                const string sql = @"
                    UPDATE Card
                    SET HolderName = @HolderName,
                        Number = @Number,
                        SecurityCode = @SecurityCode,
                        ExpireDate = @ExpireDate,
                        IsActive = @IsActive
                    WHERE Id = @Id";

                // Parameters for the update query
                var updateParameters = new
                {
                    HolderName = existingCard.HolderName,
                    Number = existingCard.Number,
                    SecurityCode = existingCard.SecurityCode,
                    ExpireDate = existingCard.ExpireDate,
                    Customer = existingCard.Customer,
                    IsActive = existingCard.IsActive,
                    Id = existingCard.Id
                };

                // Execute the update query
                var rowsAffected = connection.Execute(sql, updateParameters);

                // Check if the update was successful
                if (rowsAffected == 0)
                {
                    return Result.Fail(["No rows were affected by the update."]);
                }

                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Fail([e.Message]);
            }
        }
    }
}

public class UpdateCardDto
{
    public string? HolderName { get; set; }

    public string? Number { get; set; } 

    public string? SecurityCode { get; set; }

    public DateTime? ExpireDate { get; set; }
    public bool? IsActive { get; set; }
}


public class CardReadModel
{
    public int Id { get; set; }
    public string HolderName { get; set; }
    public string Number { get; set; }
    public string SecurityCode { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool IsActive { get; set; }
    public int Customer { get; set; }
}
