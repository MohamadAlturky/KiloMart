using Dapper;
using KiloMart.Authentication.Models;
using System.Data;


namespace KiloMart.Authentication.Services;

public static class RoleService
{
    public static async Task<int> AddAsync(IDbConnection dbConnection, Role role)
    {
        var sql = @"
            INSERT INTO Roles (Name) 
            OUTPUT INSERTED.Id 
            VALUES (@Name)";
        return await dbConnection.ExecuteScalarAsync<int>(sql, role);
    }

    public static async Task UpdateAsync(IDbConnection dbConnection, Role role)
    {
        var sql = "UPDATE Roles SET Name = @Name WHERE Id = @Id";
        await dbConnection.ExecuteAsync(sql, role);
    }

    public static async Task DeleteAsync(IDbConnection dbConnection, Role role)
    {
        var sql = "DELETE FROM Roles WHERE Id = @Id";
        await dbConnection.ExecuteAsync(sql, new { role.Id });
    }

    public static async Task<Role?> GetAsync(IDbConnection dbConnection, int roleId)
    {
        var sql = "SELECT Id, Name FROM Roles WHERE Id = @RoleId";
        return await dbConnection.QuerySingleOrDefaultAsync<Role>(sql, new { RoleId = roleId });
    }
}
