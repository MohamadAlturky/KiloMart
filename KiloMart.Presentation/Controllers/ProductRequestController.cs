using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.ProductRequests.Accept;
using KiloMart.Domain.ProductRequests.Add;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Models.Commands.ProductRequests;
using KiloMart.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Commands;

[ApiController]
[Route("api/product-request")]
public class ProductRequestController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IUserContext _userContext;
    private readonly IWebHostEnvironment _environment;

    public ProductRequestController(IDbFactory dbFactory, IUserContext userContext, IWebHostEnvironment environment)
    {
        _dbFactory=dbFactory;
        _userContext=userContext;
        _environment=environment;
    }

    [HttpPost]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Insert([FromForm] ProductRequestInsertWithFileModel request)
    {
        var (success, errors) = request.Validate();
        if (!success)
            return BadRequest(errors);
        if (request.ImageFile is null)
        {
            return BadRequest("File is required");
        }
        Guid fileName = Guid.NewGuid();
        var filePath = await FileService.SaveFileAsync(request.ImageFile,
            _environment.WebRootPath,
            fileName);


        var result = await ProductRequestService.Insert(_dbFactory, _userContext.Get(), new ProductRequestInsertModel()
        {
            Date = DateTime.Now,
            Description = request.Description,
            ImageUrl = filePath,
            Language = request.Language,
            MeasurementUnit = request.MeasurementUnit,
            Name = request.Name,
            OffPercentage = request.OffPercentage,
            Price = request.Price,
            ProductCategory = request.ProductCategory,
            Quantity = request.Quantity
        });
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);
    }

    [HttpPost("accept")]
    public async Task<IActionResult> Accept([FromQuery] int id)
    {
        var result = await AcceptProductRequestService.Accept(_dbFactory, id);
        return result.Success ? Ok(result.Data) : StatusCode(500, result.Errors);

    }
}

