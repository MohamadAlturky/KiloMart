using KiloMart.Domain.DiscountCodes.Models;
using Dapper;
using KiloMart.DataAccess.Models;
using KiloMart.DataAccess.Contracts;

namespace KiloMart.Domain.DiscountCodes.Services;

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
        catch (Exception e)
        {
            return Result<DiscountCodeDto>.Fail([e.Message]);
        }
    }
}
