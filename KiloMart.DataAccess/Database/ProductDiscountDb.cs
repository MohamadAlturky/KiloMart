using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

public static partial class Db
{
    public static async Task<long> InsertProductDiscountAsync(IDbConnection connection,
        int product,
        int discountCode,
        DateTime assignedDate,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[ProductDiscount]
                            ([Product], [DiscountCode], [AssignedDate], [IsActive])
                            VALUES (@Product, @DiscountCode, @AssignedDate, 1)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Product = product,
            DiscountCode = discountCode,
            AssignedDate = assignedDate
        }, transaction);
    }

    public static async Task<bool> UpdateProductDiscountAsync(IDbConnection connection,
        long id,
        int product,
        int discountCode,
        DateTime assignedDate,
        bool isActive,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[ProductDiscount]
                                SET
                                [Product] = @Product,
                                [DiscountCode] = @DiscountCode,
                                [AssignedDate] = @AssignedDate,
                                [IsActive] = @IsActive
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Product = product,
            DiscountCode = discountCode,
            AssignedDate = assignedDate,
            IsActive = isActive
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteProductDiscountAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[ProductDiscount]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<ProductDiscount?> GetProductDiscountByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT
                            [Id],
                            [Product],
                            [DiscountCode],
                            [AssignedDate],
                            [IsActive]
                            FROM [dbo].[ProductDiscount]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<ProductDiscount>(query, new
        {
            Id = id
        });
    }
}

public class ProductDiscount
{
    public long Id { get; set; }
    public int Product { get; set; }
    public int DiscountCode { get; set; }
    public DateTime AssignedDate { get; set; }
    public bool IsActive { get; set; }
}
