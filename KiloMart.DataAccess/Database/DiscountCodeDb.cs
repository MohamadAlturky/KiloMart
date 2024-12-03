using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database
{
    /// <summary>
    /// Table Specification
    //  CREATE TABLE [dbo].[DiscountCode](
    //  [Id] [int] IDENTITY(1,1) NOT NULL,
    //  [Code] [varchar](200) NOT NULL,
    //  [Value] [decimal](18, 5) NOT NULL,
    //  [Description] [varchar](200) NOT NULL,
    //  [StartDate] [datetime] NOT NULL,
    //  [EndDate] [datetime] NULL,
    //  [DiscountType] [tinyint] NOT NULL,
    //  [IsActive] [bit] NOT NULL)
    /// </summary>
    public static partial class Db
    {
        public static async Task<int> InsertDiscountCodeAsync(IDbConnection connection,
            string code,
            decimal value,
            string description,
            DateTime startDate,
            DateTime? endDate,
            byte discountType,
            IDbTransaction? transaction = null)
        {
            const string query = @"INSERT INTO [dbo].[DiscountCode]
                                    ([Code], [Value], [Description], [StartDate], [EndDate], [DiscountType], [IsActive])
                                    VALUES (@Code, @Value, @Description, @StartDate, @EndDate, @DiscountType, @IsActive)
                                    SELECT CAST(SCOPE_IDENTITY() AS INT)";

            return await connection.ExecuteScalarAsync<int>(query, new
            {
                Code = code,
                Value = value,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                DiscountType = discountType,
                IsActive = true
            }, transaction);
        }

        public static async Task<bool> UpdateDiscountCodeAsync(IDbConnection connection,
            int id,
            string code,
            decimal value,
            string description,
            DateTime startDate,
            DateTime? endDate,
            byte discountType,
            bool isActive,
            IDbTransaction? transaction = null)
        {
            const string query = @"UPDATE [dbo].[DiscountCode]
                                    SET 
                                    [Code] = @Code,
                                    [Value] = @Value,
                                    [Description] = @Description,
                                    [StartDate] = @StartDate,
                                    [EndDate] = @EndDate,
                                    [DiscountType] = @DiscountType,
                                    [IsActive] = @IsActive
                                    WHERE [Id] = @Id";

            var updatedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id,
                Code = code,
                Value = value,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                DiscountType = discountType,
                IsActive = isActive
            }, transaction);

            return updatedRowsCount > 0;
        }

        public static async Task<bool> DeleteDiscountCodeAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
        {
            const string query = @"DELETE FROM [dbo].[DiscountCode]
                                    WHERE [Id] = @Id";

            var deletedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id
            }, transaction);

            return deletedRowsCount > 0;
        }

        public static async Task<DiscountCode?> GetDiscountCodeByIdAsync(int id, IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [Id], 
                                    [Code], 
                                    [Value], 
                                    [Description], 
                                    [StartDate], 
                                    [EndDate], 
                                    [DiscountType], 
                                    [IsActive]
                                    FROM [dbo].[DiscountCode]
                                    WHERE [Id] = @Id";

            return await connection.QueryFirstOrDefaultAsync<DiscountCode>(query, new
            {
                Id = id
            });
        }
        public static async Task<DiscountCode?> GetDiscountCodeByCodeAsync(string code, IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [Id], 
                                    [Code], 
                                    [Value], 
                                    [Description], 
                                    [StartDate], 
                                    [EndDate], 
                                    [DiscountType], 
                                    [IsActive]
                                    FROM [dbo].[DiscountCode]
                                    WHERE [Code] = @Code";

            return await connection.QueryFirstOrDefaultAsync<DiscountCode>(query, new
            {
                Code = code
            });
        }
    }

    public class DiscountCode
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public decimal Value { get; set; }
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public byte DiscountType { get; set; }
        public bool IsActive { get; set; }
    }
}
