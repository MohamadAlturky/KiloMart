using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string WithdrawSql => @"
    SELECT
        [Id],
        [Delivery],
        [BankAccountNumber],
        [IBanNumber],
        [Date],
        [Done]
    FROM 
        dbo.[Withdraw]";

    public static DbQuery<WithdrawSqlResponse> WithdrawSqlQuery 
    => new(WithdrawSql);
}

public class WithdrawSqlResponse
{
    public long Id { get; set; }
    public int Delivery { get; set; }
    public string BankAccountNumber { get; set; } = null!;
    public string IBanNumber { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool Done { get; set; }
}
