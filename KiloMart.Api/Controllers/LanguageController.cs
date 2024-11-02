using KiloMart.Domain.Requests;
using KiloMart.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanguageController : ControllerBase
    {
        [HttpPost("add")]
        public IActionResult AddLanguage([FromBody] AddLanguageRequest request)
        {
            // Validate the request model
            var (isValid, errors) = request.Validate();

            if (!isValid)
            {
                // Return BadRequest with validation errors if the model is invalid
                return BadRequest(errors);
            }

            // Call the service method
            var response = LanguageService.AddLanguage(request);

            // Return the appropriate response
            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(new List<string> { response.Message });
        }
    }
}
