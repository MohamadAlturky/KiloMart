using Dapper;
using System.Data;
using System.Text;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification for DeliveryActivity
/// </summary>
public static partial class Db
{
    public static async Task<long> InsertDeliveryActivityAsync(IDbConnection connection,
        DateTime date,
        decimal value,
        byte type,
        int delivery,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[DeliveryActivity]
                            ([Date], [Value], [Type], [Delivery])
                            VALUES (@Date, @Value, @Type, @Delivery)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            Date = date,
            Value = value,
            Type = type,
            Delivery = delivery
        }, transaction);
    }

    public static async Task<bool> UpdateDeliveryActivityAsync(IDbConnection connection,
        long id,
        DateTime date,
        float value,
        byte type,
        int delivery,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[DeliveryActivity]
                                SET 
                                [Date] = @Date,
                                [Value] = @Value,
                                [Type] = @Type,
                                [Delivery] = @Delivery
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Date = date,
            Value = value,
            Type = type,
            Delivery = delivery
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteDeliveryActivityAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[DeliveryActivity]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<DeliveryActivity?> GetDeliveryActivityByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Date], 
                            [Value], 
                            [Type], 
                            [Delivery]
                            FROM [dbo].[DeliveryActivity]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<DeliveryActivity>(query, new
        {
            Id = id
        });
    }


    public static async Task<IEnumerable<DeliveryActivity>> GetDeliveryActivityFilteredAsync(
        IDbConnection connection,
        int delivery,
        byte? type = null,
        DateTime? startdate = null,
        DateTime? enddate = null)
    {
        var sqlBuilder = new StringBuilder();
        sqlBuilder.AppendLine("SELECT");
        sqlBuilder.AppendLine("    [Id],");
        sqlBuilder.AppendLine("    [Date],");
        sqlBuilder.AppendLine("    [Value],");
        sqlBuilder.AppendLine("    [Type],");
        sqlBuilder.AppendLine("    [Delivery]");
        sqlBuilder.AppendLine("FROM [dbo].[DeliveryActivity]");
        sqlBuilder.AppendLine("WHERE Delivery = @Delivery");

        var parameters = new DynamicParameters();
        parameters.Add("@Delivery", delivery);

        if (type.HasValue)
        {
            sqlBuilder.AppendLine("AND [Type] = @Type");
            parameters.Add("@Type", type.Value);
        }

        if (startdate.HasValue)
        {
            sqlBuilder.AppendLine("AND [Date] >= @StartDate");
            parameters.Add("@StartDate", startdate.Value);
        }

        if (enddate.HasValue)
        {
            sqlBuilder.AppendLine("AND [Date] <= @EndDate");
            parameters.Add("@EndDate", enddate.Value);
        }

        return await connection.QueryAsync<DeliveryActivity>(
            sqlBuilder.ToString(),
            parameters);
    }



    public static async Task<IEnumerable<DeliveryActivity>> GetDeliveryActivitiesByDeliveryIdAsync(int deliveryId, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Date], 
                            [Value], 
                            [Type], 
                            [Delivery]
                            FROM [dbo].[DeliveryActivity]
                            WHERE [Delivery] = @DeliveryId
                            ORDER BY [Id] DESC";

        return await connection.QueryAsync<DeliveryActivity>(query, new
        {
            DeliveryId = deliveryId
        });
    }
    public static async Task<IEnumerable<DeliveryActivity>> GetDeliveryActivitiesByDateBetweenAndDeliveryAsync(
    DateTime startDate,
    DateTime endDate,
    int deliveryId,
    IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Date], 
                            [Value], 
                            [Type], 
                            [Delivery]
                            FROM [dbo].[DeliveryActivity]
                            WHERE [Date] BETWEEN @StartDate AND @EndDate
                            AND [Delivery] = @DeliveryId
                            ORDER BY [Id] DESC";

        return await connection.QueryAsync<DeliveryActivity>(query, new
        {
            StartDate = startDate,
            EndDate = endDate,
            DeliveryId = deliveryId
        });
    }

    public static async Task<IEnumerable<DeliveryActivity>> GetDeliveryActivitiesByDateBiggerAndDeliveryAsync(
        DateTime date,
        int deliveryId,
        IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Date], 
                            [Value], 
                            [Type], 
                            [Delivery]
                            FROM [dbo].[DeliveryActivity]
                            WHERE [Date] > @Date
                            AND [Delivery] = @DeliveryId
                            ORDER BY [Id] DESC";

        return await connection.QueryAsync<DeliveryActivity>(query, new
        {
            Date = date,
            DeliveryId = deliveryId
        });
    }
}

public class DeliveryActivity
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public float Value { get; set; }
    public byte Type { get; set; }
    public int Delivery { get; set; }
}