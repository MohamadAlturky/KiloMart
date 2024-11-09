using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Queries;

[ApiController]
[Route("api/language")]
public class LanguageQueryController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    public LanguageQueryController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [AuthorizeRole(1)]
    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var languages = await connection.QueryAsync<LanguageApiResponse>("SELECT [Id], [Name] FROM Language");
        return Ok(languages.ToArray());
    }
}
