using Dapper;
using KiloMart.Api.Models;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers.Queries;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    public CustomerController(IDbFactory dbFactory)
    {
        _dbFactory = dbFactory;
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
}
