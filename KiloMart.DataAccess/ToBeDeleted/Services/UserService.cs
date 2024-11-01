using Dapper;
using KiloMart.Authentication.Models;
using System.Data;


namespace KiloMart.Authentication.Services;

public static class UserService
{
    public static async Task<int> AddAsync(IDbConnection dbConnection, User user)
    {
        var sql = @"
            INSERT INTO Users (Name, Email, HashedPassword, IsActive) 
            OUTPUT INSERTED.Id 
            VALUES (@Name, @Email, @HashedPassword, @IsActive)";
        return await dbConnection.ExecuteScalarAsync<int>(sql, user);
    }

    public static async Task UpdateAsync(IDbConnection dbConnection, User user)
    {
        var sql = "UPDATE Users SET Name = @Name, Email = @Email, HashedPassword = @HashedPassword, IsActive = @IsActive WHERE Id = @Id";
        await dbConnection.ExecuteAsync(sql, user);
    }

    public static async Task DeleteAsync(IDbConnection dbConnection, User user)
    {
        var sql = "DELETE FROM Users WHERE Id = @Id";
        await dbConnection.ExecuteAsync(sql, new { user.Id });
    }

    public static async Task<User?> GetAsync(IDbConnection dbConnection, int userId)
    {
        var sql = "SELECT Id, Name, Email, HashedPassword, IsActive FROM Users WHERE Id = @UserId";
        return await dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { UserId = userId });
    }

    public static async Task<IEnumerable<Role>> GetRolesAsync(IDbConnection dbConnection, int userId)
    {
        var sql = @"
            SELECT R.Id, R.Name 
            FROM Roles R
            INNER JOIN UserRoles UR ON R.Id = UR.Role 
            WHERE UR.User = @UserId";
        return await dbConnection.QueryAsync<Role>(sql, new { UserId = userId });
    }

    public static async Task<User?> GetByEmailAsync(IDbConnection dbConnection, string email)
    {
        var sql = "SELECT Id, Name, Email, HashedPassword, IsActive FROM Users WHERE Email = @Email";
        return await dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email });
    }

}


//write the login service 
//- use the user service and userrole service to add to the database
//- hash the password with security best practices
//- generate the jwt token 


//write the register service
//- use the user service and userrole service to add to the database
//- hash the password with security best practices
//- send an otp using gmail