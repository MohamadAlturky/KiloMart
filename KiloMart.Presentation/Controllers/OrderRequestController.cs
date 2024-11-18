using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.OrderRequests;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderRequestController : AppController
    {
        public OrderRequestController(IDbFactory dbFactory, IUserContext userContext) : base(dbFactory, userContext)
        {
        }

        [HttpPost]
        [Guard([Roles.Customer])]
        public async Task<IActionResult> CreateOrderRequest([FromBody] CreateOrderRequestApiRequestModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid request model.");
            }

            var userPayload = _userContext.Get();
            

            var result = await OrderRequestService.Insert(model, userPayload, _dbFactory);

            if (result.Success)
            {
                return CreatedAtAction(nameof(CreateOrderRequest), result.Data);
            }

            return BadRequest(result.Errors);
        }
    }
}