using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification for PaymentTransactions
/// </summary>
public static partial class Db
{
    public static async Task<long> InsertPaymentTransactionAsync(
        IDbConnection connection,
        PaymentTransactionTable transaction,
        IDbTransaction? dbTransaction = null)
    {
        const string query = @"
            INSERT INTO [dbo].[PaymentTransactions]
            (
                [Action], [Result], [Status], [OrderId], [TransactionId],
                [Hash], [TransactionDate], [RecurringToken], [ScheduleId],
                [CardToken], [Card], [CardExpirationDate], [Descriptor],
                [Amount], [Currency], [DeclineReason], [RedirectUrl],
                [RedirectParams], [RedirectMethod]
            )
            VALUES
            (
                @Action, @Result, @Status, @OrderId, @TransactionId,
                @Hash, @TransactionDate, @RecurringToken, @ScheduleId,
                @CardToken, @Card, @CardExpirationDate, @Descriptor,
                @Amount, @Currency, @DeclineReason, @RedirectUrl,
                @RedirectParams, @RedirectMethod
            );
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        var id = await connection.ExecuteScalarAsync<long>(query, transaction, dbTransaction);
        return id;
    }

    public static async Task<PaymentTransactionTable?> GetPaymentTransactionByIdAsync(
        long id,
        IDbConnection connection)
    {
        const string query = @"
            SELECT *
            FROM [dbo].[PaymentTransactions]
            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<PaymentTransactionTable>(query, new { Id = id });
    }

    public static async Task<PaymentTransactionTable?> GetPaymentTransactionByOrderIdAsync(
        string orderId,
        IDbConnection connection)
    {
        const string query = @"
            SELECT *
            FROM [dbo].[PaymentTransactions]
            WHERE [OrderId] = @OrderId";

        return await connection.QueryFirstOrDefaultAsync<PaymentTransactionTable>(query, new { OrderId = orderId });
    }

    public static async Task<PaymentTransactionTable?> GetPaymentTransactionByTransactionIdAsync(
        string transactionId,
        IDbConnection connection)
    {
        const string query = @"
            SELECT *
            FROM [dbo].[PaymentTransactions]
            WHERE [TransactionId] = @TransactionId";

        return await connection.QueryFirstOrDefaultAsync<PaymentTransactionTable>(query, new { TransactionId = transactionId });
    }

    public static async Task<IEnumerable<PaymentTransactionTable>> GetPaymentTransactionsByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        IDbConnection connection)
    {
        const string query = @"
            SELECT *
            FROM [dbo].[PaymentTransactions]
            WHERE [TransactionDate] BETWEEN @StartDate AND @EndDate
            ORDER BY [TransactionDate] DESC";

        return await connection.QueryAsync<PaymentTransactionTable>(query, new 
        { 
            StartDate = startDate,
            EndDate = endDate
        });
    }

    public static async Task<IEnumerable<PaymentTransactionTable>> GetPaymentTransactionsByStatusAsync(
        string status,
        IDbConnection connection)
    {
        const string query = @"
            SELECT *
            FROM [dbo].[PaymentTransactions]
            WHERE [Status] = @Status
            ORDER BY [TransactionDate] DESC";

        return await connection.QueryAsync<PaymentTransactionTable>(query, new { Status = status });
    }

    public static async Task<bool> UpdatePaymentTransactionAsync(
        IDbConnection connection,
        PaymentTransactionTable transaction,
        IDbTransaction? dbTransaction = null)
    {
        const string query = @"
            UPDATE [dbo].[PaymentTransactions]
            SET 
                [Action] = @Action,
                [Result] = @Result,
                [Status] = @Status,
                [OrderId] = @OrderId,
                [TransactionId] = @TransactionId,
                [Hash] = @Hash,
                [TransactionDate] = @TransactionDate,
                [RecurringToken] = @RecurringToken,
                [ScheduleId] = @ScheduleId,
                [CardToken] = @CardToken,
                [Card] = @Card,
                [CardExpirationDate] = @CardExpirationDate,
                [Descriptor] = @Descriptor,
                [Amount] = @Amount,
                [Currency] = @Currency,
                [DeclineReason] = @DeclineReason,
                [RedirectUrl] = @RedirectUrl,
                [RedirectParams] = @RedirectParams,
                [RedirectMethod] = @RedirectMethod
            WHERE [Id] = @Id";

        var rowsAffected = await connection.ExecuteAsync(query, transaction, dbTransaction);
        return rowsAffected > 0;
    }

    public static async Task<bool> DeletePaymentTransactionAsync(
        IDbConnection connection,
        long id,
        IDbTransaction? dbTransaction = null)
    {
        const string query = @"
            DELETE FROM [dbo].[PaymentTransactions]
            WHERE [Id] = @Id";

        var rowsAffected = await connection.ExecuteAsync(query, new { Id = id }, dbTransaction);
        return rowsAffected > 0;
    }
}

public class PaymentTransactionTable
{
    public long Id { get; set; }
    public string Action { get; set; } = "SALE";
    public string? Result { get; set; }
    public string? Status { get; set; }
    public string? OrderId { get; set; }
    public string? TransactionId { get; set; }
    public string? Hash { get; set; }
    public DateTime? TransactionDate { get; set; }
    
    // Fields from PaymentTransactionResponse
    public string? RecurringToken { get; set; }
    public string? ScheduleId { get; set; }
    public string? CardToken { get; set; }
    public string? Card { get; set; }
    public string? CardExpirationDate { get; set; }
    public string? Descriptor { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    
    // Field from PaymentTransactionDeclinedResponse
    public string? DeclineReason { get; set; }
    
    // Fields from PaymentTransactionRedirectResponse
    public string? RedirectUrl { get; set; }
    public string? RedirectParams { get; set; }
    public string? RedirectMethod { get; set; }
    
    public DateTime CreatedAt { get; set; }
} 