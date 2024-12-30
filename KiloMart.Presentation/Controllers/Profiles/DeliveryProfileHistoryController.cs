using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Profiles;

[ApiController]
[Route("api/delivery-profile")]
public class DeliveryProfileHistoryController(
    IDbFactory dbFactory,
    IConfiguration configuration,
    IUserContext userContext,
    IWebHostEnvironment environment)
    : AppController(dbFactory, userContext)
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IWebHostEnvironment _environment = environment;

    [HttpPost("change")]
    public async Task<IActionResult> Insert(DeliveryProfileHistoryInsertModel request)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();


        #region Getting the User 
        var user = await Db.GetMembershipUserByEmailAsync(request.Email, connection);

        if (user is null)
        {
            return Fail("User Not Found");
        }
        if (user.PasswordHash != HashHandler.GetHash(request.Password))
        {
            return Fail("Invalid Phone Number Or Password");
        }
        #endregion

        try
        {


            #region VehiclePhotoFile

            Guid fileName = Guid.NewGuid();
            if (request.VehiclePhotoFile is null)
            {
                return Fail("VehiclePhotoFile is null");
            }

            var VehiclePhotoFilePath = await FileService.SaveImageFileAsync(request.VehiclePhotoFile,
                _environment.WebRootPath,
                fileName);

            if (string.IsNullOrEmpty(VehiclePhotoFilePath))
            {
                return Fail("failed to save VehiclePhoto file");
            }
            #endregion

            #region DrivingLicenseFile

            // Save DrivingLicenseFile

            fileName = Guid.NewGuid();
            if (request.DrivingLicenseFile is null)
            {
                return Fail("DrivingLicenseFile is null");
            }
            var DrivingLicenseFilePath = await FileService.SaveImageFileAsync(request.DrivingLicenseFile,
                _environment.WebRootPath,
                fileName);

            if (string.IsNullOrEmpty(DrivingLicenseFilePath))
            {
                return Fail("failed to save DrivingLicense file");
            }
            #endregion

            #region NationalIqamaIDFile
            // Save NationalIqamaIDFile

            fileName = Guid.NewGuid();
            if (request.NationalIqamaIDFile is null)
            {
                return Fail("NationalIqamaIDFile is null");
            }
            var NationalIqamaIDFilePath = await FileService.SaveImageFileAsync(request.NationalIqamaIDFile,
                _environment.WebRootPath,
                fileName);

            if (string.IsNullOrEmpty(NationalIqamaIDFilePath))
            {
                return Fail("failed to save NationalIqamaID file");
            }

            #endregion

            #region VehicleLicenseFile

            fileName = Guid.NewGuid();
            if (request.VehicleLicenseFile is null)
            {
                return Fail("VehicleLicenseFile is null");
            }
            var VehicleLicenseFilePath = await FileService.SaveImageFileAsync(request.VehicleLicenseFile,
                _environment.WebRootPath,
                fileName);

            if (string.IsNullOrEmpty(VehicleLicenseFilePath))
            {
                return Fail("failed to save VehicleLicense file");
            }
            #endregion

            #region Adding the Profile
            long id = await Db.InsertDeliveryProfileHistoryAsync(
                connection,
                request.FirstName,
                request.SecondName,
                request.NationalName,
                request.NationalId,
                request.LicenseNumber,
                request.LicenseExpiredDate,
                request.DrivingLicenseNumber,
                request.DrivingLicenseExpiredDate,
                request.VehicleNumber,
                request.VehicleModel,
                request.VehicleType,
                request.VehicleYear,
                VehiclePhotoFilePath,
                DrivingLicenseFilePath,
                VehicleLicenseFilePath,
                NationalIqamaIDFilePath,
                DateTime.Now,
                user.Party,
                false,
                false,
                false);

            #endregion

            #region Returning The Response
            var model = new
            {
                request.FirstName,
                request.SecondName,
                request.NationalName,
                request.NationalId,
                request.LicenseNumber,
                request.LicenseExpiredDate,
                request.DrivingLicenseNumber,
                request.DrivingLicenseExpiredDate,
                request.VehicleNumber,
                request.VehicleModel,
                request.VehicleType,
                request.VehicleYear,
                VehiclePhotoFilePath,
                DrivingLicenseFilePath,
                VehicleLicenseFilePath,
                NationalIqamaIDFilePath,
                SubmitDate = DateTime.Now,
                DeliveryId = user.Party,
                IsActive = false,
                IsRejected = false,
                IsAccepted = false
            };
            return Success(new { id, model });
            #endregion
        }
        catch
        {
            return Fail("Failed to insert delivery profile history.");
        }
    }



    [HttpGet("mine")]
    [Guard([Roles.Delivery])]
    public async Task<IActionResult> Mine(
        bool? isActive = null,
        bool? isRejected = null,
        bool? isAccepted = null,
        DateTime? submitDateFrom = null,
        DateTime? submitDateTo = null,
        DateTime? reviewDateFrom = null,
        DateTime? reviewDateTo = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        int deliveryId = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var histories = await Db.GetDeliveryProfileHistoryFilteredAsync(
            connection,
            deliveryId,
            isActive,
            isRejected,
            isAccepted,
            submitDateFrom,
            submitDateTo,
            reviewDateFrom,
            reviewDateTo,
            pageNumber,
            pageSize);

        var total = await Db.GetDeliveryProfileHistoryFilteredCountAsync(
            connection,
            deliveryId,
            isActive,
            isRejected,
            isAccepted,
            submitDateFrom,
            submitDateTo,
            reviewDateFrom,
            reviewDateTo);

        return Success(new
        {
            Items = histories,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    [HttpGet("filter")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> GetFiltered(
        int? deliveryId = null,
        bool? isActive = null,
        bool? isRejected = null,
        bool? isAccepted = null,
        DateTime? submitDateFrom = null,
        DateTime? submitDateTo = null,
        DateTime? reviewDateFrom = null,
        DateTime? reviewDateTo = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var histories = await Db.GetDeliveryProfileHistoryFilteredAsync(
            connection,
            deliveryId,
            isActive,
            isRejected,
            isAccepted,
            submitDateFrom,
            submitDateTo,
            reviewDateFrom,
            reviewDateTo,
            pageNumber,
            pageSize);

        var total = await Db.GetDeliveryProfileHistoryFilteredCountAsync(
            connection,
            deliveryId,
            isActive,
            isRejected,
            isAccepted,
            submitDateFrom,
            submitDateTo,
            reviewDateFrom,
            reviewDateTo);

        return Success(new
        {
            Items = histories,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    [HttpPut("accept")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> Accept(
    [FromBody] DeliveryProfileHistoryId profileHistoryId)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        var profileHistory = await Db.GetDeliveryProfileHistoryByIdAsync(
            profileHistoryId.Id,
            connection,
            transaction);

        if (profileHistory is null)
        {
            transaction.Rollback();
            return DataNotFound("profileHistory Not Found");
        }
        if (profileHistory.IsAccepted == true)
        {
            transaction.Rollback();
            return Fail("profileHistory Is Already Accepted");
        }

        if (profileHistory.IsRejected == true)
        {
            transaction.Rollback();
            return Fail("profileHistory Is Already Rejected");
        }

        await Db.DeactivateDeliveryProfileHistoryByDeliveryIdAsync(
            connection,
            profileHistory.DeliveryId,
            false,
            transaction);

        profileHistory.ReviewDate = DateTime.Now;
        profileHistory.IsAccepted = true;
        profileHistory.IsRejected = false;
        profileHistory.IsActive = true;
        profileHistory.ReviewDescription = profileHistoryId.ReviewDescription;

        await Db.UpdateDeliveryProfileHistoryByIdAsync(
            connection,
            profileHistory.Id,
            profileHistory.IsActive,
            profileHistory.IsAccepted,
            profileHistory.IsRejected,
            profileHistoryId.ReviewDescription,
            transaction);

        transaction.Commit();
        return Success(profileHistory);
    }

    [HttpPut("reject")]
    [Guard([Roles.Admin])]
    public async Task<IActionResult> Reject(
    [FromBody] DeliveryProfileHistoryId profileHistoryId)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        var profileHistory = await Db.GetDeliveryProfileHistoryByIdAsync(
            profileHistoryId.Id,
            connection,
            transaction);

        if (profileHistory is null)
        {
            transaction.Rollback();
            return DataNotFound("profileHistory Not Found");
        }
        if (profileHistory.IsAccepted == true)
        {
            transaction.Rollback();
            return Fail("profileHistory Is Already Accepted");
        }

        if (profileHistory.IsRejected == true)
        {
            transaction.Rollback();
            return Fail("profileHistory Is Already Rejected");
        }


        profileHistory.ReviewDate = DateTime.Now;
        profileHistory.IsAccepted = false;
        profileHistory.IsRejected = true;
        profileHistory.IsActive = false;
        profileHistory.ReviewDescription = profileHistoryId.ReviewDescription;

        await Db.UpdateDeliveryProfileHistoryByIdAsync(
            connection,
            profileHistory.Id,
            profileHistory.IsActive,
            profileHistory.IsAccepted,
            profileHistory.IsRejected,
            profileHistoryId.ReviewDescription,
            transaction);

        transaction.Commit();
        return Success(profileHistory);
    }
}



public class DeliveryProfileHistoryInsertModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string NationalName { get; set; } = null!;
    public string NationalId { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public DateTime LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; } = null!;
    public DateTime DrivingLicenseExpiredDate { get; set; }
    public string VehicleNumber { get; set; } = null!;
    public string VehicleModel { get; set; } = null!;
    public string VehicleType { get; set; } = null!;
    public string VehicleYear { get; set; } = null!;
    public IFormFile? VehiclePhotoFile { get; set; } = null!;
    public IFormFile? DrivingLicenseFile { get; set; } = null!;
    public IFormFile? VehicleLicenseFile { get; set; } = null!;
    public IFormFile? NationalIqamaIDFile { get; set; } = null!;
}

public class DeliveryProfileHistoryId
{
    public long Id { get; set; }
    public string ReviewDescription { get; set; }
}