using Dapper;
using KiloMart.DataAccess.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers.Queries;

public class LanguageController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    public LanguageController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }
    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var languages = await connection.QueryAsync<LanguageDto>("SELECT [Id], [Name] FROM Language");
        return Ok(languages.ToArray());
    }
}
