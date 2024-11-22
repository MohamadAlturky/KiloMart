using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/admin/phone-number")]
public class PhoneNumberAdminController : AppController
{
    public PhoneNumberAdminController(IDbFactory dbFactory, IUserContext userContext)
        : base(dbFactory, userContext)
    {
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetPhoneNumbers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
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

        return Success(new
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