using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;

[ApiController]
[Route("api/card")]
public class CardController : AppController
{
    public CardController(IDbFactory dbFactory, IUserContext userContext)
        : base(dbFactory, userContext)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Insert(CardInsertModel model)
    {
        var result = await CardService.Insert(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return CreatedAtAction(nameof(Insert), new { id = result.Data.Id }, result.Data);
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CardUpdateModel model)
    {
        model.Id = id;

        var result = await CardService.Update(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return Ok(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return NotFound();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
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
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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