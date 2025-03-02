using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database
{
    public static partial class Db
    {
        public static async Task InsertProductRequestDataLocalizedAsync(IDbConnection connection,
            int productRequest,
            short language,
            string name,
            string description,
            string measurementUnit,
            IDbTransaction? transaction = null)
        {
            const string query = @"INSERT INTO [dbo].[ProductRequestDataLocalized]
                                ([ProductRequest], [Language], [Name], [Description], [MeasurementUnit])
                                VALUES (@ProductRequest, @Language, @Name, @Description, @MeasurementUnit);";

            await connection.ExecuteAsync(query, new
            {
                ProductRequest = productRequest,
                Language = language,
                Name = name,
                Description = description,
                MeasurementUnit = measurementUnit
            }, transaction);
        }

        public static async Task<bool> UpdateProductRequestDataLocalizedAsync(IDbConnection connection,
            int productRequest,
            short language,
            string name,
            string description,
            string measurementUnit,
            IDbTransaction? transaction = null)
        {
            const string query = @"UPDATE [dbo].[ProductRequestDataLocalized]
                                    SET 
                                    [Name] = @Name,
                                    [Description] = @Description,
                                    [MeasurementUnit] = @MeasurementUnit
                                    WHERE [ProductRequest] = @ProductRequest AND [Language] = @Language";

            var updatedRowsCount = await connection.ExecuteAsync(query, new
            {
                ProductRequest = productRequest,
                Language = language,
                Name = name,
                Description = description,
                MeasurementUnit = measurementUnit
            }, transaction);

            return updatedRowsCount > 0;
        }

        public static async Task<bool> DeleteProductRequestDataLocalizedAsync(IDbConnection connection,
            int productRequest,
            short language,
            IDbTransaction? transaction = null)
        {
            const string query = @"DELETE FROM [dbo].[ProductRequestDataLocalized]
                                    WHERE [ProductRequest] = @ProductRequest AND [Language] = @Language";

            var deletedRowsCount = await connection.ExecuteAsync(query, new
            {
                ProductRequest = productRequest,
                Language = language
            }, transaction);

            return deletedRowsCount > 0;
        }
        public static async Task<bool> DeleteProductRequestDataLocalizedAsync(IDbConnection connection,
                    int productRequest,
                    IDbTransaction? transaction = null)
        {
            const string query = @"DELETE FROM [dbo].[ProductRequestDataLocalized]
                                    WHERE [ProductRequest] = @ProductRequest";

            var deletedRowsCount = await connection.ExecuteAsync(query, new
            {
                ProductRequest = productRequest
            }, transaction);

            return deletedRowsCount > 0;
        }

        public static async Task<ProductRequestDataLocalized?> GetProductRequestDataLocalizedAsync(int productRequest,
            short language,
            IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [ProductRequest], 
                                    [Language], 
                                    [Name], 
                                    [Description], 
                                    [MeasurementUnit]
                                    FROM [dbo].[ProductRequestDataLocalized]
                                    WHERE [ProductRequest] = @ProductRequest AND [Language] = @Language";

            return await connection.QueryFirstOrDefaultAsync<ProductRequestDataLocalized>(query, new
            {
                ProductRequest = productRequest,
                Language = language
            });
        }
        public static async Task<ProductRequestDataLocalized?> GetProductRequestDataLocalizedAsync(int productRequest,
            IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [ProductRequest], 
                                    [Language], 
                                    [Name], 
                                    [Description], 
                                    [MeasurementUnit]
                                    FROM [dbo].[ProductRequestDataLocalized]
                                    WHERE [ProductRequest] = @ProductRequest";

            return await connection.QueryFirstOrDefaultAsync<ProductRequestDataLocalized>(query, new
            {
                ProductRequest = productRequest
            });
        }
        public static async Task<IEnumerable<ProductRequestDataLocalized>> GetProductRequestsDataLocalizedAsync(int productRequest,
            IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [ProductRequest], 
                                    [Language], 
                                    [Name], 
                                    [Description], 
                                    [MeasurementUnit]
                                    FROM [dbo].[ProductRequestDataLocalized]
                                    WHERE [ProductRequest] = @ProductRequest";

            return await connection.QueryAsync<ProductRequestDataLocalized>(query, new
            {
                ProductRequest = productRequest
            });
        }
        public static async Task<List<ProductRequestDataLocalized>> ListProductRequestDataLocalizedAsync(int productRequest,
            IDbConnection connection)
        {
            const string query = @"SELECT 
                                    [ProductRequest], 
                                    [Language], 
                                    [Name], 
                                    [Description], 
                                    [MeasurementUnit]
                                    FROM [dbo].[ProductRequestDataLocalized]
                                    WHERE [ProductRequest] = @ProductRequest";

            var result = await connection.QueryAsync<ProductRequestDataLocalized>(query, new
            {
                ProductRequest = productRequest
            });

            return result.ToList();
        }
    }

    public class ProductRequestDataLocalized
    {
        public int ProductRequest { get; set; }
        public short Language { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string MeasurementUnit { get; set; } = null!;
    }
}
