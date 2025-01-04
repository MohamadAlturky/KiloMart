using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string DriverFreeFeeSql => @"
    SELECT
        [Id],
        [StartDate],
        [EndDate],
        [IsActive]
    FROM 
        dbo.[DriverFreeFee]";

    public static DbQuery<DriverFreeFeeSqlResponse> DriverFreeFeeSqlQuery 
    => new(DriverFreeFeeSql);
}

public class DriverFreeFeeSqlResponse
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}
