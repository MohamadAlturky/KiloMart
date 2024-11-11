using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
//  CREATE TABLE [dbo].[Location](
// 	[Id] [int] IDENTITY(1,1) NOT NULL,
// 	[Longitude] [float] NOT NULL,
// 	[Latitude] [float] NOT NULL,
// 	[Name] [varchar](200) NOT NULL,
// 	[Party] [int] NOT NULL,
// 	[IsActive] [bit] NOT NULL
/// </summary>
///
public static partial class Db
{
    public static async Task<int> InsertLocationAsync(IDbConnection connection,
        float longitude,
        float latitude,
        string name,
        int party,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[Location]
                            ([Longitude], [Latitude], [Name], [Party])
                            VALUES (@Longitude, @Latitude, @Name, @Party)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Longitude = longitude,
            Latitude = latitude,
            Name = name,
            Party = party
        }, transaction);
    }

    public static async Task<bool> UpdateLocationAsync(IDbConnection connection,
        int id,
        float longitude,
        float latitude,
        string name,
        int party,
        bool isActive,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Location]
                                SET 
                                [Longitude] = @Longitude,
                                [Latitude] = @Latitude,
                                [Name] = @Name,
                                [Party] = @Party,
                                [IsActive] = @IsActive
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Longitude = longitude,
            Latitude = latitude,
            Name = name,
            Party = party,
            IsActive = isActive
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteLocationAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Location]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<Location?> GetLocationByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Longitude], 
                            [Latitude], 
                            [Name], 
                            [Party], 
                            [IsActive]
                            FROM [dbo].[Location]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<Location>(query, new
        {
            Id = id
        });
    }
}

public class Location
{
    public int Id { get; set; }
    public float Longitude { get; set; }
    public float Latitude { get; set; }
    public string Name { get; set; } = null!;
    public int Party { get; set; }
    public bool IsActive { get; set; }
}
