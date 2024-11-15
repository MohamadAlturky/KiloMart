using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Presentation.Commands;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/product-categories")]
public class ProductCategoryController : AppController
{
      public ProductCategoryController(IDbFactory dbFactory, IUserContext userContext) 
            : base(dbFactory, userContext)
      {
      }

      [HttpPost]
      public async Task<IActionResult> Insert(ProductCategoryLocalizedRequest model)
      {
            var result = await ProductCategoryService.InsertProductWithLocalization(_dbFactory, _userContext.Get(), model);
            return result.Success ? 
                  CreatedAtAction(nameof(Insert), new { id = result.Data.ProductCategory.Id, Data = result.Data }, result.Data) : 
                  BadRequest(result.Errors);
      }

      [HttpGet("paginated")]
      public async Task<IActionResult> GetAllLocalizedPaginatedForAdmin(
            [FromQuery] byte language,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isActive = true)
      {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            var (data, totalCount) = await Query.GetAllLocalizedPaginated(
                  connection, language, page, pageSize, isActive);
            return Ok(new { data, totalCount });
      }
}