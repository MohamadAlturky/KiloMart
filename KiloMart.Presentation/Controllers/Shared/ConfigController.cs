using Microsoft.AspNetCore.Mvc;
using KiloMart.DataAccess.Database;
using KiloMart.Core.Contracts;
using KiloMart.Core.Authentication;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;

namespace KiloMart.Presentation.Controllers.Shared;

[ApiController]
[Route("api/configs")]
public class ConfigController : AppController
{
    public ConfigController(IDbFactory dbFactory, IUserContext userContext)
        : base(dbFactory, userContext) { }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey(string key)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var config = await Db.GetConfigByKeyAsync(key, connection);
        if (config is null)
        {
            return DataNotFound();
        }

        return Success(config);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var configs = await Db.GetAllConfigsAsync(connection);

        return Success(new
        {
            Data = configs,
            TotalCount = configs.Count()
        });
    }

    [HttpPost("create")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> Create([FromBody] ConfigDto configDto)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.InsertConfigAsync(connection, configDto.Key, configDto.Value);

        if (result)
        {
            return Success(new { Created = configDto });
        }

        return Fail("Failed to create config");
    }

    [HttpPut("edit/{key}")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> Update(string key, [FromBody] ConfigDto configDto)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.UpdateConfigAsync(connection, key, configDto.Value);

        if (result)
        {
            return Success();
        }

        return DataNotFound();
    }

    [HttpDelete("{key}")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> Delete(string key)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Db.DeleteConfigAsync(connection, key);

        if (result)
        {
            return Success();
        }

        return DataNotFound();
    }
}

public class ConfigDto
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}