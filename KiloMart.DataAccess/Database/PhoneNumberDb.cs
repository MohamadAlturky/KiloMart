using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
/// CREATE TABLE [dbo].[PhoneNumber](
/// [Id] [int] IDENTITY(1,1) NOT NULL,
/// [Value] [varchar](50) NOT NULL,
/// [Party] [int] NOT NULL,
/// [IsActive] [bit] NOT NULL
/// </summary>
public static partial class Db
{
    public static async Task<int> InsertPhoneNumberAsync(IDbConnection connection, string value, int party, IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[PhoneNumber]
                            ([Value], [Party],[IsActive])
                            VALUES (@Value, @Party,1)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Value = value,
            Party = party
        }, transaction);
    }

    public static async Task<bool> UpdatePhoneNumberAsync(IDbConnection connection, int id, string value, int party, bool isActive, IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[PhoneNumber]
                                SET 
                                [Value] = @Value,
                                [Party] = @Party,
                                [IsActive] = @IsActive
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Value = value,
            Party = party,
            IsActive = isActive
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeletePhoneNumberAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[PhoneNumber]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<PhoneNumber?> GetPhoneNumberByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Value], 
                            [Party], 
                            [IsActive]
                            FROM [dbo].[PhoneNumber]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<PhoneNumber>(query, new
        {
            Id = id
        });
    }

    public static async Task<PhoneNumber?> GetPhoneNumberByValueAsync(string value, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Value], 
                            [Party], 
                            [IsActive]
                            FROM [dbo].[PhoneNumber]
                            WHERE [Value] = @Value";

        return await connection.QueryFirstOrDefaultAsync<PhoneNumber>(query, new
        {
            Value = value
        });
    }
}

public class PhoneNumber
{
    public int Id { get; set; }
    public string Value { get; set; } = null!;
    public int Party { get; set; }
    public bool IsActive { get; set; }
}
