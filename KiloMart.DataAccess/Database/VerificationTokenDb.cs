using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
//  CREATE TABLE [dbo].[VerificationToken](
//  [Id] [int] IDENTITY(1,1) NOT NULL,
//  [MembershipUser] [int] NOT NULL,
//  [Value] [varchar](50) NOT NULL,
//  [CreatedAt] [datetime] NOT NULL
/// </summary>

public static partial class Db
{
    public static async Task<int> InsertVerificationTokenAsync(IDbConnection connection,
        int membershipUser,
        string value,
        DateTime createdAt,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[VerificationToken]
                                ([MembershipUser], [Value], [CreatedAt])
                                VALUES (@MembershipUser, @Value, @CreatedAt)
                                SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            MembershipUser = membershipUser,
            Value = value,
            CreatedAt = createdAt
        }, transaction);
    }

    public static async Task<bool> UpdateVerificationTokenAsync(IDbConnection connection,
        int id,
        int membershipUser,
        string value,
        DateTime createdAt,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[VerificationToken]
                                SET 
                                [MembershipUser] = @MembershipUser,
                                [Value] = @Value,
                                [CreatedAt] = @CreatedAt
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            MembershipUser = membershipUser,
            Value = value,
            CreatedAt = createdAt
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteVerificationTokenAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[VerificationToken]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<VerificationToken?> GetVerificationTokenByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                                [Id], 
                                [MembershipUser], 
                                [Value], 
                                [CreatedAt]
                                FROM [dbo].[VerificationToken]
                                WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<VerificationToken>(query, new
        {
            Id = id
        });
    }
}

public class VerificationToken
{
    public int Id { get; set; }
    public int MembershipUser { get; set; }
    public string Value { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
