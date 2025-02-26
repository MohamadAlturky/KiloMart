using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification for PaymentResponses
/// </summary>
public static partial class Db
{
    public static async Task<long> InsertPaymentResponseAsync(
        IDbConnection connection,
        PaymentResponseTable response,
        IDbTransaction? transaction = null)
    {
        const string query = @"
            INSERT INTO [dbo].[PaymentResponses]
            (
                [PaymentRequestId], [Action], [Result], [Status], [OrderId], [TransId],
                [TransDate], [Amount], [Currency], [RedirectUrl], [RedirectParams],
                [RedirectMethod], [ErrorCode], [ErrorMessage], [Errors], [Body]
            )
            VALUES
            (
                @PaymentRequestId, @Action, @Result, @Status, @OrderId, @TransId,
                @TransDate, @Amount, @Currency, @RedirectUrl, @RedirectParams,
                @RedirectMethod, @ErrorCode, @ErrorMessage, @Errors, @Body
            );
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        var id = await connection.ExecuteScalarAsync<long>(query, response, transaction);
        return id;
    }

    public static async Task<PaymentResponseTable?> GetPaymentResponseByIdAsync(
        long id,
        IDbConnection connection)
    {
        const string query = @"
            SELECT *
            FROM [dbo].[PaymentResponses]
            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<PaymentResponseTable>(query, new { Id = id });
    }

    public static async Task<PaymentResponseTable?> GetPaymentResponseByOrderIdAsync(
        string orderId,
        IDbConnection connection)
    {
        const string query = @"
            SELECT *
            FROM [dbo].[PaymentResponses]
            WHERE [OrderId] = @OrderId";

        return await connection.QueryFirstOrDefaultAsync<PaymentResponseTable>(query, new { OrderId = orderId });
    }

    public static async Task<PaymentResponseTable?> GetPaymentResponseByTransIdAsync(
        string transId,
        IDbConnection connection)
    {
        const string query = @"
            SELECT *
            FROM [dbo].[PaymentResponses]
            WHERE [TransId] = @TransId";

        return await connection.QueryFirstOrDefaultAsync<PaymentResponseTable>(query, new { TransId = transId });
    }

    public static async Task<IEnumerable<PaymentResponseTable>> GetPaymentResponsesByStatusAsync(
        string status,
        IDbConnection connection)
    {
        const string query = @"
            SELECT *
            FROM [dbo].[PaymentResponses]
            WHERE [Status] = @Status
            ORDER BY [Id] DESC";

        return await connection.QueryAsync<PaymentResponseTable>(query, new { Status = status });
    }

    public static async Task<bool> UpdatePaymentResponseAsync(
        IDbConnection connection,
        PaymentResponseTable response,
        IDbTransaction? transaction = null)
    {
        const string query = @"
            UPDATE [dbo].[PaymentResponses]
            SET 
                [PaymentRequestId] = @PaymentRequestId,
                [Action] = @Action,
                [Result] = @Result,
                [Status] = @Status,
                [OrderId] = @OrderId,
                [TransId] = @TransId,
                [TransDate] = @TransDate,
                [Amount] = @Amount,
                [Currency] = @Currency,
                [RedirectUrl] = @RedirectUrl,
                [RedirectParams] = @RedirectParams,
                [RedirectMethod] = @RedirectMethod,
                [ErrorCode] = @ErrorCode,
                [ErrorMessage] = @ErrorMessage,
                [Errors] = @Errors,
                [Body] = @Body
            WHERE [Id] = @Id";

        var rowsAffected = await connection.ExecuteAsync(query, response, transaction);
        return rowsAffected > 0;
    }

    public static async Task<bool> DeletePaymentResponseAsync(
        IDbConnection connection,
        long id,
        IDbTransaction? transaction = null)
    {
        const string query = @"
            DELETE FROM [dbo].[PaymentResponses]
            WHERE [Id] = @Id";

        var rowsAffected = await connection.ExecuteAsync(query, new { Id = id }, transaction);
        return rowsAffected > 0;
    }
}

public class PaymentResponseTable
{
    public long Id { get; set; }
    public long PaymentRequestId { get; set; }
    public string? Action { get; set; }
    public string? Result { get; set; }
    public string? Status { get; set; }
    public string? OrderId { get; set; }
    public string? TransId { get; set; }
    public string? TransDate { get; set; }
    public string? Amount { get; set; }
    public string? Currency { get; set; }
    public string? RedirectUrl { get; set; }
    public string? RedirectParams { get; set; }
    public string? RedirectMethod { get; set; }
    public int? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Errors { get; set; }
    public string? Body { get; set; }
} 