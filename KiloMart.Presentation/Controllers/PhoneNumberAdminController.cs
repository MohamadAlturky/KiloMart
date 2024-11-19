using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/phone-number")]
public class PhoneNumberAdminController : AppController
{
    public PhoneNumberAdminController(IDbFactory dbFactory, IUserContext userContext)
        : base(dbFactory, userContext)
    {
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
              p.DisplayName AS PersonName,
        	  u.Email UserEmail,
        	  u.Role UserRole
                  FROM PhoneNumber ph
                      INNER JOIN Party p ON ph.Party = p.Id
                      INNER JOIN MembershipUser u ON u.Party = p.Id
                          WHERE p.IsActive = 1
                              ORDER BY ph.Id
                OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY
        """;
        var phoneNumbers = await connection.QueryAsync<PhoneNumberApiResponse>(sql,
            new { skip, pageSize });

        return Ok(new
        {
            data = phoneNumbers.ToArray(),
            totalCount = count
        });
    }
}


public class PhoneNumberApiResponse
{
    public int PhoneNumberId { get; set; }
    public string NumberValue { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public int PersonId { get; set; }
    public byte UserRole { get; set; }
    public string PersonName { get; set; } = null!;
}

public class PhoneNumberApiResponseForMine
{
    public int Id { get; set; }
    public string Value { get; set; } = null!;
}