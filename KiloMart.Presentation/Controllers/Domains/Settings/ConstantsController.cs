using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Register.Utils;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Domains.Settings;

[ApiController]
[Route("api/constants")]
public partial class ConstantsController(IDbFactory dbFactory, IUserContext userContext)
 : AppController(dbFactory, userContext)
{
    [HttpGet("migrate-admin")]
    public async Task<IActionResult> Migrate()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        string email = "admin@system.kilomart";
        var membershipUser = await Db.GetMembershipUserByEmailAsync(
            email,
            connection);
        if (membershipUser is not null)
        {
            return Fail("user already exist");
        }
        // Create a new MembershipUser instance
        var newUser = new MembershipUser
        {
            Email = email,
            EmailConfirmed = true,
            PasswordHash = HashHandler.GetHash("12345@kilomart.admin.secret"),
            Role = 4,
            Party = 9,
            IsActive = true,
            Language = 1
        };
        int id = await Db.InsertMembershipUserAsync(
            connection,
            newUser.Email,
            newUser.EmailConfirmed,
            newUser.PasswordHash,
            newUser.Role,
            newUser.Party,
            newUser.Language);


        return Success(new { id });
    }

    [HttpGet("OrderActivityType")]
    public async Task<IActionResult> GetOrderActivityTypes()
    {
        var result = await DatabaseHelper.SelectFromTable("OrderActivityType", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    [HttpGet("FAQType")]
    public IActionResult GetFAQTypes()
    {
        var result = new List<DatabaseSettingsTable>()
        {
            new()
            {
                Id = 0,
                Name = "For all"
            },
            new()
            {
                Id = 1,
                Name = "For Customers"
            },
            new()
            {
                Id = 2,
                Name = "For Providers"
            },
            new()
            {
                Id = 3,
                Name = "For Delivery"
            }
        };
        return Success(result);
    }
    [HttpGet("PaymentType")]
    public async Task<IActionResult> GetPaymentTypes()
    {
        var result = await DatabaseHelper.SelectFromTable("PaymentType", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    [HttpGet("DiscountType")]
    public async Task<IActionResult> GetDiscountTypes()
    {
        var result = await DatabaseHelper.SelectFromTable("DiscountType", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    [HttpGet("DocumentType")]
    public async Task<IActionResult> GetDocumentTypes()
    {
        var result = await DatabaseHelper.SelectFromTable("DocumentType", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    [HttpGet("Language")]
    public async Task<IActionResult> GetLanguages()
    {
        var result = await DatabaseHelper.SelectFromTable("Language", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    // [HttpGet("OrderRequestStatus")]
    // public async Task<IActionResult> GetOrderRequestStatuses()
    // {
    //     var result = await DatabaseHelper.SelectFromTable("OrderRequestStatus", _dbFactory.CreateDbConnection());
    //     return Success(result);
    // }
    [HttpGet("OrderStatus")]
    public async Task<IActionResult> GetOrderStatuses()
    {
        var result = await DatabaseHelper.SelectFromTable("OrderStatus", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    [HttpGet("ProductRequestStatus")]
    public async Task<IActionResult> GetProductRequestStatuses()
    {
        var result = await DatabaseHelper.SelectFromTable("ProductRequestStatus", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    [HttpGet("Role")]
    public async Task<IActionResult> GetRoles()
    {
        var result = await DatabaseHelper.SelectFromTable("Role", _dbFactory.CreateDbConnection());
        return Success(result);
    }
    [HttpGet("DeliveryActivityType")]
    public async Task<IActionResult> GeDeliveryActivityTypes()
    {
        var result = await DatabaseHelper.SelectFromTable("DeliveryActivityType", _dbFactory.CreateDbConnection());
        return Success(result);
    }
}