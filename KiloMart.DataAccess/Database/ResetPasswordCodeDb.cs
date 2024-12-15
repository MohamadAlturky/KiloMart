using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
/// CREATE TABLE [dbo].[ResetPasswordCode](
///     [Id] [bigint] IDENTITY(1,1) NOT NULL,
///     [Code] [varchar](50) NOT NULL,
///     [MembershipUser] [int] NOT NULL,
///     [Date] [datetime] NOT NULL
/// )
/// </summary>
public static partial class Db
{
    public static async Task<long> InsertResetPasswordCodeAsync(IDbConnection connection,
        string code,
        int membershipUser,
        DateTime date,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[ResetPasswordCode]
                            ([Code], [MembershipUser], [Date])
                            VALUES (@Code, @MembershipUser, @Date)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Code = code,
            MembershipUser = membershipUser,
            Date = date
        }, transaction);
    }

    public static async Task<bool> UpdateResetPasswordCodeAsync(IDbConnection connection,
        long id,
        string code,
        int membershipUser,
        DateTime date,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[ResetPasswordCode]
                                SET 
                                [Code] = @Code,
                                [MembershipUser] = @MembershipUser,
                                [Date] = @Date
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Code = code,
            MembershipUser = membershipUser,
            Date = date
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteResetPasswordCodeAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[ResetPasswordCode]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<ResetPasswordCode?> GetResetPasswordCodeByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Code], 
                            [MembershipUser], 
                            [Date]
                            FROM [dbo].[ResetPasswordCode]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<ResetPasswordCode>(query, new
        {
            Id = id
        });
    }
    public static async Task<ResetPasswordCode?> GetResetPasswordCodeByCodeAsync(
            string code,
            int membershipUser,
            IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Code], 
                            [MembershipUser], 
                            [Date]
                            FROM [dbo].[ResetPasswordCode]
                            WHERE [Code] = @code
                            AND [MembershipUser] = @membershipUser  
                            AND [Date] >= DATEADD(HOUR, -1, GETDATE())";

        return await connection.QueryFirstOrDefaultAsync<ResetPasswordCode>(query, new
        {
            code,
            membershipUser
        });
    }
}

public class ResetPasswordCode
{
    public long Id { get; set; }
    public string Code { get; set; } = null!;
    public int MembershipUser { get; set; }
    public DateTime Date { get; set; }
}
