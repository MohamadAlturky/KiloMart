
using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Queries;

[ApiController]
[Route("api/delivary")]
public class DelivaryQueryController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;
    public DelivaryQueryController(IDbFactory dbFactory,IUserContext userContext)
    {
        _userContext = userContext;
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

    [HttpGet("profile")]
public async Task<IActionResult> GetProfile()
{
    using var connection = _dbFactory.CreateDbConnection();
    // You need to get the delivary Id from the current user context or some other way
    var delivaryId = _userContext.Get().Party; // You need to implement this method
    connection.Open();
    var sql = @"
        SELECT
            [Id],
            [Delivary],
            [FirstName],
            [SecondName],
            [NationalName],
            [NationalId],
            [LicenseNumber],
            [LicenseExpiredDate],
            [DrivingLicenseNumber],
            [DrivingLicenseExpiredDate]
        FROM
            DelivaryProfile
        WHERE
            Id = @delivaryId";
    var delivaryProfile = await connection.QueryFirstOrDefaultAsync<DelivaryProfileApiResponse>(sql, new { delivaryId });
    return Ok(delivaryProfile);
}

}


public class DelivaryProfileApiResponse
{
    public int Id { get; set; }
    public int Delivary { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string NationalName { get; set; }
    public string NationalId { get; set; }
    public string LicenseNumber { get; set; }
    public DateTime LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; }
    public DateTime DrivingLicenseExpiredDate { get; set; }
}
