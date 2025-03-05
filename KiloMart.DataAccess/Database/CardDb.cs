using Dapper;
using System.Data;

namespace KiloMart.DataAccess.Database;

/// <summary>
/// Table Specification
//  CREATE TABLE [dbo].[Card](
// 	[Id] [int] IDENTITY(1,1) NOT NULL,
// 	[HolderName] [varchar](100) NOT NULL,
// 	[Number] [varchar](100) NOT NULL,
// 	[SecurityCode] [varchar](100) NOT NULL,
// 	[ExpireDate] [date] NOT NULL,
// 	[Customer] [int] NOT NULL,
// 	[IsActive] [bit] NOT NULL,
//  [IsPrimary] [bit] NOT NULL) 
/// </summary>
/// 
public static partial class Db
{
    public static async Task<int> InsertCardAsync(IDbConnection connection,
        string holderName,
        string number,
        string securityCode,
        DateTime expireDate,
        int customer,
        bool isPrimary = false,
        IDbTransaction? transaction = null)
    {
        const string query = @"INSERT INTO [dbo].[Card]
                            ([HolderName], [Number], [SecurityCode], [ExpireDate], [Customer], [IsActive], [IsPrimary])
                            VALUES (@HolderName, @Number, @SecurityCode, @ExpireDate, @Customer, 1, @IsPrimary)
                            SELECT CAST(SCOPE_IDENTITY() AS INT)";

        return await connection.ExecuteScalarAsync<int>(query, new
        {
            HolderName = holderName,
            Number = number,
            SecurityCode = securityCode,
            ExpireDate = expireDate,
            Customer = customer,
            IsPrimary = isPrimary
        }, transaction);
    }

    public static async Task<bool> UpdateCardAsync(IDbConnection connection,
        int id, 
        string holderName, 
        string number, 
        string securityCode, 
        DateTime expireDate,
        int customer, 
        bool isActive,
        bool isPrimary,
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Card]
                                SET 
                                [HolderName] = @HolderName,
                                [Number] = @Number,
                                [SecurityCode] = @SecurityCode,
                                [ExpireDate] = @ExpireDate,
                                [Customer] = @Customer,
                                [IsActive] = @IsActive,
                                [IsPrimary] = @IsPrimary
                                WHERE [Id] = @Id";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id,
            HolderName = holderName,
            Number = number,
            SecurityCode = securityCode,
            ExpireDate = expireDate,
            Customer = customer,
            IsActive = isActive,
            IsPrimary = isPrimary
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> DeleteCardAsync(IDbConnection connection, int id, IDbTransaction? transaction = null)
    {
        const string query = @"DELETE FROM [dbo].[Card]
                                WHERE [Id] = @Id";

        var deletedRowsCount = await connection.ExecuteAsync(query, new
        {
            Id = id
        }, transaction);

        return deletedRowsCount > 0;
    }

    public static async Task<Card?> GetCardByIdAsync(int id, IDbConnection connection, IDbTransaction? transaction = null)
    {
        const string query = @"SELECT 
                            [Id], 
                            [HolderName], 
                            [Number], 
                            [SecurityCode], 
                            [ExpireDate], 
                            [Customer], 
                            [IsActive],
                            [IsPrimary]
                            FROM [dbo].[Card]
                            WHERE [Id] = @Id";

        return await connection.QueryFirstOrDefaultAsync<Card>(query, new
        {
            Id = id
        }, transaction);
    }

    public static async Task<bool> SetAllCardsNonPrimaryAsync(IDbConnection connection, 
        int customerId, 
        IDbTransaction? transaction = null)
    {
        const string query = @"UPDATE [dbo].[Card]
                             SET [IsPrimary] = 0
                             WHERE [Customer] = @CustomerId";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            CustomerId = customerId
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<bool> SetCardAsPrimaryAsync(IDbConnection connection,
        int cardId,
        int customerId,
        IDbTransaction? transaction = null)
    {
        // First, set all customer's cards as non-primary
        await SetAllCardsNonPrimaryAsync(connection, customerId, transaction);

        // Then set the specified card as primary
        const string query = @"UPDATE [dbo].[Card]
                             SET [IsPrimary] = 1
                             WHERE [Id] = @CardId 
                             AND [Customer] = @CustomerId
                             AND [IsActive] = 1";

        var updatedRowsCount = await connection.ExecuteAsync(query, new
        {
            CardId = cardId,
            CustomerId = customerId
        }, transaction);

        return updatedRowsCount > 0;
    }

    public static async Task<IEnumerable<Card>> GetCardsByCustomerAsync(IDbConnection connection, int customerId)
    {
        const string query = @"SELECT 
                            [Id], 
                            [HolderName], 
                            [Number], 
                            [SecurityCode], 
                            [ExpireDate], 
                            [Customer], 
                            [IsActive],
                            [IsPrimary]
                            FROM [dbo].[Card]
                            WHERE [Customer] = @CustomerId AND [IsActive] = 1
                            ORDER BY [IsPrimary] DESC, [Id] DESC";

        return await connection.QueryAsync<Card>(query, new
        {
            CustomerId = customerId
        });
    }
}

public class Card
{
    public int Id { get; set; }
    public string HolderName { get; set; } = null!;
    public string Number { get; set; } = null!;
    public string SecurityCode { get; set; } = null!;
    public DateTime ExpireDate { get; set; }
    public int Customer { get; set; }
    public bool IsActive { get; set; }
    public bool IsPrimary { get; set; }
}
