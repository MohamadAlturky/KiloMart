using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Profiles
{
    [ApiController]
    [Route("api/provider-profile")]
    public class ProviderProfileHistoryController : AppController
    {
        private readonly IWebHostEnvironment _environment;

        public ProviderProfileHistoryController(
            IDbFactory dbFactory,
            IConfiguration configuration,
            IUserContext userContext,
            IWebHostEnvironment environment)
            : base(dbFactory, userContext)
        {
            _environment = environment;
        }

        [HttpPost("change")]
        public async Task<IActionResult> Insert(ProviderProfileHistoryInsertModel request)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            #region Getting the User
            var user = await Db.GetMembershipUserByEmailAsync(connection, request.Email);

            if (user is null)
            {
                return Fail("User Not Found");
            }
            if (user.PasswordHash != HashHandler.GetHash(request.Password))
            {
                return Fail("Invalid Email or Password");
            }
            #endregion

            try
            {
                #region File Uploads

                // OwnershipDocumentFile
                if (request.OwnershipDocumentFile == null)
                {
                    return Fail("OwnershipDocumentFile is null");
                }
                var ownershipDocumentFilePath = await FileService.SaveImageFileAsync(
                    request.OwnershipDocumentFile,
                    _environment.WebRootPath,
                    Guid.NewGuid());
                if (string.IsNullOrEmpty(ownershipDocumentFilePath))
                {
                    return Fail("Failed to save OwnershipDocumentFile");
                }

                // OwnerNationalApprovalFile
                if (request.OwnerNationalApprovalFile == null)
                {
                    return Fail("OwnerNationalApprovalFile is null");
                }
                var ownerNationalApprovalFilePath = await FileService.SaveImageFileAsync(
                    request.OwnerNationalApprovalFile,
                    _environment.WebRootPath,
                    Guid.NewGuid());
                if (string.IsNullOrEmpty(ownerNationalApprovalFilePath))
                {
                    return Fail("Failed to save OwnerNationalApprovalFile");
                }

                #endregion

                #region Insert into Database
                long id = await Db.InsertProviderProfileHistoryAsync(
                    connection,
                    request.FirstName,
                    request.SecondName,
                    request.NationalApprovalId,
                    request.CompanyName,
                    request.OwnerName,
                    request.OwnerNationalId,
                    ownershipDocumentFilePath,
                    ownerNationalApprovalFilePath,
                    request.LocationName,
                    request.Longitude,
                    request.Latitude,
                    request.BuildingType,
                    request.BuildingNumber,
                    request.FloorNumber,
                    request.ApartmentNumber,
                    request.StreetNumber,
                    request.PhoneNumber,
                    false, // isAccepted
                    false, // isRejected
                    DateTime.Now, // submitDate
                    null, // reviewDate
                    user.Party, // providerId
                    false); // isActive

                #endregion

                #region Returning The Response
                var model = new
                {
                    request.FirstName,
                    request.SecondName,
                    request.NationalApprovalId,
                    request.CompanyName,
                    request.OwnerName,
                    request.OwnerNationalId,
                    OwnershipDocumentFileUrl = ownershipDocumentFilePath,
                    OwnerNationalApprovalFileUrl = ownerNationalApprovalFilePath,
                    request.LocationName,
                    request.Longitude,
                    request.Latitude,
                    request.BuildingType,
                    request.BuildingNumber,
                    request.FloorNumber,
                    request.ApartmentNumber,
                    request.StreetNumber,
                    request.PhoneNumber,
                    IsAccepted = false,
                    IsRejected = false,
                    SubmitDate = DateTime.Now,
                    ReviewDate = (DateTime?)null,
                    ProviderId = user.Party,
                    IsActive = false
                };
                return Success(new { id, model });
                #endregion
            }
            catch (Exception ex)
            {
                return Fail($"Failed to insert provider profile history: {ex.Message}");
            }
        }










        [HttpPost("change/with-token")]
        [Guard([Roles.Provider])]
        public async Task<IActionResult> Change(ProviderProfileHistoryInsertModelByToken request)
        {
            int userId = _userContext.Get().Id;
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            #region Getting the User
            var user = await Db.GetMembershipUserByIdAsync(connection, userId);

            if (user is null)
            {
                return Fail("User Not Found");
            }

            #endregion
            try
            {

                string? ownershipDocumentFilePath = null;
                string? ownerNationalApprovalFilePath = null;

                var oldProfile = await Db.GetLatestProviderProfileHistoryAsync(connection, user.Party);
                if (oldProfile is null)
                {
                    return Fail("No Old Profile Found");
                }
                #region File Uploads

                // OwnershipDocumentFile
                if (request.OwnershipDocumentFile is not null)
                {

                    ownershipDocumentFilePath = await FileService.SaveImageFileAsync(
                        request.OwnershipDocumentFile,
                        _environment.WebRootPath,
                        Guid.NewGuid());
                    if (string.IsNullOrEmpty(ownershipDocumentFilePath))
                    {
                        return Fail("Failed to save OwnershipDocumentFile");
                    }
                }

                // OwnerNationalApprovalFile
                if (request.OwnerNationalApprovalFile is not null)
                {
                    ownerNationalApprovalFilePath = await FileService.SaveImageFileAsync(
                        request.OwnerNationalApprovalFile,
                        _environment.WebRootPath,
                        Guid.NewGuid());
                    if (string.IsNullOrEmpty(ownerNationalApprovalFilePath))
                    {
                        return Fail("Failed to save OwnerNationalApprovalFile");
                    }
                }

                #endregion

                #region Insert into Database
                long id = await Db.InsertProviderProfileHistoryAsync(
                    connection,
                    request.FirstName??oldProfile.FirstName,
                    request.SecondName??oldProfile.SecondName,
                    request.NationalApprovalId??oldProfile.NationalApprovalId,
                    request.CompanyName??oldProfile.CompanyName,
                    request.OwnerName??oldProfile.OwnerName,
                    request.OwnerNationalId??oldProfile.OwnerNationalId,
                    ownershipDocumentFilePath??oldProfile.OwnershipDocumentFileUrl,
                    ownerNationalApprovalFilePath??oldProfile.OwnerNationalApprovalFileUrl,
                    request.LocationName??oldProfile.LocationName,
                    request.Longitude??oldProfile.Longitude,
                    request.Latitude??oldProfile.Latitude,
                    request.BuildingType??oldProfile.BuildingType,
                    request.BuildingNumber??oldProfile.BuildingNumber,
                    request.FloorNumber??oldProfile.FloorNumber,
                    request.ApartmentNumber??oldProfile.ApartmentNumber,
                    request.StreetNumber??oldProfile.StreetNumber,
                    request.PhoneNumber??oldProfile.PhoneNumber,
                    false, // isAccepted
                    false, // isRejected
                    DateTime.Now, // submitDate
                    null, // reviewDate
                    user.Party, // providerId
                    false); // isActive

                #endregion

                #region Returning The Response
                var model = new
                {
                    FirstName = request.FirstName??oldProfile.FirstName,
                    SecondName = request.SecondName??oldProfile.SecondName,
                    NationalApprovalId = request.NationalApprovalId??oldProfile.NationalApprovalId,
                    CompanyName = request.CompanyName??oldProfile.CompanyName,
                    OwnerName = request.OwnerName??oldProfile.OwnerName,
                    OwnerNationalId = request.OwnerNationalId??oldProfile.OwnerNationalId,
                    ownershipDocumentFileUrl = ownershipDocumentFilePath??oldProfile.OwnershipDocumentFileUrl,
                    ownerNationalApprovalFileUrl = ownerNationalApprovalFilePath??oldProfile.OwnerNationalApprovalFileUrl,
                    LocationName = request.LocationName??oldProfile.LocationName,
                    Longitude = request.Longitude??oldProfile.Longitude,
                    Latitude = request.Latitude??oldProfile.Latitude,
                    BuildingType = request.BuildingType??oldProfile.BuildingType,
                    BuildingNumber = request.BuildingNumber??oldProfile.BuildingNumber,
                    FloorNumber = request.FloorNumber??oldProfile.FloorNumber,
                    ApartmentNumber = request.ApartmentNumber??oldProfile.ApartmentNumber,
                    StreetNumber = request.StreetNumber??oldProfile.StreetNumber,
                    PhoneNumber = request.PhoneNumber??oldProfile.PhoneNumber,
                    IsAccepted = false,
                    IsRejected = false,
                    SubmitDate = DateTime.Now,
                    ReviewDate = (DateTime?)null,
                    ProviderId = user.Party,
                    IsActive = false
                };
                return Success(new { id, model });
                #endregion
            }
            catch (Exception ex)
            {
                return Fail($"Failed to insert provider profile history: {ex.Message}");
            }
        }






















        [HttpGet("mine")]
        [Guard([Roles.Provider])]
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
            int providerId = _userContext.Get().Party;

            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();

            var histories = await Db.GetProviderProfileHistoryFilteredAsync(
                connection,
                providerId,
                isActive,
                isRejected,
                isAccepted,
                submitDateFrom,
                submitDateTo,
                reviewDateFrom,
                reviewDateTo,
                pageNumber,
                pageSize);

            var total = await Db.GetProviderProfileHistoryFilteredCountAsync(
                connection,
                providerId,
                isActive,
                isRejected,
                isAccepted,
                submitDateFrom,
                submitDateTo,
                reviewDateFrom,
                reviewDateTo);
            var user = await Db.GetMembershipUserByIdAsync(connection, _userContext.Get().Id);
            var party = await Db.GetPartyByIdAsync(providerId, connection);
            return Success(new
            {
                Items = histories,
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize,
                User = user,
                Provider = party
            });
        }

        [HttpGet("filter")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> GetFiltered(
            int? providerId = null,
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

            var histories = await Db.GetProviderProfileHistoryFilteredAsync(
                connection,
                providerId,
                isActive,
                isRejected,
                isAccepted,
                submitDateFrom,
                submitDateTo,
                reviewDateFrom,
                reviewDateTo,
                pageNumber,
                pageSize);

            var total = await Db.GetProviderProfileHistoryFilteredCountAsync(
                connection,
                providerId,
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
        public async Task<IActionResult> Accept([FromBody] ProviderProfileHistoryId profileHistoryId)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var profileHistory = await Db.GetProviderProfileHistoryByIdAsync(
                profileHistoryId.Id,
                connection,
                transaction);

            if (profileHistory is null)
            {
                transaction.Rollback();
                return DataNotFound("Profile History Not Found");
            }
            if (profileHistory.IsAccepted == true)
            {
                transaction.Rollback();
                return Fail("Profile History Is Already Accepted");
            }
            if (profileHistory.IsRejected == true)
            {
                transaction.Rollback();
                return Fail("Profile History Is Already Rejected");
            }

            await Db.DeactivateProviderProfileHistoryByProviderIdAsync(
                connection,
                profileHistory.ProviderId,
                false,
                transaction);

            profileHistory.ReviewDate = DateTime.Now;
            profileHistory.IsAccepted = true;
            profileHistory.IsRejected = false;
            profileHistory.IsActive = true;
            profileHistory.ReviewDescription = profileHistoryId.ReviewDescription;

            await Db.UpdateProviderProfileHistoryByIdAsync(
                connection,
                profileHistory.Id,
                profileHistory.IsActive,
                profileHistory.IsAccepted,
                profileHistory.IsRejected,
                profileHistoryId.ReviewDescription,
                transaction);


            await Db.DeactivateLocationByPartyAsync(
                connection,
                profileHistory.ProviderId,
                transaction);

            var locationId = await Db.InsertLocationAsync(
                 connection,
                 profileHistory.Longitude,
                 profileHistory.Latitude,
                 profileHistory.LocationName,
                 profileHistory.ProviderId,
                 transaction);

            await Db.InsertLocationDetailsAsync(
                connection,
                profileHistory.BuildingType,
                profileHistory.BuildingNumber,
                profileHistory.FloorNumber,
                profileHistory.ApartmentNumber,
                profileHistory.StreetNumber,
                profileHistory.PhoneNumber,
                locationId,
                transaction);

            transaction.Commit();
            return Success(profileHistory);
        }

        [HttpPut("reject")]
        [Guard([Roles.Admin])]
        public async Task<IActionResult> Reject([FromBody] ProviderProfileHistoryId profileHistoryId)
        {
            using var connection = _dbFactory.CreateDbConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var profileHistory = await Db.GetProviderProfileHistoryByIdAsync(
                profileHistoryId.Id,
                connection,
                transaction);

            if (profileHistory == null)
            {
                transaction.Rollback();
                return DataNotFound("Profile History Not Found");
            }
            if (profileHistory.IsAccepted == true)
            {
                transaction.Rollback();
                return Fail("Profile History Is Already Accepted");
            }
            if (profileHistory.IsRejected == true)
            {
                transaction.Rollback();
                return Fail("Profile History Is Already Rejected");
            }

            profileHistory.ReviewDate = DateTime.Now;
            profileHistory.IsAccepted = false;
            profileHistory.IsRejected = true;
            profileHistory.IsActive = false;
            profileHistory.ReviewDescription = profileHistoryId.ReviewDescription;

            await Db.UpdateProviderProfileHistoryByIdAsync(
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
}


public class ProviderProfileHistoryInsertModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string NationalApprovalId { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
    public string OwnerNationalId { get; set; } = null!;
    public IFormFile? OwnershipDocumentFile { get; set; }
    public IFormFile? OwnerNationalApprovalFile { get; set; }
    public string LocationName { get; set; } = null!;
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
    public string BuildingType { get; set; } = null!;
    public string BuildingNumber { get; set; } = null!;
    public string FloorNumber { get; set; } = null!;
    public string ApartmentNumber { get; set; } = null!;
    public string StreetNumber { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}


public class ProviderProfileHistoryInsertModelByToken
{
    public string? FirstName { get; set; } = null!;
    public string? SecondName { get; set; } = null!;
    public string? NationalApprovalId { get; set; } = null!;
    public string? CompanyName { get; set; } = null!;
    public string? OwnerName { get; set; } = null!;
    public string? OwnerNationalId { get; set; } = null!;
    public IFormFile? OwnershipDocumentFile { get; set; }
    public IFormFile? OwnerNationalApprovalFile { get; set; }
    public string? LocationName { get; set; } = null!;
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public string? BuildingType { get; set; } = null!;
    public string? BuildingNumber { get; set; } = null!;
    public string? FloorNumber { get; set; } = null!;
    public string? ApartmentNumber { get; set; } = null!;
    public string? StreetNumber { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
}



public class ProviderProfileHistoryId
{
    public long Id { get; set; }
    public string ReviewDescription { get; set; }
}