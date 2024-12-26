using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Customers.Profile;
using KiloMart.Domain.Register.Customer.Models;
using KiloMart.Domain.Register.Customer.Services;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Controllers;
using KiloMart.Presentation.Models.Commands.Customers;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/customer")]
public class CustomerCommandController : AppController
{
    private readonly IConfiguration _configuration;
    public CustomerCommandController(IDbFactory dbFactory,
    IConfiguration configuration,
    IUserContext userContext) : base(dbFactory, userContext)
    {
        _configuration = configuration;
    }
    #region register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCustomerDto dto)
    {
        var (success, errors) = dto.Validate();
        if (!success)
        {
            return ValidationError(errors);
            // return BadRequest(errors);
        }
        var result = await new RegisterCustomerService().Register(_dbFactory,
                            _configuration,
                            dto.Email,
                            dto.Password,
                            dto.DisplayName,
                            dto.Language);
        // return Ok(result);
        return result.IsSuccess
            ? Success(new
            {
                CustomerId = result.PartyId,
                result.UserId,
                result.VerificationToken
            }) : Fail(new string[] { result.ErrorMessage });
    }
    #endregion

    #region profile
    [HttpPost("profile/add")]
    public async Task<IActionResult> CreateProfile(CreateCustomerProfileApiRequest request)
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
        if(user.PasswordHash != HashHandler.GetHash(request.Password))
        {
            return Fail("Invalid Phone Number Or Password");
        }
        
        var result = await CustomerProfileService.InsertAsync(_dbFactory,
        new CreateCustomerProfileRequest
        {
            Customer = user.Party,
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            NationalId = request.NationalId,
            NationalName = request.NationalName

        });

        return result.Success ? Success(result) : Fail(result.Errors);
    }
    #endregion

    #region profile
    [HttpPost("profile/edit")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> EditProfile(UpdateCustomerProfileRequest request)
    {

        var result = await CustomerProfileService.UpdateAsync(_dbFactory,
            _userContext.Get(), request);

        return result.Success ? Success(result) : Fail(result.Errors);
    }
    #endregion

    [HttpGet("mine")]
    [Guard([Roles.Customer])]
    public async Task<IActionResult> GetMine()
    {
        var customer = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var query = @"
        SELECT
            [Id]
            ,[Customer]
            ,[FirstName]
            ,[SecondName]
            ,[NationalName]
            ,[NationalId]
        FROM [dbo].[CustomerProfile]
        WHERE [Customer] = @Customer";
        var result = await connection.QueryFirstOrDefaultAsync<CustomerProfile>(query, new { Customer = customer });
        if (result is null)
        {
            return DataNotFound();
        }
        var user = await Db.GetMembershipUserByIdAsync(_userContext.Get().Id, connection);
        var party = await Db.GetPartyByIdAsync(_userContext.Get().Party, connection);
        return Success(
            new 
            {
                profile = result,
                userInfo = new 
                {
                    user?.Id,
                    user?.Email,
                    user?.EmailConfirmed,
                    user?.IsActive,
                    user?.Role
                },
                customerInfo = party
            }
        );
    }

    // Assuming CustomerProfile class is defined as
    public class CustomerProfile
    {
        public int Id { get; set; }
        public int Customer { get; set; }
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public string NationalName { get; set; } = null!;
        public string NationalId { get; set; } = null!;
    }
    [HttpGet("admin/list")]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var result = await Query.GetCustomerProfilesWithUserInfoPaginated(connection, page, pageSize);
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

}
