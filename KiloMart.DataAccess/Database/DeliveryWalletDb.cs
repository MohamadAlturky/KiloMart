using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
/// </summary>
public static partial class Db
{
    public static async Task<int> InsertDeliveryWalletAsync(IDbConnection connection,
        decimal value,
        int delivery,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[DeliveryWallet]
                            ([Value], [Delivery])
                            VALUES (@Value, @Delivery)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            Value = value,
            Delivery = delivery
        }, transaction);
    }

    public static async Task<bool> UpdateDeliveryWalletAsync(IDbConnection connection,
        int id,
        decimal value,
        int delivery,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[DeliveryWallet]
                                SET 
                                [Value] = @Value,
                                [Delivery] = @Delivery
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            Value = value,
            Delivery = delivery
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteDeliveryWalletAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[DeliveryWallet]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<DeliveryWallet?> GetDeliveryWalletByIdAsync(int id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Value], 
                            [Delivery]
                            FROM [dbo].[DeliveryWallet]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<DeliveryWallet>(query, new
        {
            Id = id
        });
    }
    public static async Task<DeliveryWallet?> GetDeliveryWalletByDeliveryIdAsync(int deliveryId, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [Value], 
                            [Delivery]
                            FROM [dbo].[DeliveryWallet]
                            WHERE [Delivery] = @DeliveryId";

        return await connection.QueryFirstOrDefaultAsync<DeliveryWallet>(query, new
        {
            DeliveryId = deliveryId
        });
    }
}

public class DeliveryWallet
{
    public int Id { get; set; }
    public decimal Value { get; set; }
    public int Delivery { get; set; }
}