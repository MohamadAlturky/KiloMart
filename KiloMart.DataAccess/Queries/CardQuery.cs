using System.Data;
using Dapper;

namespace KiloMart.Requests.Queries;

public partial class Query
{
    public static async Task<CardApiResponse[]> GetCustomerCards(IDbConnection connection, int partyId)
    {
        var cards = await connection.QueryAsync<CardApiResponse>(
            "SELECT [Id], [HolderName], [Number], [SecurityCode], [ExpireDate], [Customer] FROM Card WHERE Customer = @partyId AND IsActive = 1;",
            new { partyId });
        return cards.ToArray();
    }

    public static async Task<CardApiResponseWithCustomerName[]> GetAllCardsPaginated(
        IDbConnection connection,
        int page = 1,
        int pageSize = 10)
    {
        int skip = (page - 1) * pageSize;
        var query = @"
        SELECT 
                [c].[Id],
                [c].[HolderName], 
                [c].[Number], 
                [c].[SecurityCode], 
                [c].[ExpireDate], 
                [p].[Id] as [CustomerId],
                [p].[DisplayName] as [CustomerName],
                [c].[IsActive]
		FROM Card [c]
		INNER JOIN Party [p] 
			ON [c].[Customer] = [p].[Id]
		WHERE [p].[IsActive] = 1  
			ORDER BY [c].[Id] 
	    OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY;";

        var cards = await connection.QueryAsync<CardApiResponseWithCustomerName>(
            query,
            new { skip, pageSize });

        return cards.ToArray();
    }
}

public class CardApiResponseWithCustomerName
{
    public int Id { get; set; }
    public string HolderName { get; set; } = null!;
    public string Number { get; set; } = null!;
    public string SecurityCode { get; set; } = null!;
    public DateTime ExpireDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public bool IsActive { get; set; }
}
public class CardApiResponse
{
    public int Id { get; set; }
    public string HolderName { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string SecurityCode { get; set; } = string.Empty;
    public DateTime ExpireDate { get; set; }
    public int Customer { get; set; }
}
