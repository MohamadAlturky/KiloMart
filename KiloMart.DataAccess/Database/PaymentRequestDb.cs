using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification for PaymentRequests
/// </summary>
public static partial class Db
{
    public static async Task<long> InsertPaymentRequestAsync(
        IDbConnection connection,
        PaymentRequestTable request,
        IDbTransaction? transaction = null)
    {
        const string query = @"
            INSERT INTO [dbo].[PaymentRequests]
            (
                [PayerCountry], [PayerAddress], [Action], [PayerZip], [PayerIp],
                [OrderCurrency], [PayerFirstName], [PayerCity], [Auth], [PayerLastName],
                [PayerPhone], [PayerEmail], [ReqToken], [RecurringInit], [TermUrl3ds],
                [CardExpYear], [CardExpMonth], [OrderId], [OrderIdInSystem], [CardCvv2],
                [OrderDescription], [CardNumber], [Hash], [OrderAmount]
            )
            VALUES 
            (
                @PayerCountry, @PayerAddress, @Action, @PayerZip, @PayerIp,
                @OrderCurrency, @PayerFirstName, @PayerCity, @Auth, @PayerLastName,
                @PayerPhone, @PayerEmail, @ReqToken, @RecurringInit, @TermUrl3ds,
                @CardExpYear, @CardExpMonth, @OrderId, @OrderIdInSystem, @CardCvv2,
                @OrderDescription, @CardNumber, @Hash, @OrderAmount
            );
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

        var id = await connection.ExecuteScalarAsync<long>(query, request, transaction);
        return id;
    }

    public static async Task<PaymentRequestTable?> GetPaymentRequestByIdAsync(
        long id,
        IDbConnection connection)
    {
        const string query = @"
            SELECT *
            FROM [dbo].[PaymentRequests]
            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<PaymentRequestTable>(query, new { Id = id });
    }

    public static async Task<PaymentRequestTable?> GetPaymentRequestByOrderIdAsync(
        string orderId,
        IDbConnection connection)
    {
        const string query = @"
            SELECT *
            FROM [dbo].[PaymentRequests]
            WHERE [OrderId] = @OrderId";

        return await connection.QueryFirstOrDefaultAsync<PaymentRequestTable>(query, new { OrderId = orderId });
    }

    public static async Task<IEnumerable<PaymentRequestTable>> GetPaymentRequestsByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        IDbConnection connection)
    {
        const string query = @"
            SELECT *
            FROM [dbo].[PaymentRequests]
            WHERE [CreatedAt] BETWEEN @StartDate AND @EndDate
            ORDER BY [CreatedAt] DESC";

        return await connection.QueryAsync<PaymentRequestTable>(query, new 
        { 
            StartDate = startDate,
            EndDate = endDate
        });
    }

    public static async Task<bool> UpdatePaymentRequestAsync(
        IDbConnection connection,
        PaymentRequestTable request,
        IDbTransaction? transaction = null)
    {
        const string query = @"
            UPDATE [dbo].[PaymentRequests]
            SET 
                [PayerCountry] = @PayerCountry,
                [PayerAddress] = @PayerAddress,
                [Action] = @Action,
                [PayerZip] = @PayerZip,
                [PayerIp] = @PayerIp,
                [OrderCurrency] = @OrderCurrency,
                [PayerFirstName] = @PayerFirstName,
                [PayerCity] = @PayerCity,
                [Auth] = @Auth,
                [PayerLastName] = @PayerLastName,
                [PayerPhone] = @PayerPhone,
                [PayerEmail] = @PayerEmail,
                [ReqToken] = @ReqToken,
                [RecurringInit] = @RecurringInit,
                [TermUrl3ds] = @TermUrl3ds,
                [CardExpYear] = @CardExpYear,
                [CardExpMonth] = @CardExpMonth,
                [OrderId] = @OrderId,
                [OrderIdInSystem] = @OrderIdInSystem,
                [CardCvv2] = @CardCvv2,
                [OrderDescription] = @OrderDescription,
                [CardNumber] = @CardNumber,
                [Hash] = @Hash,
                [OrderAmount] = @OrderAmount
            WHERE [Id] = @Id";

        var rowsAffected = await connection.ExecuteAsync(query, request, transaction);
        return rowsAffected > 0;
    }

    public static async Task<bool> DeletePaymentRequestAsync(
        IDbConnection connection,
        long id,
        IDbTransaction? transaction = null)
    {
        const string query = @"
            DELETE FROM [dbo].[PaymentRequests]
            WHERE [Id] = @Id";

        var rowsAffected = await connection.ExecuteAsync(query, new { Id = id }, transaction);
        return rowsAffected > 0;
    }
}

public class PaymentRequestTable
{
    public long Id { get; set; }
    public string PayerCountry { get; set; } = "SA";
    public string PayerAddress { get; set; } = "adnanh@expresspay.sa";
    public string Action { get; set; } = "SALE";
    public string PayerZip { get; set; } = "123221";
    public string PayerIp { get; set; } = "176.44.76.222";
    public string OrderCurrency { get; set; } = "SAR";
    public string PayerFirstName { get; set; } = "Adnan";
    public string PayerCity { get; set; } = "Riyadh";
    public string Auth { get; set; } = "N";
    public string PayerLastName { get; set; } = "Hashmi";
    public string PayerPhone { get; set; } = "966565897862";
    public string PayerEmail { get; set; } = "adnanh@expresspay.sa";
    public string ReqToken { get; set; } = "N";
    public string RecurringInit { get; set; } = "N";
    public string TermUrl3ds { get; set; } = "http://kilomart-001-site1.ptempurl.com/callbacks/success";
    public string CardExpYear { get; set; }
    public string CardExpMonth { get; set; }
    public string OrderId { get; set; }
    public long OrderIdInSystem { get; set; }
    public string CardCvv2 { get; set; }
    public string? OrderDescription { get; set; }
    public string CardNumber { get; set; }
    public string? Hash { get; set; }
    public decimal OrderAmount { get; set; }
    public DateTime CreatedAt { get; set; }
} 