using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Deliveries.Profile;
using KiloMart.Domain.Documents;
using KiloMart.Domain.Register.Delivery.Models;
using KiloMart.Domain.Register.Delivery.Services;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Controllers;
using KiloMart.Presentation.Models.Commands.Deliveries;
using KiloMart.Presentation.Services;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/delivery")]
public class DeliveryCommandController : AppController
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment environment;
    public DeliveryCommandController(IDbFactory dbFactory,
        IConfiguration configuration,
        IUserContext userContext, IWebHostEnvironment environment) : base(dbFactory, userContext)
    {
        _configuration = configuration;
        this.environment = environment;
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

        return result.IsSuccess
            ? Success(new
            {
                CustomerId = result.PartyId,
                result.UserId,
                result.VerificationToken
            }) : Fail(new string[] { result.ErrorMessage });
    }

    [HttpPost("profile/add")]
    public async Task<IActionResult> CreateProfile(CreateDeliveryProfileApiRequest request)
    {
        var (success, errors) = request.Validate();
        if (!success)
        {
            return ValidationError(errors);
        }
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var user = await Db.GetMembershipUserByEmailAsync(request.Email, connection);

        if (user is null)
        {
            return Fail("User Not Found");
        }
        if (user.PasswordHash != HashHandler.GetHash(request.Password))
        {
            return Fail("Invalid Phone Number Or Password");
        }
        using var writeConnection = _dbFactory.CreateDbConnection();
        writeConnection.Open();
        using var transaction = writeConnection.BeginTransaction();

        var result = await DeliveryProfileService.InsertAsync(writeConnection,
            transaction,
        new CreateDeliveryProfileRequest
        {
            Delivery = user.Party,
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            NationalId = request.NationalId,
            NationalName = request.NationalName,
            DrivingLicenseExpiredDate = request.DrivingLicenseExpiredDate,
            LicenseNumber = request.LicenseNumber,
            DrivingLicenseNumber = request.DrivingLicenseNumber,
            LicenseExpiredDate = request.LicenseExpiredDate,
        });

        if (!result.Success)
        {
            transaction.Rollback();
            return Fail(result.Errors);
        }

        #region VehiclePhotoFile
        // Save VehiclePhotoFile

        Guid fileName = Guid.NewGuid();
        if (request.VehiclePhotoFile is null)
        {
            transaction.Rollback();
            return Fail("VehiclePhotoFile is null");
        }
        var filePath = await FileService.SaveImageFileAsync(request.VehiclePhotoFile,
            environment.WebRootPath,
            fileName);

        if (string.IsNullOrEmpty(filePath))
        {
            transaction.Rollback();
            return Fail("failed to save file");
        }
        int deliveryId = user.Party;

        int documentId = await Db.InsertDeliveryDocumentAsync(
            writeConnection,
            "VehiclePhotoFile",
            filePath,
            deliveryId,
            (byte)DocumentType.VehiclePhoto,
            transaction);
        #endregion
        
        #region DrivingLicenseFile

        // Save DrivingLicenseFile

        fileName = Guid.NewGuid();
        if (request.DrivingLicenseFile is null)
        {
            transaction.Rollback();
            return Fail("DrivingLicenseFile is null");
        }
        filePath = await FileService.SaveImageFileAsync(request.DrivingLicenseFile,
            environment.WebRootPath,
            fileName);

        if (string.IsNullOrEmpty(filePath))
        {
            transaction.Rollback();
            return Fail("failed to save file");
        }

        documentId = await Db.InsertDeliveryDocumentAsync(
            writeConnection,
            "DrivingLicenseFile",
            filePath,
            deliveryId,
            (byte)DocumentType.DrivingLicense,
            transaction);
        #endregion

        #region NationalIqamaIDFile
        // Save NationalIqamaIDFile

        fileName = Guid.NewGuid();
        if (request.NationalIqamaIDFile is null)
        {
            transaction.Rollback();
            return Fail("NationalIqamaIDFile is null");
        }
        filePath = await FileService.SaveImageFileAsync(request.NationalIqamaIDFile,
            environment.WebRootPath,
            fileName);

        if (string.IsNullOrEmpty(filePath))
        {
            transaction.Rollback();
            return Fail("failed to save file");
        }

        documentId = await Db.InsertDeliveryDocumentAsync(
            writeConnection,
            "NationalIqamaIDFile",
            filePath,
            deliveryId,
            (byte)DocumentType.NationalIqamaID,
            transaction);
        #endregion

        #region VehicleLicenseFile

        fileName = Guid.NewGuid();
        if (request.VehicleLicenseFile is null)
        {
            transaction.Rollback();
            return Fail("VehicleLicenseFile is null");
        }
        filePath = await FileService.SaveImageFileAsync(request.VehicleLicenseFile,
            environment.WebRootPath,
            fileName);

        if (string.IsNullOrEmpty(filePath))
        {
            transaction.Rollback();
            return Fail("failed to save file");
        }

        documentId = await Db.InsertDeliveryDocumentAsync(
            writeConnection,
            "VehicleLicenseFile",
            filePath,
            deliveryId,
            (byte)DocumentType.VehicleLicense,
            transaction);
        #endregion

        #region Insert Vehicle
        var vehicleId = await Db.InsertVehicleAsync(
            writeConnection, 
            request.Number, 
            request.Model, 
            request.Type, 
            request.Year, deliveryId,
            transaction);
        #endregion

        transaction.Commit();
        return Success();
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
