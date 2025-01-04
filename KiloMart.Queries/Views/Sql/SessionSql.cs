using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string SessionSql => @"
    SELECT
        [Id],
        [Token],
        [UserId],
        [ExpireDate],
        [CreationDate],
        [Code]
    FROM 
        dbo.[Sessions]";

    public static DbQuery<SessionSqlResponse> SessionSqlQuery 
    => new(SessionSql);
}

public class SessionSqlResponse
{
    public long Id { get; set; }
    public string Token { get; set; } = null!;
    public int UserId { get; set; }
    public DateTime ExpireDate { get; set; }
    public DateTime CreationDate { get; set; }
    public string Code { get; set; } = null!;
}
