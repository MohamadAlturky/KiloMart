using Dapper;
using KiloMart.Authentication.Models;
using System.Data;


namespace KiloMart.Authentication.Services;

public static class UserRoleService
{
    public static async Task<int> AddAsync(IDbConnection dbConnection, UserRole userRole)
    {
        var sql = @"
            INSERT INTO UserRoles (User, Role) 
            OUTPUT INSERTED.Id 
            VALUES (@User, @Role)";
        return await dbConnection.ExecuteScalarAsync<int>(sql, userRole);
    }

    public static async Task UpdateAsync(IDbConnection dbConnection, UserRole userRole)
    {
        var sql = "UPDATE UserRoles SET User = @User, Role = @Role WHERE Id = @Id";
        await dbConnection.ExecuteAsync(sql, userRole);
    }

    public static async Task DeleteAsync(IDbConnection dbConnection, UserRole userRole)
    {
        var sql = "DELETE FROM UserRoles WHERE Id = @Id";
        await dbConnection.ExecuteAsync(sql, new { userRole.Id });
    }

    public static async Task<UserRole?> GetAsync(IDbConnection dbConnection, int userRoleId)
    {
        var sql = "SELECT Id, User, Role FROM UserRoles WHERE Id = @UserRoleId";
        return await dbConnection.QuerySingleOrDefaultAsync<UserRole>(sql, new { UserRoleId = userRoleId });
    }
}
