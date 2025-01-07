using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Queries.Views.Sql;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Domains.Common;

[ApiController]
[Route("api/search")]
public partial class SearchController(
    IDbFactory dbFactory,
    IUserContext userContext)
    : AppController(dbFactory, userContext)
{

    // [HttpPost("demo33")]
    // public async Task<IActionResult> Demo()
    // {
    //     // Create a database connection
    //     using var connection = _dbFactory.CreateDbConnection();

    //     // Open the connection
    //     connection.Open();

    //     // Define the SQL query
    //     var query = @$"
    //     SELECT 
    //         m.*,
    //         p.*
    //     FROM ({Sql.MembershipUserSql}) m
    //     INNER JOIN ({Sql.PartySql}) p 
    //     ON p.Id = m.Party";

    //     // Execute the query and map results to the specified response types
    //     var results = await connection.QueryAsync<MembershipUserSqlResponse, PartySqlResponse, PartyMembershipUserSqlResponse>(
    //         query,
    //         (membershipUser, party) =>
    //         {
    //             return new PartyMembershipUserSqlResponse
    //             {
    //                 MembershipUser = membershipUser,
    //                 Party = party
    //             };
    //         }
    //         // ,
    //         // splitOn: "p.Id" // Specify the column to split on if necessary
    //     );

    //     // Return the results as an OK response
    //     return Ok(new { results });
    // }

    // // Define a combined response model if needed







    #region Search History
    // [HttpPost("search")]
    // [Guard([Roles.Customer, Roles.Provider])]
    // public async Task<IActionResult> Search([FromBody] AddSearchTermRequest request)
    // {
    //     var (IsSuccess, Errors) = request.Validate();
    //     if (!IsSuccess)
    //     {
    //         return ValidationError(Errors);
    //     }
    //     using var connection = _dbFactory.CreateDbConnection();
    //     var partyId = _userContext.Get().Party;

    //     // Insert the search term into the database
    //     var searchId = await Db.InsertSearchHistoryAsync(connection, partyId, request.Term);


    //     if (_userContext.Get().Role == (byte)Roles.Customer)
    //     {
    //         var products = await Db.SearchProductDetailsForCustomerAsync(
    //                     5,
    //                     request.Language,
    //                     partyId,
    //                     request.Term,
    //                     connection);

    //         return Success(new { searchId, products });
    //     }


    //     if (_userContext.Get().Role == (byte)Roles.Provider)
    //     {
    //         var products = await Db.SearchProductDetailsForProviderAsync(
    //                     5,
    //                     request.Language,
    //                     partyId,
    //                     request.Term,
    //                     connection);

    //         return Success(new { searchId, products });
    //     }
    //     return Fail("Only Provider and Customer can use this api");
    // }


    [HttpPost("search-by-location")]
    [Guard([Roles.Customer, Roles.Provider])]
    public async Task<IActionResult> SearchByLocation(
        [FromBody] AddSearchTermRequest request,
        [FromQuery] decimal? longitude,
        [FromQuery] decimal? latitude)
    {
        var (IsSuccess, Errors) = request.Validate();
        if (!IsSuccess)
        {
            return ValidationError(Errors);
        }
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var settings = await Db.GetSystemSettingsByIdAsync(0, connection);

        if (settings is null)
        {
            return Fail("system settings not found");
        }

        decimal distanceInKm = settings.RaduisForGetProducts;

        var partyId = _userContext.Get().Party;

        // Insert the search term into the database
        var searchId = await Db.InsertSearchHistoryAsync(connection, partyId, request.Term);


        if (_userContext.Get().Role == (byte)Roles.Customer)
        {
            var products = await Db.SearchProductDetailsForCustomerWithInLocationCircleAsync(
                        5,
                        request.Language,
                        partyId,
                        request.Term,
                        distanceInKm,
                        longitude,
                        latitude,
                        connection);

            return Success(new { searchId, products });
        }


        if (_userContext.Get().Role == (byte)Roles.Provider)
        {
            var products = await Db.SearchProductDetailsForProviderAsync(
                        5,
                        request.Language,
                        partyId,
                        request.Term,
                        connection);

            return Success(new { searchId, products });
        }
        return Fail("Only Provider and Customer can use this api");
    }

    [HttpPost("search-by-location-id")]
    [Guard([Roles.Customer, Roles.Provider])]
    public async Task<IActionResult> SearchByLocationId(
      [FromBody] AddSearchTermRequest request,
        [FromQuery] int? locationId)
    {
        var (IsSuccess, Errors) = request.Validate();
        if (!IsSuccess)
        {
            return ValidationError(Errors);
        }
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var settings = await Db.GetSystemSettingsByIdAsync(0, connection);

        if (settings is null)
        {
            return Fail("system settings not found");
        }

        decimal? longitude = null;
        decimal? latitude = null;
        if (locationId.HasValue)
        {
            var location = await Db.GetLocationByIdAsync(locationId.Value, connection);
            if (location is not null)
            {
                longitude = location.Longitude;
                latitude = location.Latitude;
            }
        }

        decimal distanceInKm = settings.RaduisForGetProducts;

        var partyId = _userContext.Get().Party;

        // Insert the search term into the database
        var searchId = await Db.InsertSearchHistoryAsync(connection, partyId, request.Term);


        if (_userContext.Get().Role == (byte)Roles.Customer)
        {
            var products = await Db.SearchProductDetailsForCustomerWithInLocationCircleAsync(
                        5,
                        request.Language,
                        partyId,
                        request.Term,
                        distanceInKm,
                        longitude,
                        latitude,
                        connection);

            return Success(new { searchId, products });
        }


        if (_userContext.Get().Role == (byte)Roles.Provider)
        {
            var products = await Db.SearchProductDetailsForProviderAsync(
                        5,
                        request.Language,
                        partyId,
                        request.Term,
                        connection);

            return Success(new { searchId, products });
        }
        return Fail("Only Provider and Customer can use this api");
    }


    [HttpGet("get-last-searches/{count}")]
    [Guard([Roles.Customer, Roles.Provider, Roles.Delivery])]
    public async Task<IActionResult> GetLastSearches(int count)
    {
        using var connection = _dbFactory.CreateDbConnection();
        var customerId = _userContext.Get().Party;

        // Retrieve the last 'count' searches for the customer
        var lastSearches = await Db.GetLastSearchesByCustomerAsync(connection, customerId, count);

        return Success(lastSearches);
    }

    [HttpDelete("remove-search-term/{id}")]
    [Guard([Roles.Customer, Roles.Provider, Roles.Delivery])]
    public async Task<IActionResult> RemoveSearchTerm(long id)
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Retrieve the search history record by ID
        SearchHistory? searchHistory = await Db.GetSearchHistoryByIdAsync(id, connection);

        if (searchHistory is null)
        {
            return DataNotFound("Search term not found.");
        }

        if (searchHistory.Customer != _userContext.Get().Party)
        {
            return Fail("Unauthorized");
        }

        // Delete the search history record
        var deleted = await Db.DeleteSearchHistoryAsync(connection, id);

        if (!deleted)
        {
            return DataNotFound("Failed to delete search term.");
        }

        return Success();
    }


    public class AddSearchTermRequest
    {
        public string Term { get; set; } = null!;
        public byte Language { get; set; }

        public (bool Success, string[] Errors) Validate()
        {
            var errors = new List<string>();

            if (Term.Length < 2)
                errors.Add("Search Term shouldn't be less than 2");

            return (errors.Count == 0, errors.ToArray());
        }
    }
    #endregion
}