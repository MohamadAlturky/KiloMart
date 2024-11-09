
using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Queries;

[ApiController]
[Route("api/[controller]")]
public class DelivaryController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    public DelivaryController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var delivary = await connection.QueryFirstOrDefaultAsync<DelivaryProfileApiResponse>(
            "SELECT [Id], [Delivary], [FirstName], [SecondName], [NationalName], [NationalId], [LicenseNumber], [LicenseExpiredDate], [DrivingLicenseNumber] FROM DelivaryProfile WHERE Id = @id",
            new { id });
        return Ok(delivary);
    }
    [HttpGet("admin/list")]
    public async Task<IActionResult> AdminList()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var delivaries = await connection.QueryAsync<DelivaryProfileApiResponse>("SELECT [Id], [Delivary], [FirstName], [SecondName], [NationalName], [NationalId], [LicenseNumber], [LicenseExpiredDate], [DrivingLicenseNumber] FROM DelivaryProfile");
        return Ok(delivaries);
    }
    //get localized fields
    [HttpGet("localized/{id}")]
    public async Task<IActionResult> GetLocalized(int id, byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var sql = @"
    SELECT 
        DelivaryProfile.Id, 
        DelivaryProfile.Delivary, 
        DelivaryProfile.FirstName, 
        DelivaryProfile.SecondName, 
        DelivaryProfile.NationalName, 
        DelivaryProfile.NationalId,
        DelivaryProfile.LicenseNumber,
        DelivaryProfile.LicenseExpiredDate,
        DelivaryProfile.DrivingLicenseNumber,
        DelivaryProfileLocalized.FirstName AS FirstNameLocalized, 
        DelivaryProfileLocalized.SecondName AS SecondNameLocalized, 
        DelivaryProfileLocalized.NationalName AS NationalNameLocalized, 
        DelivaryProfileLocalized.Language
    FROM 
        DelivaryProfile
    LEFT JOIN 
        DelivaryProfileLocalized 
        ON DelivaryProfile.Id = DelivaryProfileLocalized.DelivaryProfile 
        AND DelivaryProfileLocalized.Language = @language
    WHERE 
        DelivaryProfile.Id = @id";
        var Delivary = await connection.QueryAsync<DelivaryProfileApiResponse>(sql, new { id, language });
        return Ok(Delivary.FirstOrDefault()); // Return the first result if it exists

    }
}