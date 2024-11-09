using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers.Queries;

public class LanguageController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    public LanguageController(IDbFactory dbFactory)
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
