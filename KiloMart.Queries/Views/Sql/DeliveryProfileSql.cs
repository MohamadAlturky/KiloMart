using KiloMart.Core.Sql;

namespace KiloMart.Queries.Views.Sql;

public partial class Sql 
{
    public static string DeliveryProfileSql => @"
    SELECT
        [Id],
        [Delivary],
        [FirstName],
        [SecondName],
        [NationalName],
        [NationalId],
        [LicenseNumber],
        [LicenseExpiredDate],
        [DrivingLicenseNumber],
        [DrivingLicenseExpiredDate]
    FROM 
        dbo.[DelivaryProfile]";

    public static DbQuery<DeliveryProfileSqlResponse> DeliveryProfileSqlQuery 
    => new(DeliveryProfileSql);
}

public class DeliveryProfileSqlResponse
{
    public int Id { get; set; }
    public int Delivary { get; set; }
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string NationalName { get; set; } = null!;
    public string NationalId { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public DateTime LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; } = null!;
    public DateTime DrivingLicenseExpiredDate { get; set; }
}
