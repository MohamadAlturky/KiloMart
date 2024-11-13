using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Queries;

[ApiController]
[Route("api/card")]
public class CardQueryController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;
    public CardQueryController(IDbFactory dbFactory, IUserContext userContext)
    {
        _userContext = userContext;
        _dbFactory = dbFactory;
    }

    [HttpGet("mine")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetMine()
    {
        var partyId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var cards = await connection.QueryAsync<CardApiResponse>(
            "SELECT [Id], [HolderName], [Number], [SecurityCode], [ExpireDate], [Customer] FROM Card WHERE Customer = @partyId AND IsActive = 1;",
            new { partyId });
        return Ok(cards.ToArray());

    }




    [HttpGet("list")]
    // [Guard([Roles.Admin])]
    public async Task<IActionResult> GetAll([FromQuery]  int page = 1,[FromQuery]  int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
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

        return Ok(cards.ToArray());
    }
}

public class CardApiResponseWithCustomerName
{
    public int Id { get; set; }
    public string HolderName { get; set; } = null!;
    public string Number { get; set; }= null!;
    public string SecurityCode { get; set; }= null!;
    public DateTime ExpireDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }= null!;
    public bool IsActive { get; set; }
}
