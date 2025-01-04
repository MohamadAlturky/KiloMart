using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string MembershipUserSql => @"
    SELECT
        [Id],
        [Email],
        [EmailConfirmed],
        [PasswordHash],
        [Role],
        [Party],
        [IsActive],
        [Language],
        [IsDeleted]
    FROM 
        dbo.[MembershipUser]";

    public static DbQuery<MembershipUserSqlResponse> MembershipUserSqlQuery 
    => new(MembershipUserSql);
}

public class MembershipUserSqlResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; } = null!;
    public byte Role { get; set; }
    public int Party { get; set; }
    public bool IsActive { get; set; }
    public byte Language { get; set; }
    public bool? IsDeleted { get; set; }
}
