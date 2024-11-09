using Dapper;
using KiloMart.Api.Models;
using KiloMart.DataAccess.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers.Queries;

[ApiController]
[Route("api/[controller]")]
public class ProviderController : ControllerBase
{
    private readonly IDbFactory _dbFactory;

    public ProviderController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
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
}
