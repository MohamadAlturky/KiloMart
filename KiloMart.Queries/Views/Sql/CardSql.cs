using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string CardSql => @"
    SELECT
        [Id],
        [HolderName],
        [Number],
        [SecurityCode],
        [ExpireDate],
        [Customer],
        [IsActive]
    FROM 
        dbo.[Card]";

    public static DbQuery<CardSqlResponse> CardSqlQuery 
    => new(CardSql);
}

public class CardSqlResponse
{
    public int Id { get; set; }
    public string HolderName { get; set; } = null!;
    public string Number { get; set; } = null!;
    public string SecurityCode { get; set; } = null!;
    public DateTime ExpireDate { get; set; }
    public int Customer { get; set; }
    public bool IsActive { get; set; }
}
