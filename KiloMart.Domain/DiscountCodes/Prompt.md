using KiloMart.Domain.DiscountCode.Models;
using Dapper;
using KiloMart.DataAccess.Models;
using KiloMart.DataAccess.Contracts;

namespace KiloMart.Domain.DiscountCode.Services;

public static class DiscountCodeService
{
    public static Result<DiscountCodeDto> Insert(IDbFactory dbFactory, DiscountCodeDto discountCode)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            // SQL query to insert into DiscountCode table
            const string sql = @"
                INSERT INTO DiscountCode (Code, Value, Description, StartDate, EndDate, DiscountType, IsActive)
                VALUES (@Code, @Value, @Description, @StartDate, @EndDate, @DiscountType, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int);"; // Retrieve the generated Id

            // Execute the insert and retrieve the new Id
            discountCode.Id = connection.QuerySingle<int>(sql, new
            {
                discountCode.Code,
                discountCode.Value,
                discountCode.Description,
                discountCode.StartDate,
                discountCode.EndDate,
                discountCode.DiscountType,
                discountCode.IsActive
            }, transaction);

            transaction.Commit();
            return Result<DiscountCodeDto>.Ok(discountCode);
        }
        catch (Exception)
        {
            return Result<DiscountCodeDto>.Fail();
        }
    }
}

------------------------------------------------
namespace KiloMart.Domain.DiscountCode.Models;

public class DiscountCodeDto
{
    public int Id { get; set; }
    
    public string Code { get; set; } = string.Empty;
    
    public decimal Value { get; set; }
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public byte DiscountType { get; set; }
    
    public bool IsActive { get; set; }
}
-------------------------------------------------

as this code write the service and the model for 
