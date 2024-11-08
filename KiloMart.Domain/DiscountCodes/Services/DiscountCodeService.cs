using KiloMart.Domain.DiscountCodes.Models;
using Dapper;
using KiloMart.DataAccess.Models;
using KiloMart.DataAccess.Contracts;

namespace KiloMart.Domain.DiscountCodes.Services;

public static class DiscountCodeService
{
    public static Result<CreateDiscountCodeResponse> Insert(IDbFactory dbFactory,
     CreateDiscountCodeRequest discountCode)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            // SQL query to insert into DiscountCode table
            const string sql = @"
                INSERT INTO DiscountCode (Code, Value, Description, StartDate, EndDate, DiscountType, IsActive)
                VALUES (@Code, @Value, @Description, @StartDate, @EndDate, @DiscountType, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int);"; // Retrieve the generated Id

            // Execute the insert and retrieve the new Id
            int id = connection.QuerySingle<int>(sql, new
            {
                discountCode.Code,
                discountCode.Value,
                discountCode.Description,
                discountCode.StartDate,
                discountCode.EndDate,
                discountCode.DiscountType,
                IsActive = true,
            });

            return Result<CreateDiscountCodeResponse>.Ok(new CreateDiscountCodeResponse()
            {
                Id = id,
                Code = discountCode.Code,
                Value = discountCode.Value,
                Description = discountCode.Description,
                StartDate = discountCode.StartDate,
                EndDate = discountCode.EndDate,
                DiscountType = discountCode.DiscountType,
                IsActive = true
            });
        }
        catch (Exception e)
        {
            return Result<CreateDiscountCodeResponse>.Fail([e.Message]);
        }
    }
}
