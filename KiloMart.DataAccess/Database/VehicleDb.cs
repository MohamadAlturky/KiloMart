using System.Data;
using Dapper;

namespace KiloMart.DataAccess.Database;
//  CREATE TABLE [dbo].[Vehicle](
// 	[Id] [int] IDENTITY(1,1) NOT NULL,
// 	[Number] [varchar](200) NOT NULL,
// 	[Model] [varchar](200) NOT NULL,
// 	[Type] [varchar](200) NOT NULL,
// 	[Year] [varchar](200) NOT NULL,
// 	[Delivary] [int] NOT NULL)

/// <summary>
/// Table Specification
/// </summary>
/// 
public static partial class Db
{
    public static async Task<int> InsertVehicleAsync(IDbConnection connection,
        string number,
        string model,
        string type,
        string year,
        int delivary,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[Vehicle]
                            ([Number], [Model], [Type], [Year], [Delivary])
                            VALUES (@Number, @Model, @Type, @Year, @Delivary)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Number = number,
            Model = model,
            Type = type,
            Year = year,
            Delivary = delivary
        }, transaction);
    }

    public static async Task<bool> UpdateVehicleAsync(IDbConnection connection,
        int id, 
        string number, 
        string model, 
        string type, 
        string year,
        int delivary,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Vehicle]
                                SET 
                                [Number] = @Number,
                                [Model] = @Model,
                                [Type] = @Type,
                                [Year] = @Year,
                                [Delivary] = @Delivary
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Number = number,
            Model = model,
            Type = type,
            Year = year,
            Delivary = delivary
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteVehicleAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Vehicle]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<Vehicle?> GetVehicleByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Number], 
                            [Model], 
                            [Type], 
                            [Year], 
                            [Delivary]
                            FROM [dbo].[Vehicle]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<Vehicle>(query, new
        {
            Id = id
        });
    }
}

public class Vehicle
{
    public int Id { get; set; }
    public string Number { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Year { get; set; } = null!;
    public int Delivary { get; set; }
}
