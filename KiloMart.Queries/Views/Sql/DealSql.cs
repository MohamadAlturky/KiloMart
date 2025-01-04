using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string DealSql => @"
    SELECT
        [Id],
        [Product],
        [IsActive],
        [OffPercentage],
        [StartDate],
        [EndDate]
    FROM 
        dbo.[Deal]";

    public static DbQuery<DealSqlResponse> DealSqlQuery 
    => new(DealSql);
}

public class DealSqlResponse
{
    public int Id { get; set; }
    public int Product { get; set; }
    public bool IsActive { get; set; }
    public double OffPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
