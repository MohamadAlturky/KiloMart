using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string NotificationSql => @"
    SELECT
        [Id],
        [Title],
        [Content],
        [Date],
        [ForParty],
        [JsonPayLoad],
        [IsRead]
    FROM 
        dbo.[Notification]";

    public static DbQuery<NotificationSqlResponse> NotificationSqlQuery 
    => new(NotificationSql);
}

public class NotificationSqlResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime Date { get; set; }
    public int ForParty { get; set; }
    public string JsonPayLoad { get; set; } = null!;
    public bool IsRead { get; set; }
}
