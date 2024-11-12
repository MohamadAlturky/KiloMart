// ProductLocalizedController.cs
using KiloMart.Commands.Services;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductLocalizedController : AppController
    {
        public ProductLocalizedController(IDbFactory dbFactory, IUserContext userContext) 
            : base(dbFactory, userContext)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Insert(ProductLocalizedDto model)
        {
            var result = ProductLocalizedService.Insert(_dbFactory, model);

            if (result.Success)
            {
                return CreatedAtAction(nameof(Insert), new { id = result.Data.Product }, result.Data);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> Update(int productId, ProductLocalizedDto model)
        {
            // You would need to add an additional property to the ProductLocalizedDto to include the ID
            // You could add a property for Id and override the Product and ProductId
            // For now, you can handle these based on service layer implementation
            // this part is commented out for you, consider the above suggestion to improve model
            // model.Product = productId;
            var result = ProductLocalizedService.Insert(_dbFactory, model);

            if (result.Success)
            {
                return Ok(result.Data);
            }
            else
            {
                if (result.Errors.Contains("Not Found"))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
        }
    }
}
