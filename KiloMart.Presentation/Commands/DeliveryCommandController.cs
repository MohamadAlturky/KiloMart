using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Delivery.Profile.Models;
using KiloMart.Domain.Delivery.Profile.Services;
using KiloMart.Domain.Register.Delivery.Models;
using KiloMart.Domain.Register.Delivery.Services;
using KiloMart.Presentation.Models.Commands.Deliveries;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/delivery")]
public class DeliveryCommandController : ControllerBase
{
    private readonly IDbFactory _dbFactory;
    private readonly IConfiguration _configuration;
    private readonly IUserContext _userContext;
    public DeliveryCommandController(IDbFactory dbFactory,
        IConfiguration configuration,
        IUserContext userContext)
    {
        _userContext = userContext;
        _dbFactory = dbFactory;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDeliveryDto dto)
    {
        var (success, errors) = dto.Validate();

        if (!success)
        {
            return BadRequest(errors);
        }

        var result = await new RegisterDeliveryService().Register(
            _dbFactory,
            _configuration,
            dto.Email,
            dto.Password,
            dto.DisplayName);
        return Ok(result);
    }

    [HttpPost("profile/add")]
    public async Task<IActionResult> CreateProfile(CreateDeliveryProfileApiRequest request)
    {
        var (success, errors) = request.Validate();
        if (!success)
        {
            return BadRequest(errors);
        }
        var delivery = _userContext.Get().Party;

        var result = await DeliveryProfileService.InsertAsync(_dbFactory,
        new CreateDeliveryProfileRequest
        {
            Delivery = request.Delivery,
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            NationalId = request.NationalId,
            NationalName = request.NationalName,
            DrivingLicenseExpiredDate = request.DrivingLicenseExpiredDate,
            LicenseNumber = request.LicenseNumber,
            DrivingLicenseNumber = request.DrivingLicenseNumber,
            LicenseExpiredDate = request.LicenseExpiredDate,
        });

        return result.Success ? Ok(result) : StatusCode(500, result.Errors);
    }
}
