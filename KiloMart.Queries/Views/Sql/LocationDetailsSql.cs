using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string LocationDetailsSql => @"
    SELECT
        [Id],
        [BuildingType],
        [BuildingNumber],
        [FloorNumber],
        [ApartmentNumber], 
        [StreetNumber],
        [PhoneNumber],
        [Location]
    FROM 
        dbo.[LocationDetails]";

    public static DbQuery<LocationDetailsSqlResponse> LocationDetailsSqlQuery 
    => new(LocationDetailsSql);
}

public class LocationDetailsSqlResponse
{
    public int Id { get; set; }
    public string BuildingType { get; set; } = null!;
    public string BuildingNumber { get; set; } = null!;
    public string FloorNumber { get; set; } = null!;
    public string ApartmentNumber { get; set; } = null!;
    public string StreetNumber { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public int Location { get; set; }
}
