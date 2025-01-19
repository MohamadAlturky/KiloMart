using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;

namespace KiloMart.Domain.Register.Activate;


public class UserAccountService
{
    public static async Task<bool> ActivateUser(string email, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();

        var rowsAffected = await connection.ExecuteAsync(
            "UPDATE MembershipUser SET IsActive = 1 WHERE Email = @Email",
            new { Email = email }
        );

        return rowsAffected > 0;
    }

    public static async Task<bool> DeActivateUser(string email, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var membershipUser = await Db.GetMembershipUserByEmailAsync(connection, email);
        if (membershipUser is null)
        {
            throw new Exception("user not found");
        }
        var rowsAffected = await connection.ExecuteAsync(
            "UPDATE MembershipUser SET IsActive = 0, IsDeleted = 1 WHERE Email = @Email",
            new { Email = membershipUser.Email }
        );
        rowsAffected = await connection.ExecuteAsync(
            "UPDATE Party SET IsActive = 0 WHERE Id = @Id",
            new { Id = membershipUser.Party }
        );
        return rowsAffected > 0;
    }

    public static async Task<bool> ActivateUser(int id, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();

        var rowsAffected = await connection.ExecuteAsync(
            "UPDATE MembershipUser SET IsActive = 1 WHERE Id = @Id",
            new { Id = id }
        );

        return rowsAffected > 0;
    }

    public static async Task<bool> DeActivateUser(int id, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var membershipUser = await Db.GetMembershipUserByIdAsync(connection, id);
        if (membershipUser is null)
        {
            throw new Exception("user not found");
        }
        var rowsAffected = await connection.ExecuteAsync(
            "UPDATE MembershipUser SET IsActive = 0, IsDeleted = 1 WHERE Email = @Email",
            new { Email = membershipUser.Email }
        );
        rowsAffected = await connection.ExecuteAsync(
            "UPDATE Party SET IsActive = 0 WHERE Id = @Id",
            new { Id = membershipUser.Party }
        );
        return rowsAffected > 0;
        // using var connection = dbFactory.CreateDbConnection();
        // connection.Open();

        // var rowsAffected = await connection.ExecuteAsync(
        //     "UPDATE MembershipUser SET IsActive = 0 WHERE Id = @Id",
        //     new { Id = id }
        // );

        // return rowsAffected > 0;
    }

}
