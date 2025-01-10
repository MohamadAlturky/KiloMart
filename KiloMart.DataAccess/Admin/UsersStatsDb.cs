using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Admin;

public static partial class Stats
{

    public static async Task<long> GetActiveCustomersCountAsync(
        IDbConnection connection)
    {
        const string query = @"
                    SELECT COUNT(u.Party) AS TotalCount
                FROM dbo.[Customer] u
                INNER JOIN dbo.[Party] p ON u.Party = p.Id
                INNER JOIN dbo.[MembershipUser] m ON m.Party = u.Party
                WHERE m.[IsDeleted] = 0 OR m.[IsDeleted] IS NULL";

        return await connection.ExecuteScalarAsync<int>(query);
    }
    public static async Task<long> GetDeletedCustomersCountAsync(
        IDbConnection connection)
    {
        const string query = @"
                    SELECT COUNT(u.Party) AS TotalCount
                FROM dbo.[Customer] u
                INNER JOIN dbo.[Party] p ON u.Party = p.Id
                INNER JOIN dbo.[MembershipUser] m ON m.Party = u.Party
                WHERE m.[IsDeleted] = 1";

        return await connection.ExecuteScalarAsync<int>(query);

    }

    public static async Task<long> GetActiveProvidersCountAsync(
        IDbConnection connection)
    {
        const string query = @"
                    SELECT COUNT(u.Party) AS TotalCount
                FROM dbo.[Provider] u
                INNER JOIN dbo.[Party] p ON u.Party = p.Id
                INNER JOIN dbo.[MembershipUser] m ON m.Party = u.Party
                WHERE m.[IsDeleted] = 0 OR m.[IsDeleted] IS NULL";

        return await connection.ExecuteScalarAsync<int>(query);

    }
    public static async Task<long> GetDeletedProvidersCountAsync(
        IDbConnection connection)
    {
        const string query = @"
                    SELECT COUNT(u.Party) AS TotalCount
                FROM dbo.[Provider] u
                INNER JOIN dbo.[Party] p ON u.Party = p.Id
                INNER JOIN dbo.[MembershipUser] m ON m.Party = u.Party
                WHERE m.[IsDeleted] = 1";

        return await connection.ExecuteScalarAsync<int>(query);

    }
    public static async Task<long> GetActiveDeliveriesCountAsync(
        IDbConnection connection)
    {
        const string query = @"
                    SELECT COUNT(u.Party) AS TotalCount
                FROM dbo.[Delivery] u
                INNER JOIN dbo.[Party] p ON u.Party = p.Id
                INNER JOIN dbo.[MembershipUser] m ON m.Party = u.Party
                WHERE m.[IsDeleted] = 0 OR m.[IsDeleted] IS NULL";
        return await connection.ExecuteScalarAsync<int>(query);

    }
    public static async Task<long> GetDeletedDeliveriesCountAsync(
        IDbConnection connection)
    {
        const string query = @"
                    SELECT COUNT(u.Party) AS TotalCount
                FROM dbo.[Delivery] u
                INNER JOIN dbo.[Party] p ON u.Party = p.Id
                INNER JOIN dbo.[MembershipUser] m ON m.Party = u.Party
                WHERE m.[IsDeleted] = 1";
        return await connection.ExecuteScalarAsync<int>(query);

    }
}