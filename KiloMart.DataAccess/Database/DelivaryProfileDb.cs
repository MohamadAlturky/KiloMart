using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
//  CREATE TABLE [dbo].[DelivaryProfile](
// 	[Id] [int] IDENTITY(1,1) NOT NULL,
// 	[Delivary] [int] NOT NULL,
// 	[FirstName] [varchar](200) NOT NULL,
// 	[SecondName] [varchar](200) NOT NULL,
// 	[NationalName] [varchar](200) NOT NULL,
// 	[NationalId] [varchar](200) NOT NULL,
// 	[LicenseNumber] [varchar](200) NOT NULL,
// 	[LicenseExpiredDate] [date] NOT NULL,
// 	[DrivingLicenseNumber] [varchar](200) NOT NULL,
// 	[DrivingLicenseExpiredDate] [date] NOT NULL
/// </summary>
/// 
public static partial class Db
{
    public static async Task<int> InsertDelivaryProfileAsync(IDbConnection connection,
        int delivary,
        string firstName,
        string secondName,
        string nationalName,
        string nationalId,
        string licenseNumber,
        DateOnly licenseExpiredDate,
        string drivingLicenseNumber,
        DateOnly drivingLicenseExpiredDate,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[DelivaryProfile]
                                ([Delivary], [FirstName], [SecondName], [NationalName], [NationalId], [LicenseNumber], [LicenseExpiredDate], [DrivingLicenseNumber], [DrivingLicenseExpiredDate])
                                VALUES (@Delivary, @FirstName, @SecondName, @NationalName, @NationalId, @LicenseNumber, @LicenseExpiredDate, @DrivingLicenseNumber, @DrivingLicenseExpiredDate)
                                SELECT CAST(SCOPE_IDENTITY() AS INT)";
        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Delivary = delivary,
            FirstName = firstName,
            SecondName = secondName,
            NationalName = nationalName,
            NationalId = nationalId,
            LicenseNumber = licenseNumber,
            LicenseExpiredDate = licenseExpiredDate,
            DrivingLicenseNumber = drivingLicenseNumber,
            DrivingLicenseExpiredDate = drivingLicenseExpiredDate
        }, transaction);
    }

    public static async Task<bool> UpdateDelivaryProfileAsync(IDbConnection connection,
        int id,
        int delivary,
        string firstName,
        string secondName,
        string nationalName,
        string nationalId,
        string licenseNumber,
        DateOnly licenseExpiredDate,
        string drivingLicenseNumber,
        DateOnly drivingLicenseExpiredDate,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[DelivaryProfile]
                                SET
                                [Delivary] = @Delivary,
                                [FirstName] = @FirstName,
                                [SecondName] = @SecondName,
                                [NationalName] = @NationalName,
                                [NationalId] = @NationalId,
                                [LicenseNumber] = @LicenseNumber,
                                [LicenseExpiredDate] = @LicenseExpiredDate,
                                [DrivingLicenseNumber] = @DrivingLicenseNumber,
                                [DrivingLicenseExpiredDate] = @DrivingLicenseExpiredDate
                                WHERE [Id] = @Id";
        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Delivary = delivary,
            FirstName = firstName,
            SecondName = secondName,
            NationalName = nationalName,
            NationalId = nationalId,
            LicenseNumber = licenseNumber,
            LicenseExpiredDate = licenseExpiredDate,
            DrivingLicenseNumber = drivingLicenseNumber,
            DrivingLicenseExpiredDate = drivingLicenseExpiredDate
        }, transaction);
        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteDelivaryProfileAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[DelivaryProfile]
                                WHERE [Id] = @Id";
        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);
        return deletedRowsCount > 0;
    }

    public static async Task<DelivaryProfile?> GetDelivaryProfileByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
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
                                FROM [dbo].[DelivaryProfile]
                                WHERE [Id] = @Id";
        return await connection.QueryFirstOrDefaultAsync<DelivaryProfile>(query, new
        {
            Id = id
        });
    }
}

public class DelivaryProfile
{
    public int Id { get; set; }
    public int Delivary { get; set; }
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string NationalName { get; set; } = null!;
    public string NationalId { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public DateOnly LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; } = null!;
    public DateOnly DrivingLicenseExpiredDate { get; set; }
}
