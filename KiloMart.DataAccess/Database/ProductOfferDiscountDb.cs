using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database
{
    public static partial class Db
    {
        public static async Task<long> InsertProductOfferDiscountAsync(IDbConnection connection,
            int productOffer,
            int discountCode,
            DateTime assignedDate,
            IDbTransaction? transaction = null)
        {
            const string query = @"INSERT INTO [dbo].[ProductOfferDiscount]
                                ([ProductOffer], [DiscountCode], [AssignedDate], [IsActive])
                                VALUES (@ProductOffer, @DiscountCode, @AssignedDate, @IsActive)
                                SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

            return await connection.ExecuteScalarAsync<long>(query, new
            {
                ProductOffer = productOffer,
                DiscountCode = discountCode,
                AssignedDate = assignedDate,
                IsActive = true
            }, transaction);
        }

        public static async Task<bool> UpdateProductOfferDiscountAsync(IDbConnection connection,
            long id,
            int productOffer,
            int discountCode,
            DateTime assignedDate,
            bool isActive,
            IDbTransaction? transaction = null)
        {
            const string query = @"UPDATE [dbo].[ProductOfferDiscount]
                                SET 
                                [ProductOffer] = @ProductOffer,
                                [DiscountCode] = @DiscountCode,
                                [AssignedDate] = @AssignedDate,
                                [IsActive] = @IsActive
                                WHERE [Id] = @Id";

            var updatedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id,
                ProductOffer = productOffer,
                DiscountCode = discountCode,
                AssignedDate = assignedDate,
                IsActive = isActive
            }, transaction);

            return updatedRowsCount > 0;
        }

        public static async Task<bool> DeleteProductOfferDiscountAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
        {
            const string query = @"DELETE FROM [dbo].[ProductOfferDiscount]
                                WHERE [Id] = @Id";

            var deletedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id
            }, transaction);

            return deletedRowsCount > 0;
        }

        public static async Task<ProductOfferDiscount?> GetProductOfferDiscountByIdAsync(long id, IDbConnection connection)
        {
            const string query = @"SELECT 
                                [Id], 
                                [ProductOffer], 
                                [DiscountCode], 
                                [AssignedDate], 
                                [IsActive]
                                FROM [dbo].[ProductOfferDiscount]
                                WHERE [Id] = @Id";

            return await connection.QueryFirstOrDefaultAsync<ProductOfferDiscount>(query, new
            {
                Id = id
            });
        }
    }

    public class ProductOfferDiscount
    {
        public long Id { get; set; }
        public int ProductOffer { get; set; }
        public int DiscountCode { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
