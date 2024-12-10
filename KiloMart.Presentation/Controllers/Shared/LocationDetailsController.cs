using Dapper;
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers;

[ApiController]
[Route("api/customer-and-provider/location-details")]
public class LocationDetailsController(IDbFactory dbFactory, IUserContext userContext)
    : AppController(dbFactory, userContext)
{
    [HttpPost]
    [Guard([Roles.Customer, Roles.Provider])]
    public async Task<IActionResult> Insert(LocationDetailsInsertModel model)
    {
        var result = await LocationDetailsService.Insert(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            return Fail(result.Errors);
        }
    }

    [HttpPut("{id}")]
    [Guard([Roles.Customer, Roles.Provider])]
    public async Task<IActionResult> Update(LocationDetailsUpdateModel model)
    {

        var result = await LocationDetailsService.Update(_dbFactory, _userContext.Get(), model);

        if (result.Success)
        {
            return Success(result.Data);
        }
        else
        {
            if (result.Errors.Contains("Not Found"))
            {
                return DataNotFound();
            }
            else
            {
                return Fail(result.Errors);
            }
        }
    }

    [HttpGet("{locationId}")]
    [Guard([Roles.Customer, Roles.Provider])]
    public async Task<IActionResult> GetDetails(int locationId)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var query = "SELECT * FROM [dbo].[LocationDetails] WHERE [Location] = @locationId";
        var result = await connection.QuerySingleOrDefaultAsync<LocationDetails>(query, new { locationId = locationId });

        if (result is null)
        {
            return DataNotFound();
        }

        return Success(result);
    }
}