// using KiloMart.Commands.Services;
// using KiloMart.Core.Authentication;
// using KiloMart.Core.Contracts;
// using KiloMart.Domain.Provider.Profile.Models;
// using Microsoft.AspNetCore.Mvc;

// namespace KiloMart.Presentation.Commands
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class ProviderProfileController : AppController
//     {
//         public ProviderProfileController(IDbFactory dbFactory, IUserContext userContext) 
//             : base(dbFactory, userContext)
//         {
//         }

//         [HttpPost]
//         public async Task<IActionResult> Create(CreateProviderProfileRequest request)
//         {
//             var result = await ProviderProfileService.InsertAsync(_dbFactory, request);

//             if (result.Success)
//             {
//                 return CreatedAtAction(nameof(Create), new { id = result.Data.Id }, result.Data);
//             }
//             else
//             {
//                 return BadRequest(result.Errors);
//             }
//         }

//         [HttpPut("{id}")]
//         public async Task<IActionResult> Update(int id, UpdateProviderProfileRequest request)
//         {
//             request.Id = id;

//             // Call Update method from ProviderProfileService.cs (Note: Update method is not implemented in the provided code)
//             // Below is an example implementation, you need to implement it as per your requirement
//             var result = await ProviderProfileService.UpdateAsync(_dbFactory, request);

//             if (result.Success)
//             {
//                 return Ok(result.Data);
//             }
//             else
//             {
//                 if (result.Errors.Contains("Not Found"))
//                 {
//                     return NotFound();
//                 }
//                 else
//                 {
//                     return BadRequest(result.Errors);
//                 }
//             }
//         }
//     }
// }
