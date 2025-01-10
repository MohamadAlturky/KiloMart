using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/textual-config")]
public partial class TextualConfigController(IDbFactory dbFactory, IUserContext userContext)
    : AppController(dbFactory, userContext)
{

    [HttpPut("terms-and-conditions")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> UpdateTerms([FromBody] UpdateTextualConfigModel request)
    {
        var (success, errors) = request.Validate();
        if (!success)
            return ValidationError(errors);

        // Serialize the list of key-value pairs to a JSON string
        string jsonValue = JsonSerializer.Serialize(request.Value);

        using var connection = _dbFactory.CreateDbConnection();

        bool updated = await Db.UpdateTextualConfigAsync(
            connection,
            TextualConfigKey.TermsAndConditions,
            jsonValue, // Save the JSON string
            request.Language);

        if (!updated)
            return DataNotFound();

        return Success();
    }

    [HttpPut("about-app")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> UpdateAbout([FromBody] UpdateTextualConfigModel request)
    {
        var (success, errors) = request.Validate();
        if (!success)
            return ValidationError(errors);

        // Serialize the list of key-value pairs to a JSON string
        string jsonValue = JsonSerializer.Serialize(request.Value);

        using var connection = _dbFactory.CreateDbConnection();

        bool updated = await Db.UpdateTextualConfigAsync(
            connection,
            TextualConfigKey.AboutApp,
            jsonValue, // Save the JSON string
            request.Language);

        if (!updated)
            return DataNotFound();

        return Success();
    }

    [HttpGet("about-app/{language}")]
    public async Task<IActionResult> GetAbout(byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();

        var config = await Db.GetTextualConfigByKeyAndLanguageAsync(
            connection,
            TextualConfigKey.AboutApp,
            language);

        if (config is null)
            return DataNotFound();

        // Deserialize the JSON string back to a list of key-value pairs
        var valueList = JsonSerializer.Deserialize<List<KeyValuePair<string, string>>>(config.Value);

        return Success(valueList); // Return as JSON
    }

    [HttpGet("terms-and-conditions/{language}")]
    public async Task<IActionResult> GetTerms(byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();

        var config = await Db.GetTextualConfigByKeyAndLanguageAsync(
            connection,
            TextualConfigKey.TermsAndConditions,
            language);

        if (config is null)
            return DataNotFound();

        // Deserialize the JSON string back to a list of key-value pairs
        var valueList = JsonSerializer.Deserialize<List<KeyValuePair<string, string>>>(config.Value);

        return Success(valueList); // Return as JSON
    }
}

public class UpdateTextualConfigModel
{
    public List<KeyValuePair<string, string>> Value { get; set; } = null!; // List of key-value pairs
    public byte Language { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (Value == null || Value.Count == 0)
            errors.Add("Value is required and cannot be empty.");

        if (Language < 1)
            errors.Add("Invalid language code.");

        return (errors.Count == 0, errors.ToArray());
    }
}