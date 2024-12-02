using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Domain.Deliveries.Profile;
using KiloMart.Domain.Register.Delivery.Models;
using KiloMart.Domain.Register.Delivery.Services;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Controllers;
using KiloMart.Presentation.Models.Commands.Deliveries;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/delivery")]
public class DeliveryCommandController : AppController
{
    private readonly IConfiguration _configuration;
    public DeliveryCommandController(IDbFactory dbFactory,
        IConfiguration configuration,
        IUserContext userContext) : base(dbFactory, userContext)
    {
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDeliveryDto dto)
    {
        var (success, errors) = dto.Validate();

        if (!success)
        {
            return ValidationError(errors);
        }

        var result = await new RegisterDeliveryService().Register(
            _dbFactory,
            _configuration,
            dto.Email,
            dto.Password,
            dto.DisplayName,
            dto.Language);
        return Success(result);
    }

    [HttpPost("profile/add")]
    public async Task<IActionResult> CreateProfile(CreateDeliveryProfileApiRequest request)
    {
        var (success, errors) = request.Validate();
        if (!success)
        {
            return ValidationError(errors);
        }
        var delivery = _userContext.Get().Party;

        var result = await DeliveryProfileService.InsertAsync(_dbFactory,
        new CreateDeliveryProfileRequest
        {
            Delivery = delivery,
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            NationalId = request.NationalId,
            NationalName = request.NationalName,
            DrivingLicenseExpiredDate = request.DrivingLicenseExpiredDate,
            LicenseNumber = request.LicenseNumber,
            DrivingLicenseNumber = request.DrivingLicenseNumber,
            LicenseExpiredDate = request.LicenseExpiredDate,
        });

        return result.Success ? Success(result) : Fail(result.Errors);
    }

    #region profile
    [HttpPost("profile/edit")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> EditProfile(UpdateDeliveryProfileRequest request)
    {

        var result = await DeliveryProfileService.UpdateAsync(_dbFactory,
            _userContext.Get(), request);

        return result.Success ? Success(result) : Fail(result.Errors);
    }
    #endregion

    [HttpGet("mine")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> GetDelivaryProfile()
    {
        var party = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var query = "SELECT [Id], [Delivary], [FirstName], [SecondName], [NationalName], [NationalId], [LicenseNumber], [LicenseExpiredDate], [DrivingLicenseNumber], [DrivingLicenseExpiredDate] FROM [dbo].[DelivaryProfile] WHERE [Delivary] = @Party";
        var result = await connection.QueryFirstOrDefaultAsync<DelivaryProfile>(query, new { Party = party });
        if (result is null)
        {
            return DataNotFound();
        }
        return Success(result);
    }


    [HttpGet("admin/list")]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Query.GetDeliveryProfilesWithUserInfoPaginated(connection, page, pageSize);
        if (result.Profiles is null || result.Profiles.Length == 0)
        {
            return DataNotFound();
        }
        return Success(new
        {
            Data = result.Profiles,
            TotalCount = result.TotalCount
        });
    }
    public class DelivaryProfile
    {
        public int Id { get; set; }
        public string Delivary { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public string NationalName { get; set; } = null!;
        public string NationalId { get; set; } = null!;
        public string LicenseNumber { get; set; } = null!;
        public DateTime LicenseExpiredDate { get; set; }
        public string DrivingLicenseNumber { get; set; } = null!;
        public DateTime DrivingLicenseExpiredDate { get; set; }
    }

}
