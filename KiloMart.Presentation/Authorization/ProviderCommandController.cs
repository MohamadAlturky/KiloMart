using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Documents;
using KiloMart.Domain.Providers.Profile;
using KiloMart.Domain.Register.Provider.Models;
using KiloMart.Domain.Register.Provider.Services;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Controllers;
using KiloMart.Presentation.Services;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/provider")]
public class ProviderCommandController(IConfiguration configuration,
 IDbFactory dbFactory,
  IUserContext userContext,
  IWebHostEnvironment environment) : AppController(dbFactory, userContext)
{
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterProviderDto dto)
    {
        var (success, errors) = dto.Validate();

        if (!success)
        {
            return ValidationError(errors);
        }

        var result = await new RegisterProviderService().Register(
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

    [HttpPost("profile/create")]
    public async Task<IActionResult> CreateProfile(CreateProviderProfileApiRequest request)
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

        var result = await ProviderProfileService.InsertAsync(
            writeConnection,
            transaction,
        new CreateProviderProfileRequest
        {
            Provider = user.Party,
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            NationalApprovalId = request.NationalApprovalId,
            CompanyName = request.CompanyName,
            OwnerName = request.OwnerName,
            OwnerNationalId = request.OwnerNationalId,
        });
        if (!result.Success)
        {
            transaction.Rollback();
            return Fail(result.Errors);
        }

        // Save OwnerNationalApprovalFile

        Guid fileName = Guid.NewGuid();
        if (request.OwnerNationalApprovalFile is null)
        {
            transaction.Rollback();
            return Fail("OwnerNationalApprovalFile is null");
        }
        var filePath = await FileService.SaveImageFileAsync(request.OwnerNationalApprovalFile,
            environment.WebRootPath,
            fileName);

        if (string.IsNullOrEmpty(filePath))
        {
            transaction.Rollback();
            return Fail("failed to save file");
        }
        int providerId = user.Party;
        int documentId = await Db.InsertProviderDocumentAsync(
            writeConnection,
            "OwnerNationalApproval",
            (byte)DocumentType.OwnerNationalApproval,
            filePath,
            providerId,
            transaction);


        // // Save NationalIqamaIDFile

        // fileName = Guid.NewGuid();
        // if (request.NationalIqamaIDFile is null)
        // {
        //     transaction.Rollback();
        //     return Fail("NationalIqamaIDFile is null");
        // }
        // filePath = await FileService.SaveImageFileAsync(request.NationalIqamaIDFile,
        //     environment.WebRootPath,
        //     fileName);

        // if (string.IsNullOrEmpty(filePath))
        // {
        //     transaction.Rollback();
        //     return Fail("failed to save file");
        // }
        // documentId = await Db.InsertProviderDocumentAsync(
        //     writeConnection,
        //     "NationalIqamaIDFile",
        //     (byte)DocumentType.NationalIqamaID,
        //     filePath,
        //     providerId,
        //     transaction);


        // Save OwnershipDocumentFile

        fileName = Guid.NewGuid();
        if (request.OwnershipDocumentFile is null)
        {
            transaction.Rollback();
            return Fail("OwnershipDocumentFile is null");
        }
        filePath = await FileService.SaveImageFileAsync(request.OwnershipDocumentFile,
            environment.WebRootPath,
            fileName);

        if (string.IsNullOrEmpty(filePath))
        {
            transaction.Rollback();
            return Fail("failed to save file");
        }
        documentId = await Db.InsertProviderDocumentAsync(
            writeConnection,
            "OwnershipDocumentFile",
            (byte)DocumentType.OwnershipDocument,
            filePath,
            providerId,
            transaction);

        transaction.Commit();
        return Success();
        //return result.Success ? Success(result) : Fail(result.Errors);
    }

    #region profile
    [HttpPost("profile/edit")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> EditProfile(UpdateProviderProfileRequest request)
    {

        var result = await ProviderProfileService.UpdateAsync(_dbFactory,
            _userContext.Get(), request);

        return result.Success ? Success(result) : Fail(result.Errors);
    }
    #endregion

    [HttpGet("mine")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetMine()
    {
        var provider = _userContext.Get().Party; // Assuming you have a way to get the current provider
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var query = "SELECT * FROM [dbo].[ProviderProfile] WHERE [Provider] = @Provider";
        var result = await connection.QueryFirstOrDefaultAsync<ProviderProfile>(query, new { Provider = provider });
        if (result is null)
        {
            return DataNotFound();
        }
        return Success(result);
    }

    // Define a class for ProviderProfile
    public class ProviderProfile
    {
        public int Id { get; set; }
        public int Provider { get; set; }
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public string OwnerNationalId { get; set; } = null!;
        public string NationalApprovalId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
    }

    [HttpGet("admin/list")]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Query.GetProviderProfilesWithUserInfoPaginated(connection, page, pageSize);
        if (result.Profiles is null || result.Profiles.Length == 0)
        {
            return DataNotFound();
        }
        return Success(
            new
            {
                Data = result.Profiles,
                TotalCount = result.TotalCount
            });
    }
}
