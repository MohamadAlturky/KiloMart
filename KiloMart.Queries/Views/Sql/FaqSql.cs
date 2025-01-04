using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string FaqSql => @"
    SELECT
        [Id],
        [Question],
        [Answer],
        [Language],
        [Type]
    FROM 
        dbo.[FAQ]";

    public static DbQuery<FaqSqlResponse> FaqSqlQuery 
    => new(FaqSql);
}

public class FaqSqlResponse
{
    public int Id { get; set; }
    public string Question { get; set; } = null!;
    public string Answer { get; set; } = null!;
    public byte Language { get; set; }
    public byte Type { get; set; }
}
