using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;

[ApiController]
[Route("api/phone-number")]
public class PhoneNumberController : AppController
{
    public PhoneNumberController(IDbFactory dbFactory, IUserContext userContext)
        : base(dbFactory, userContext)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Insert(PhoneNumberInsertModel model)
    {
        var result = await PhoneNumberService.Insert(_dbFactory, _userContext.Get(), model);

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
    public async Task<IActionResult> Update(int id, PhoneNumberUpdateModel model)
    {
        model.Id = id;

        var result = await PhoneNumberService.Update(_dbFactory, _userContext.Get(), model);

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
    [Guard([Roles.Customer, Roles.Provider, Roles.Delivery])]
    public async Task<IActionResult> GetByPartyId()
    {
        int partyId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var phoneNumbers = await connection.QueryAsync<PhoneNumberApiResponseForMine>(
            "SELECT [Id], [Value] FROM PhoneNumber WHERE Party = @partyId",
            new { partyId });
        return Ok(phoneNumbers.ToArray());
    }




    [HttpGet("list")]
    // [Guard([Roles.Admin])]
    public async Task<IActionResult> GetPhoneNumbers(int page = 1, int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        int skip = (page - 1) * pageSize;

        var countSql = """
        SELECT COUNT(*) FROM PhoneNumber ph 
            INNER JOIN Party p ON ph.Party = p.Id 
                WHERE p.IsActive = 1;
        """;
        var count = await connection.ExecuteScalarAsync<int>(countSql);

        var sql = """
            SELECT 
                ph.Id AS PhoneNumberId,
                ph.Value AS NumberValue,
                p.Id AS PersonId,
                p.DisplayName AS PersonName
                    FROM PhoneNumber ph
                        INNER JOIN Party p ON ph.Party = p.Id
                            WHERE p.IsActive = 1
                                ORDER BY ph.Id
                OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY
        """;
        var phoneNumbers = await connection.QueryAsync<PhoneNumberApiResponse>(sql,
            new { skip = skip, pageSize = pageSize });

        return Ok(new
        {
            data = phoneNumbers.ToArray(),
            totalCount = count
        });
    }


    public class PhoneNumberApiResponse
    {
        public int PhoneNumberId { get; set; }
        public string NumberValue { get; set; } = null!;
        public int PersonId { get; set; }
        public string PersonName { get; set; } = null!;
    }

    public class PhoneNumberApiResponseForMine
    {
        public int Id { get; set; }
        public string Value { get; set; } = null!;
    }


}

