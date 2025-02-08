using Dapper;
using System.Data;
using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database
{
    public static partial class Db
    {
        // The new base query with the extra joins and selected columns.
        private const string BaseWithdrawDetailsVwQuery = @"
SELECT 
    wd.[Id],
    pp.Party AS ProviderId,
    dd.Party AS DeliveryId,
    wd.[BankAccountNumber],
    wd.[IBanNumber],
    wd.[Date],
    wd.[Done],
    wd.[Accepted],
    wd.[Rejected],
    p.[Id] AS PartyId,
    p.[DisplayName] AS PartyDisplayName,
    p.[IsActive] AS PartyIsActive,
    (SELECT SUM(da.[Value]) 
     FROM dbo.DeliveryActivity da 
     WHERE da.Type = 1 AND da.Delivery = wd.[Party]) AS DeliveryActiveBalanceReceives,
    (SELECT SUM(da.[Value]) 
     FROM dbo.DeliveryActivity da 
     WHERE da.Type = 2 AND da.Delivery = wd.[Party]) AS DeliveryActiveBalanceDeductions,
    (SELECT SUM(da.[Value]) 
     FROM dbo.ProviderActivity da 
     WHERE da.Type = 1 AND da.[Provider] = wd.[Party]) AS ProviderActiveBalanceReceives,
    (SELECT SUM(da.[Value]) 
     FROM dbo.ProviderActivity da 
     WHERE da.Type = 2 AND da.[Provider] = wd.[Party]) AS ProviderActiveBalanceDeductions
FROM [dbo].[Withdraw] wd
INNER JOIN dbo.[Party] p ON wd.Party = p.Id
LEFT JOIN dbo.Provider pp ON pp.Party = p.Id
LEFT JOIN dbo.Delivery dd ON dd.Party = p.Id";

        /// <summary>
        /// Retrieves a Withdraw record (with joined Party, Provider, Delivery data and calculated balances) by its Id.
        /// </summary>
        public static async Task<WithdrawDetailsVw?> GetWithdrawDetailsVwByIdAsync(long id, IDbConnection connection)
        {
            var query = BaseWithdrawDetailsVwQuery + @"
WHERE wd.[Id] = @Id";

            return await connection.QueryFirstOrDefaultAsync<WithdrawDetailsVw>(query, new { Id = id });
        }

        /// <summary>
        /// Retrieves a list of Withdraw records filtered by Party.
        /// </summary>
        public static async Task<IEnumerable<WithdrawDetailsVw>> GetWithdrawDetailsVwByPartyAsync(int party, IDbConnection connection)
        {
            var query = BaseWithdrawDetailsVwQuery + @"
WHERE p.[Id] = @Party";

            return await connection.QueryAsync<WithdrawDetailsVw>(query, new { Party = party });
        }

        /// <summary>
        /// Retrieves a list of Withdraw records filtered by Done status.
        /// </summary>
        public static async Task<IEnumerable<WithdrawDetailsVw>> GetWithdrawDetailsVwByDoneAsync(bool done, IDbConnection connection)
        {
            var query = BaseWithdrawDetailsVwQuery + @"
WHERE wd.[Done] = @Done";

            return await connection.QueryAsync<WithdrawDetailsVw>(query, new { Done = done });
        }

        /// <summary>
        /// Retrieves a list of Withdraw records filtered by Rejected status.
        /// </summary>
        public static async Task<IEnumerable<WithdrawDetailsVw>> GetWithdrawDetailsVwByRejectedAsync(bool rejected, IDbConnection connection)
        {
            var query = BaseWithdrawDetailsVwQuery + @"
WHERE wd.[Rejected] = @Rejected";

            return await connection.QueryAsync<WithdrawDetailsVw>(query, new { Rejected = rejected });
        }

        /// <summary>
        /// Retrieves a list of Withdraw records filtered by both Party and Done status.
        /// </summary>
        public static async Task<IEnumerable<WithdrawDetailsVw>> GetWithdrawDetailsVwByPartyAndDoneAsync(int party, bool done, IDbConnection connection)
        {
            var query = BaseWithdrawDetailsVwQuery + @"
WHERE p.[Id] = @Party AND wd.[Done] = @Done";

            return await connection.QueryAsync<WithdrawDetailsVw>(query, new { Party = party, Done = done });
        }

        /// <summary>
        /// Retrieves a paginated list of Withdraw records filtered by Party along with the total count.
        /// </summary>
        public static async Task<(IEnumerable<WithdrawDetailsVw> Withdraws, int TotalCount)> GetPaginatedWithdrawDetailsVwByPartyAsync(int party, int pageNumber, int pageSize, IDbConnection connection)
        {
            var withdrawQuery = BaseWithdrawDetailsVwQuery + @"
WHERE p.[Id] = @Party
ORDER BY wd.[Id] DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var countQuery = @"
SELECT COUNT(*) 
FROM [dbo].[Withdraw] wd
INNER JOIN dbo.[Party] p ON wd.Party = p.Id
WHERE p.[Id] = @Party";

            var parameters = new
            {
                Party = party,
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            };

            var withdraws = await connection.QueryAsync<WithdrawDetailsVw>(withdrawQuery, parameters);
            var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { Party = party });

            return (withdraws, totalCount);
        }

        /// <summary>
        /// Retrieves a paginated list of Withdraw records filtered by Done status along with the total count.
        /// </summary>
        public static async Task<(IEnumerable<WithdrawDetailsVw> Withdraws, int TotalCount)> GetPaginatedWithdrawDetailsVwByDoneAsync(bool done, int pageNumber, int pageSize, IDbConnection connection)
        {
            var withdrawQuery = BaseWithdrawDetailsVwQuery + @"
WHERE wd.[Done] = @Done
ORDER BY wd.[Id] DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var countQuery = @"SELECT COUNT(*) FROM [dbo].[Withdraw] WHERE [Done] = @Done";

            var parameters = new
            {
                Done = done,
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            };

            var withdraws = await connection.QueryAsync<WithdrawDetailsVw>(withdrawQuery, parameters);
            var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { Done = done });

            return (withdraws, totalCount);
        }

        /// <summary>
        /// Retrieves a paginated list of Withdraw records filtered by both Party and Done status along with the total count.
        /// </summary>
        public static async Task<(IEnumerable<WithdrawDetailsVw> Withdraws, int TotalCount)> GetPaginatedWithdrawDetailsVwByPartyAndDoneAsync(int party, bool done, int pageNumber, int pageSize, IDbConnection connection)
        {
            var withdrawQuery = BaseWithdrawDetailsVwQuery + @"
WHERE p.[Id] = @Party AND wd.[Done] = @Done
ORDER BY wd.[Id] DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var countQuery = @"
SELECT COUNT(*) 
FROM [dbo].[Withdraw] wd
INNER JOIN dbo.[Party] p ON wd.Party = p.Id
WHERE p.[Id] = @Party AND wd.[Done] = @Done";

            var parameters = new
            {
                Party = party,
                Done = done,
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            };

            var withdraws = await connection.QueryAsync<WithdrawDetailsVw>(withdrawQuery, parameters);
            var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { Party = party, Done = done });

            return (withdraws, totalCount);
        }
    }
}
/// <summary>
/// Represents a Withdraw record along with joined Party, Provider, Delivery information and calculated balance values.
/// </summary>
public class WithdrawDetailsVw
{
    public long Id { get; set; }

    // New columns from the additional joins:
    public int? ProviderId { get; set; }
    public int? DeliveryId { get; set; }

    public string BankAccountNumber { get; set; } = null!;
    public string IBanNumber { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool Done { get; set; }
    public bool Accepted { get; set; }
    public bool Rejected { get; set; }

    // Joined Party columns:
    public int PartyId { get; set; }
    public string PartyDisplayName { get; set; } = null!;
    public bool PartyIsActive { get; set; }

    // Calculated balance columns:
    public decimal? DeliveryActiveBalanceReceives { get; set; }
    public decimal? DeliveryActiveBalanceDeductions { get; set; }
    public decimal? ProviderActiveBalanceReceives { get; set; }
    public decimal? ProviderActiveBalanceDeductions { get; set; }
}


/// <summary>
/// Represents the data transfer object that will be serialized into JSON with the following shape:
/// {
///   "id": 1,
///   "party": 24,
///   "deliveryId": null,
///   "providerId": 24,
///   "bankAccountNumber": "string",
///   "iBanNumber": "string",
///   "date": "2025-01-04T04:39:56.383",
///   "done": false,
///   "accepted": false,
///   "rejected": false,
///   "partyDisplayName": "Delivery 1",
///   "partyIsActive": true,
///   "ActiveBalanceReceives": 20,
///   "ActiveBalanceDeductions": 22
/// }
/// </summary>
public class WithdrawDetailsDto
{
    public long Id { get; set; }
    /// <summary>
    /// If the party is of type Delivery, this will be populated.
    /// </summary>
    public int? DeliveryId { get; set; }

    /// <summary>
    /// If the party is of type Provider, this will be populated.
    /// </summary>
    public int? ProviderId { get; set; }

    public string BankAccountNumber { get; set; }
    public string IBanNumber { get; set; }
    public DateTime Date { get; set; }
    public bool Done { get; set; }
    public bool Accepted { get; set; }
    public bool Rejected { get; set; }
    public string PartyDisplayName { get; set; }
    public bool PartyIsActive { get; set; }
    public decimal? ActiveBalanceReceives { get; set; }
    public decimal? ActiveBalanceDeductions { get; set; }

    /// <summary>
    /// Creates a new instance of WithdrawDetailsDto by mapping the properties of a WithdrawDetailsVw.
    /// </summary>
    /// <param name="source">The source object containing the raw data.</param>
    public WithdrawDetailsDto(WithdrawDetailsVw source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        Id = source.Id;
        BankAccountNumber = source.BankAccountNumber;
        IBanNumber = source.IBanNumber;
        Date = source.Date;
        Done = source.Done;
        Accepted = source.Accepted;
        Rejected = source.Rejected;
        PartyDisplayName = source.PartyDisplayName;
        PartyIsActive = source.PartyIsActive;
        ProviderId = source.ProviderId;
        DeliveryId = source.DeliveryId;
        // Determine whether this record represents a Provider or Delivery.
        // In this example, if ProviderActiveBalanceReceives has a value,
        // we assume the party is a provider; otherwise, if DeliveryActiveBalanceReceives has a value,
        // we assume the party is a delivery.
        if (source.ProviderId.HasValue)
        {
            ActiveBalanceReceives = source.ProviderActiveBalanceReceives;
            ActiveBalanceDeductions = source.ProviderActiveBalanceDeductions;
        }
        if (source.DeliveryId.HasValue)
        {
            ActiveBalanceReceives = source.DeliveryActiveBalanceReceives;
            ActiveBalanceDeductions = source.DeliveryActiveBalanceDeductions;
        }
    }
}
