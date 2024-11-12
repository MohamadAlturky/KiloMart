using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Queries;

[ApiController]
[Route("api/provider")]
public class ProviderQueryController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;

    public ProviderQueryController(IDbFactory dbFactory, IUserContext userContext)
    {
        _dbFactory = dbFactory;
        _userContext = userContext;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var provider = await connection.QueryFirstOrDefaultAsync<ProviderProfileApiResponse>(
            "SELECT [Id], [Provider], [FirstName], [SecondName], [OwnerNationalId], [NationalApprovalId], [CompanyName], [OwnerName] " +
            "FROM ProviderProfile WHERE Id = @id",
            new { id });

        return Ok(provider);
    }

    [HttpGet("admin/list")]
    public async Task<IActionResult> AdminList()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var providers = await connection.QueryAsync<ProviderProfileApiResponse>(
            "SELECT [Id], [Provider], [FirstName], [SecondName], [OwnerNationalId], [NationalApprovalId], [CompanyName], [OwnerName] " +
            "FROM ProviderProfile");

        return Ok(providers);
    }

    [HttpGet("localized/{id}")]
    public async Task<IActionResult> GetLocalized(int id, byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var sql = @"
            SELECT 
                ProviderProfile.Id,
                ProviderProfile.Provider,
                ProviderProfile.FirstName,
                ProviderProfile.SecondName,
                ProviderProfile.OwnerNationalId,
                ProviderProfile.NationalApprovalId,
                ProviderProfile.CompanyName,
                ProviderProfile.OwnerName,
                ProviderProfileLocalized.FirstName AS FirstNameLocalized,
                ProviderProfileLocalized.SecondName AS SecondNameLocalized,
                ProviderProfileLocalized.CompanyName AS CompanyNameLocalized,
                ProviderProfileLocalized.OwnerName AS OwnerNameLocalized,
                ProviderProfileLocalized.Language
            FROM 
                ProviderProfile
            LEFT JOIN 
                ProviderProfileLocalized 
            ON 
                ProviderProfile.Id = ProviderProfileLocalized.ProviderProfile 
                AND ProviderProfileLocalized.Language = @language
            WHERE 
                ProviderProfile.Id = @id";

        var provider = await connection.QueryFirstOrDefaultAsync<ProviderProfileApiResponse>(sql, new { id, language });

        return Ok(provider);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var sql = @"
        SELECT 
            [Id],
            [Provider],
            [FirstName],
            [SecondName],
            [OwnerNationalId],
            [NationalApprovalId],
            [CompanyName],
            [OwnerName]
        FROM 
            ProviderProfile
        WHERE 
            [Provider] = @provider";

        var providerId = _userContext.Get().Party;
        var provider = await connection.QueryFirstOrDefaultAsync<ProviderProfileApiResponse>(sql, new { provider = providerId });

        return Ok(provider);
    }

}
