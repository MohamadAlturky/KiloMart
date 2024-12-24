using Dapper;
using Microsoft.Identity.Client;
using System.Data;

namespace KiloMart.Domain.Orders.DataAccess;

/// <summary>
/// CREATE TABLE[dbo].[Order]
/// (
///     [Id][bigint] IDENTITY(1,1) NOT NULL,
///     [OrderStatus] [tinyint] NOT NULL,
///     [TotalPrice] [money] NOT NULL,
///     [TransactionId] [varchar] (50) NOT NULL
/// );
/// </summary>
public static partial class OrdersDb
{
    public static async Task<long> InsertOrderAsync(IDbConnection connection,
        byte orderStatus,
        decimal totalPrice,
        decimal itemsPrice,
        decimal systemFee,
        decimal deliveryFee,
        string transactionId,
        DateTime date,
        bool isPaid,
        byte paymentType,
        string? specialRequest,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[Order]
                            ([OrderStatus], [TotalPrice], [TransactionId], [Date],[PaymentType],[IsPaid],[DeliveryFee],[SystemFee],[ItemsPrice],[SpecialRequest])
                            VALUES (@OrderStatus, @TotalPrice, @TransactionId, @Date, @PaymentType, @IsPaid,@DeliveryFee,@SystemFee,@ItemsPrice,@SpecialRequest)
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        return await connection.ExecuteScalarAsync<long>(query, new
        {
            OrderStatus = orderStatus,
            TotalPrice = totalPrice,
            TransactionId = transactionId,
            Date = date,
            PaymentType = paymentType,
            IsPaid = isPaid,
            ItemsPrice = itemsPrice,
            SystemFee = systemFee,
            DeliveryFee = deliveryFee,
            SpecialRequest = specialRequest
        }, transaction);
    }

    public static async Task<bool> UpdateOrderAsync(IDbConnection connection,
        long id,
        byte orderStatus,
        decimal totalPrice, 
        decimal itemsPrice,
        decimal systemFee,
        decimal deliveryFee,
        string transactionId,
        DateTime date,
        byte paymentType,
        bool isPaid,
        string? specialRequest,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Order]
                                SET 
                                [OrderStatus] = @OrderStatus,
                                [TotalPrice] = @TotalPrice,
                                [TransactionId] = @TransactionId,
                                [Date] = @Date,
                                [ItemsPrice] = @ItemsPrice,
                                [SystemFee] = @SystemFee,
                                [DeliveryFee] = @DeliveryFee,
                                [IsPaid] = @IsPaid,
                                [PaymentType] = @PaymentType,
                                [SpecialRequest] = @SpecialRequest
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            OrderStatus = orderStatus,
            TotalPrice = totalPrice,
            TransactionId = transactionId,
            Date = date,
            PaymentType = paymentType,
            IsPaid = isPaid,
            ItemsPrice = itemsPrice,
            SystemFee = systemFee,
            DeliveryFee = deliveryFee,
            SpecialRequest = specialRequest
        }, transaction);

        return updatedRowsCount > 0;
    }
    public static async Task<bool> UpdateOrderPaymentAsync(IDbConnection connection,
        long id,
        byte paymentType,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Order]
                                SET 
                                [PaymentType] = @PaymentType
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            PaymentType = paymentType
        }, transaction);

        return updatedRowsCount > 0;
    }
    public static async Task<bool> UpdateOrderIsPaidAsync(IDbConnection connection,
            long id,
            bool isPaid,
            IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Order]
                                SET 
                                [IsPaid] = @IsPaid
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            IsPaid = isPaid
        }, transaction);

        return updatedRowsCount > 0;
    }
    public static async Task<bool> DeleteOrderAsync(IDbConnection connection, long id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Order]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<Order?> GetOrderByIdAsync(long id, IDbConnection connection)
    {
        const string query = @"SELECT 
                            [Id], 
                            [OrderStatus], 
                            [TotalPrice], 
                            [TransactionId],
                            [Date],
                            [PaymentType],
                            [IsPaid],
                            [DeliveryFee],
                            [SystemFee],
                            [ItemsPrice],
                            [SpecialRequest]
                            FROM [dbo].[Order]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<Order>(query, new
        {
            Id = id
        });
    }
}

public class Order
{
    public long Id { get; set; }
    public byte OrderStatus { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal ItemsPrice { get; set; }
    public decimal SystemFee { get; set; }
    public decimal DeliveryFee { get; set; }
    public string TransactionId { get; set; } = null!;
    public string? SpecialRequest { get; set; }
    public DateTime Date { get; set; }
    public byte PaymentType { get; set; }
    public bool IsPaid { get; set; }
}