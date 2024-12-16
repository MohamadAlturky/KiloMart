using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Shared;

[ApiController]
[Route("api/notifications")]
public class NotificationsController(IDbFactory dbFactory, IUserContext userContext) : AppController(dbFactory, userContext)
{
    [HttpGet("paged")]
    [Guard([Roles.Admin, Roles.Delivery, Roles.Provider, Roles.Customer, Roles.Admin])]
    public async Task<IActionResult> GetPagedNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        int party = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();

        try
        {
            connection.Open();

            var (notifications, totalCount) = await Db.GetNotificationsPagedAsync(
                connection,
                party: party,
                pageNumber: pageNumber,
                pageSize: pageSize);

            return Success(new
            {
                Notifications = notifications.ToArray(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            return Fail(new string[] { ex.Message });
        }
    }

    [HttpGet("top-unread")]
    [Guard([Roles.Admin, Roles.Delivery, Roles.Provider, Roles.Customer, Roles.Admin])]
    public async Task<IActionResult> GetTopUnreadNotifications([FromQuery] int topCount = 10)
    {
        int party = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();

        try
        {
            connection.Open();

            var notifications = await Db.GetTopUnreadNotificationsAsync(
                connection,
                party: party,
                topCount: topCount);

            return Success(notifications.ToArray());
        }
        catch (Exception ex)
        {
            return Fail(new string[] { ex.Message });
        }
    }

    [HttpPost("mark-as-read")]
    [Guard([Roles.Admin, Roles.Delivery, Roles.Provider, Roles.Customer, Roles.Admin])]
    public async Task<IActionResult> MarkNotificationsAsRead([FromBody] List<long> notificationIds)
    {
        int party = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();

        try
        {
            connection.Open();

            int updatedCount = await Db.MarkNotificationsAsReadAsync(
                connection,
                idList: notificationIds,
                party: party);

            return Success(new { UpdatedCount = updatedCount });
        }
        catch (Exception ex)
        {
            return Fail(new string[] { ex.Message });
        }
    }
}