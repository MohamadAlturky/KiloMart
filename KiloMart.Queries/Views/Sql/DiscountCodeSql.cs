using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string DiscountCodeSql => @"
    SELECT
        [Id],
        [Code],
        [Value],
        [Description],
        [StartDate],
        [EndDate],
        [DiscountType],
        [IsActive]
    FROM 
        dbo.[DiscountCode]";

    public static DbQuery<DiscountCodeSqlResponse> DiscountCodeSqlQuery 
    => new(DiscountCodeSql);
}

public class DiscountCodeSqlResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public decimal Value { get; set; }
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public byte DiscountType { get; set; }
    public bool IsActive { get; set; }
}
