using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Domains.Common;

[ApiController]
[Route("api/search")]
public partial class SearchController(
    IDbFactory dbFactory,
    IUserContext userContext)
    : AppController(dbFactory, userContext)
{

    #region Search History
    [HttpPost("search")]
    [Guard([Roles.Customer, Roles.Provider, Roles.Delivery])]
    public async Task<IActionResult> Search([FromBody] AddSearchTermRequest request)
    {
        var (IsSuccess, Errors) = request.Validate();
        if (!IsSuccess)
        {
            return ValidationError(Errors);
        }
        using var connection = _dbFactory.CreateDbConnection();
        var customerId = _userContext.Get().Party;

        // Insert the search term into the database
        var searchId = await Db.InsertSearchHistoryAsync(connection, customerId, request.Term);
        var productsInfos = await Db.SearchProductsAsync(connection, request.Term, 5);
        return Success(new { Id = searchId,productsInfos });
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