using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database
{
    public static partial class Db
    {
        public static async Task<int> InsertProductOfferAsync(IDbConnection connection,
            int product,
            decimal price,
            decimal offPercentage,
            DateTime fromDate,
            DateTime? toDate,
            decimal quantity,
            int provider,
            IDbTransaction? transaction = null)
        {
            const string query = @"INSERT INTO [dbo].[ProductOffer]
                                ([Product], [Price], [OffPercentage], [FromDate], [ToDate], [Quantity], [Provider], [IsActive])
                                VALUES (@Product, @Price, @OffPercentage, @FromDate, @ToDate, @Quantity, @Provider, @IsActive)
                                SELECT CAST(SCOPE_IDENTITY() AS INT)";

            return await connection.ExecuteScalarAsync<int>(query, new
            {
                Product = product,
                Price = price,
                OffPercentage = offPercentage,
                FromDate = fromDate,
                ToDate = toDate,
                Quantity = quantity,
                Provider = provider,
                IsActive = true
            }, transaction);
        }

        public static async Task<bool> UpdateProductOfferAsync(IDbConnection connection,
            int id,
            int product,
            decimal price,
            decimal offPercentage,
            DateTime fromDate,
            DateTime? toDate,
            decimal quantity,
            int provider,
            bool isActive,
            IDbTransaction? transaction = null)
        {
            const string query = @"UPDATE [dbo].[ProductOffer]
                                    SET 
                                    [Product] = @Product,
                                    [Price] = @Price,
                                    [OffPercentage] = @OffPercentage,
                                    [FromDate] = @FromDate,
                                    [ToDate] = @ToDate,
                                    [Quantity] = @Quantity,
                                    [Provider] = @Provider,
                                    [IsActive] = @IsActive
                                    WHERE [Id] = @Id";

            var updatedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id,
                Product = product,
                Price = price,
                OffPercentage = offPercentage,
                FromDate = fromDate,
                ToDate = toDate,
                Quantity = quantity,
                Provider = provider,
                IsActive = isActive
            }, transaction);

            return updatedRowsCount > 0;
        }
        public static async Task<bool> UpdateProductOfferQuantityAsync(IDbConnection connection,
            int id,
            decimal quantity,
            IDbTransaction? transaction = null)
        {
            const string query = @"UPDATE [dbo].[ProductOffer]
                                    SET 
                                    [Quantity] = @Quantity
                                    WHERE [Id] = @Id";

            var updatedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id,
                Quantity = quantity
            }, transaction);

            return updatedRowsCount > 0;
        }
        public static async Task<bool> IncreaseProductOfferQuantityAsync(IDbConnection connection,
                    int id,
                    decimal quantity,
                    IDbTransaction? transaction = null)
        {
            const string query = @"UPDATE [dbo].[ProductOffer]
                            SET 
                            [Quantity] = [Quantity] + @Quantity
                            WHERE [Id] = @Id";

            var updatedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id,
                Quantity = quantity
            }, transaction);

            return updatedRowsCount > 0;
        }
        public static async Task<bool> DeleteProductOfferAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
        {
            const string query = @"DELETE FROM [dbo].[ProductOffer]
                                    WHERE [Id] = @Id";

            var deletedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id
            }, transaction);

            return deletedRowsCount > 0;
        }

        public static async Task<ProductOffer?> GetProductOfferByIdAsync(int id, IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [Id], 
                                    [Product], 
                                    [Price], 
                                    [OffPercentage], 
                                    [FromDate], 
                                    [ToDate], 
                                    [Quantity], 
                                    [Provider],
                                    [IsActive]
                                    FROM [dbo].[ProductOffer]
                                    WHERE [Id] = @Id";

            return await connection.QueryFirstOrDefaultAsync<ProductOffer>(query, new
            {
                Id = id
            });
        }
    }

    public class ProductOffer
    {
        public int Id { get; set; }
        public int Product { get; set; }
        public decimal Price { get; set; }
        public decimal OffPercentage { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal Quantity { get; set; }
        public int Provider { get; set; }
        public bool IsActive { get; set; }
    }
}
