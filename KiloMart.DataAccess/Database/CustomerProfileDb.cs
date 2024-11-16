using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database
{
    /// <summary>
    /// Table Specification
    //  CREATE TABLE [dbo].[CustomerProfile](
    // 	[Id] [int] IDENTITY(1,1) NOT NULL,
    // 	[Customer] [int] NOT NULL,
    // 	[FirstName] [varchar](200) NOT NULL,
    // 	[SecondName] [varchar](200) NOT NULL,
    // 	[NationalName] [varchar](200) NOT NULL,
    // 	[NationalId] [varchar](200) NOT NULL)
    /// </summary>
    public static partial class Db
    {
        public static async Task<int> InsertCustomerProfileAsync(IDbConnection connection,
            int customer,
            string firstName,
            string secondName,
            string nationalName,
            string nationalId,
            IDbTransaction? transaction = null)
        {
            const string query = @"INSERT INTO [dbo].[CustomerProfile]
                                ([Customer], [FirstName], [SecondName], [NationalName], [NationalId])
                                VALUES (@Customer, @FirstName, @SecondName, @NationalName, @NationalId)
                                SELECT CAST(SCOPE_IDENTITY() AS INT)";

            return await connection.ExecuteScalarAsync<int>(query, new
            {
                Customer = customer,
                FirstName = firstName,
                SecondName = secondName,
                NationalName = nationalName,
                NationalId = nationalId
            }, transaction);
        }

        public static async Task<bool> UpdateCustomerProfileAsync(IDbConnection connection,
            int id,
            int customer,
            string firstName,
            string secondName,
            string nationalName,
            string nationalId,
            IDbTransaction? transaction = null)
        {
            const string query = @"UPDATE [dbo].[CustomerProfile]
                                SET 
                                [Customer] = @Customer,
                                [FirstName] = @FirstName,
                                [SecondName] = @SecondName,
                                [NationalName] = @NationalName,
                                [NationalId] = @NationalId
                                WHERE [Id] = @Id";

            var updatedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id,
                Customer = customer,
                FirstName = firstName,
                SecondName = secondName,
                NationalName = nationalName,
                NationalId = nationalId
            }, transaction);

            return updatedRowsCount > 0;
        }

        public static async Task<bool> DeleteCustomerProfileAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
        {
            const string query = @"DELETE FROM [dbo].[CustomerProfile]
                                WHERE [Id] = @Id";

            var deletedRowsCount = await connection.ExecuteAsync(query, new
            {
                Id = id
            }, transaction);

            return deletedRowsCount > 0;
        }

        public static async Task<CustomerProfile?> GetCustomerProfileByIdAsync(int id, IDbConnection connection)
        {
            const string query = @"SELECT 
                                [Id], 
                                [Customer], 
                                [FirstName], 
                                [SecondName], 
                                [NationalName], 
                                [NationalId]
                                FROM [dbo].[CustomerProfile]
                                WHERE [Id] = @Id";

            return await connection.QueryFirstOrDefaultAsync<CustomerProfile>(query, new
            {
                Id = id
            });
        }
        public static async Task<CustomerProfile?> GetCustomerProfileByCustomerIdAsync(int id, IDbConnection connection)
        {
            const string query = @"SELECT 
                                [Id], 
                                [Customer], 
                                [FirstName], 
                                [SecondName], 
                                [NationalName], 
                                [NationalId]
                                FROM [dbo].[CustomerProfile]
                                WHERE [Customer] = @Id";

            return await connection.QueryFirstOrDefaultAsync<CustomerProfile>(query, new
            {
                Id = id
            });
        }
    }

    public class CustomerProfile
    {
        public int Id { get; set; }
        public int Customer { get; set; }
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public string NationalName { get; set; } = null!;
        public string NationalId { get; set; } = null!;
    }
}
