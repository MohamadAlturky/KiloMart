using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Controllers;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/card")]
public class CardAdminController(IDbFactory dbFactory, IUserContext userContext) : AppController(dbFactory, userContext)
{
    [HttpGet("list")]
      public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
      {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            var cards = await Query.GetAllCardsPaginated(connection, page, pageSize);
            return Success(cards);
      }
}