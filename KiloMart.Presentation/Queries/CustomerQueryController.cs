using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Models.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Queries;

[ApiController]
[Route("api/customer")]
public class CustomerQueryController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;

    public CustomerQueryController(IDbFactory dbFactory,IUserContext userContext)
    {
        _dbFactory = dbFactory;
        _userContext = userContext;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var customer = await connection.QueryFirstOrDefaultAsync<CustomerProfileApiResponse>(
            "SELECT [Id], [FirstName], [SecondName], [NationalName], [NationalId] FROM CustomerProfile WHERE Id = @id",
            new { id });
        return Ok(customer);
    }
    //admin/list
    [HttpGet("admin/list")]
    public async Task<IActionResult> AdminList()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var customers = await connection.QueryAsync<CustomerProfileApiResponse>("SELECT [Id], [FirstName], [SecondName], [NationalName], [NationalId] FROM CustomerProfile");
        return Ok(customers.ToArray());
    }
    //localized/id leftjoin customerprofile and customerprofilelocalized
    [HttpGet("localized/{id}")]
    public async Task<IActionResult> Localized(int id, byte language)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var sql = @"
    SELECT 
        CustomerProfile.Id, 
        CustomerProfile.Customer, 
        CustomerProfile.FirstName, 
        CustomerProfile.SecondName, 
        CustomerProfile.NationalName, 
        CustomerProfile.NationalId,
        CustomerProfileLocalized.FirstName AS LocalizedFirstName, 
        CustomerProfileLocalized.SecondName AS LocalizedSecondName, 
        CustomerProfileLocalized.NationalName AS LocalizedNationalName, 
        CustomerProfileLocalized.Language
    FROM 
        CustomerProfile
    LEFT JOIN 
        CustomerProfileLocalized 
        ON CustomerProfile.Id = CustomerProfileLocalized.CustomerProfile 
        AND CustomerProfileLocalized.Language = @language
    WHERE 
        CustomerProfile.Id = @id";
        var customer = await connection.QueryAsync<CustomerProfileApiResponse>(sql, new { id, language });
        return Ok(customer.FirstOrDefault()); // Return the first result if it exists

    }
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        using var connection = _dbFactory.CreateDbConnection();
        var customerId = _userContext.Get().Party;

        connection.Open();
        var sql = @"
        SELECT 
            [Id], 
            [Customer], 
            [FirstName], 
            [SecondName], 
            [NationalName], 
            [NationalId]
        FROM 
            [CustomerProfile]
        WHERE 
            [Customer] = @customerId";
        var customer = await connection.QueryFirstOrDefaultAsync<CustomerProfileApiResponse>(sql, new { customerId });
        return Ok(customer);
    }
}
