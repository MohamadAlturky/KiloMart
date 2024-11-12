using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Vehicles.Models;

namespace KiloMart.Domain.Vehicles.DataAccess;

public static class VehicleRepository
{
    public static async Task InsertVehicle(Vehicle model, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var query = @"
        INSERT INTO [dbo].[Vehicle] ([Number], [Model], [Type], [Year], [Delivary])
            VALUES (@Number, @Model, @Type, @Year, @Delivary)";
        await connection.ExecuteAsync(query, model);
    }

    public static async Task UpdateVehicle(Vehicle model, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var query = @"
        UPDATE [dbo].[Vehicle]
        SET [Number] = @Number, [Model] = @Model, [Type] = @Type, [Year] = @Year, [Delivary] = @Delivary
        WHERE [Id] = @Id";
        await connection.ExecuteAsync(query, model);
    }

    public static async Task<Vehicle?> GetVehicleById(int id, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var query = "SELECT * FROM [dbo].[Vehicle] WHERE [Id] = @Id";
        var result = await connection.QueryFirstOrDefaultAsync<Vehicle>(query, new { Id = id });
        return result;
    }

    public static async Task<List<Vehicle>> GetVehiclesByDelivery(int deliveryId, IDbFactory dbFactory)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        var query = "SELECT * FROM [dbo].[Vehicle] WHERE [Delivary] = @Delivary";
        var result = await connection.QueryAsync<Vehicle>(query, new { Delivary = deliveryId });
        return result.ToList();
    }
}
