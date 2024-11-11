using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database
{
    /// <summary>
    /// Table Specification
    /// CREATE TABLE [dbo].[Product](
    /// [Id] [int] IDENTITY(1,1) NOT NULL,
    /// [ImageUrl] [varchar](300) NOT NULL,
    /// [ProductCategory] [int] NOT NULL,
    /// [IsActive] [bit] NOT NULL,
    /// [MeasurementUnit] [varchar](200) NOT NULL,
    /// [Description] [varchar](300) NOT NULL,
    /// [Name] [varchar](200) NOT NULL
    /// </summary>
    public static partial class Db
    {
        public static async Task<int> InsertProductAsync(IDbConnection connection,
            string imageUrl,
            int productCategory,
            bool isActive,
            string measurementUnit,
            string description,
            string name,
            IDbTransaction? transaction = null)
        {
            const string query = @"INSERT INTO [dbo].[Product]
                                    ([ImageUrl], [ProductCategory], [IsActive], [MeasurementUnit], [Description], [Name])
                                    VALUES (@ImageUrl, @ProductCategory, @IsActive, @MeasurementUnit, @Description, @Name)
                                    SELECT CAST(SCOPE_IDENTITY() AS INT)";

            return await connection.ExecuteScalarAsync<int>(query, new
            {
                ImageUrl = imageUrl,
                ProductCategory = productCategory,
                IsActive = isActive,
                MeasurementUnit = measurementUnit,
                Description = description,
                Name = name
            }, transaction);
        }

        public static async Task<bool> UpdateProductAsync(IDbConnection connection,
            int id,
            string imageUrl,
            int productCategory,
            bool isActive,
            string measurementUnit,
            string description,
            string name,
            IDbTransaction? transaction = null)
        {
            const string query = @"UPDATE [dbo].[Product]
                                    SET 
                                    [ImageUrl] = @ImageUrl,
                                    [ProductCategory] = @ProductCategory,
                                    [IsActive] = @IsActive,
                                    [MeasurementUnit] = @MeasurementUnit,
                                    [Description] = @Description,
                                    [Name] = @Name
                                    WHERE [Id] = @Id";

            var updatedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id,
                ImageUrl = imageUrl,
                ProductCategory = productCategory,
                IsActive = isActive,
                MeasurementUnit = measurementUnit,
                Description = description,
                Name = name
            }, transaction);

            return updatedRowsCount > 0;
        }

        public static async Task<bool> DeleteProductAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
        {
            const string query = @"DELETE FROM [dbo].[Product]
                                    WHERE [Id] = @Id";

            var deletedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id
            }, transaction);

            return deletedRowsCount > 0;
        }

        public static async Task<Product?> GetProductByIdAsync(int id, IDbConnection connection)
        {
            const string query = @"SELECT 
                                [Id], 
                                [ImageUrl], 
                                [ProductCategory], 
                                [IsActive], 
                                [MeasurementUnit], 
                                [Description], 
                                [Name]
                                FROM [dbo].[Product]
                                WHERE [Id] = @Id";

            return await connection.QueryFirstOrDefaultAsync<Product>(query, new
            {
                Id = id
            });
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int ProductCategory { get; set; }
        public bool IsActive { get; set; }
        public string MeasurementUnit { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
