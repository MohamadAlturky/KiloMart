using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
//  CREATE TABLE [dbo].[MembershipUser](
// 	[Id] [int] IDENTITY(1,1) NOT NULL,
// 	[Email] [varchar](256) NOT NULL,
// 	[EmailConfirmed] [bit] NOT NULL,
// 	[PasswordHash] [varchar](max) NOT NULL,
// 	[Role] [tinyint] NOT NULL,
// 	[Party] [int] NOT NULL,
// 	[IsActive] [bit] NOT NULL)
/// </summary>
public static partial class Db
{
    public static async Task<int> InsertMembershipUserAsync(IDbConnection connection,
        string email,
        bool emailConfirmed,
        string passwordHash,
        byte role,
        int party,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[MembershipUser]
                            ([Email], [EmailConfirmed], [PasswordHash], [Role], [Party])
                            VALUES (@Email, @EmailConfirmed, @PasswordHash, @Role, @Party)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Email = email,
            EmailConfirmed = emailConfirmed,
            PasswordHash = passwordHash,
            Role = role,
            Party = party
        }, transaction);
    }

    public static async Task<bool> UpdateMembershipUserAsync(IDbConnection connection,
        int id,
        string email,
        bool emailConfirmed,
        string passwordHash,
        byte role,
        int party,
        bool isActive,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[MembershipUser]
                                SET 
                                [Email] = @Email,
                                [EmailConfirmed] = @EmailConfirmed,
                                [PasswordHash] = @PasswordHash,
                                [Role] = @Role,
                                [Party] = @Party,
                                [IsActive] = @IsActive
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Email = email,
            EmailConfirmed = emailConfirmed,
            PasswordHash = passwordHash,
            Role = role,
            Party = party,
            IsActive = isActive
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteMembershipUserAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[MembershipUser]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<MembershipUser?> GetMembershipUserByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Email], 
                            [EmailConfirmed], 
                            [PasswordHash], 
                            [Role], 
                            [Party], 
                            [IsActive]
                            FROM [dbo].[MembershipUser]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<MembershipUser>(query, new
        {
            Id = id
        });
    }

    public static async Task<MembershipUser?> GetMembershipUserByEmailAsync(string email, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Email], 
                            [EmailConfirmed], 
                            [PasswordHash], 
                            [Role], 
                            [Party], 
                            [IsActive]
                            FROM [dbo].[MembershipUser]
                            WHERE [Email] = @Email";

        return await connection.QueryFirstOrDefaultAsync<MembershipUser>(query, new
        {
            Email = email
        });
    }
}

public class MembershipUser
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = null!;
    public byte Role { get; set; }
    public int Party { get; set; }
    public bool IsActive { get; set; }
}
