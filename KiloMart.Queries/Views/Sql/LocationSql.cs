using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string LocationSql => @"
    SELECT
        [Id],
        [Longitude],
        [Latitude],
        [Name],
        [Party],
        [IsActive]
    FROM 
        dbo.[Location]";

    public static DbQuery<LocationSqlResponse> LocationSqlQuery 
    => new(LocationSql);
}

public class LocationSqlResponse
{
    public int Id { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string Name { get; set; } = null!;
    public int Party { get; set; }
    public bool IsActive { get; set; }
}
